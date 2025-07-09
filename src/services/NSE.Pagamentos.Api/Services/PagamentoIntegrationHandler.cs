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
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SetResponder();
        return Task.CompletedTask;
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
}