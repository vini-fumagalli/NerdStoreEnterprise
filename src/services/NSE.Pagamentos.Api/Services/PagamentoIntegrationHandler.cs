using NSE.Core.DomainObjects;
using NSE.Core.Messages.Integration;
using NSE.MessageBus;
using NSE.Pagamentos.Api.Models;

namespace NSE.Pagamentos.Api.Services;

public class PagamentoIntegrationHandler(
    IMessageBus bus,
    IServiceProvider serviceProvider
    ) : BackgroundService
{
    private void SetResponder()
    {
        bus.RespondAsync<PedidoIniciadoIntegrationEvent, ResponseMessage>(AutorizarPagamento);
    }

    private void SetSubscribers()
    {
        bus.SubscribeAsync<PedidoCanceladoIntegrationEvent>("PedidoCancelado", CancelarPagamento);
        bus.SubscribeAsync<PedidoBaixadoEstoqueIntegrationEvent>("PedidoBaixadoEstoque", CapturarPagamento);
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SetResponder();
        SetSubscribers();
        return Task.CompletedTask;
    }

    private async Task CancelarPagamento(PedidoCanceladoIntegrationEvent message)
    {
        using var scope = serviceProvider.CreateScope();
        var pagamentoService = scope.ServiceProvider.GetRequiredService<IPagamentoService>();

        var response = await pagamentoService.CancelarPagamento(message.PedidoId);

        if (response.ValidationResult.IsValid) return;
            
        throw new DomainException($"Falha ao cancelar pagamento do pedido {message.PedidoId}");
    }
    
    private async Task<ResponseMessage> AutorizarPagamento(PedidoIniciadoIntegrationEvent message)
    {
        using var scope = serviceProvider.CreateScope();
        var pagamentoService = scope.ServiceProvider.GetRequiredService<IPagamentoService>();
        var pagamento = new Pagamento
        {
            PedidoId = message.PedidoId,
            TipoPagamento = (TipoPagamento)message.TipoPagamento,
            Valor = message.Valor,
            CartaoCredito = new CartaoCredito(
                message.NomeCartao, message.NumeroCartao, message.MesAnoVencimento, message.CVV)
        };

        var response = await pagamentoService.AutorizarPagamento(pagamento);

        return response;
    }
    
    private async Task CapturarPagamento(PedidoBaixadoEstoqueIntegrationEvent message)
    {
        using var scope = serviceProvider.CreateScope();
        var pagamentoService = scope.ServiceProvider.GetRequiredService<IPagamentoService>();

        var response = await pagamentoService.CapturarPagamento(message.PedidoId);

        if (!response.ValidationResult.IsValid)
        {
            throw new DomainException($"Falha ao capturar pagamento do pedido {message.PedidoId}");
        }

        await bus.PublishAsync(new PedidoPagoIntegrationEvent(message.ClienteId, message.PedidoId));
    }
}