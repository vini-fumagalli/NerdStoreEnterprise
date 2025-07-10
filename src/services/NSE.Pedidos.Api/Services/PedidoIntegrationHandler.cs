using NSE.Core.DomainObjects;
using NSE.Core.Messages.Integration;
using NSE.MessageBus;
using NSE.Pedidos.Domain.Pedidos;

namespace NSE.Pedidos.Api.Services;

public class PedidoIntegrationHandler(
    IServiceProvider serviceProvider,
    IMessageBus bus
    ) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SetSubscribers();
        return Task.CompletedTask;
    }
    
    private void SetSubscribers()
    {
        bus.SubscribeAsync<PedidoCanceladoIntegrationEvent>("PedidoCancelado", CancelarPedido);

        bus.SubscribeAsync<PedidoPagoIntegrationEvent>("PedidoPago", FinalizarPedido);
    }

    private async Task CancelarPedido(PedidoCanceladoIntegrationEvent message)
    {
        using var scope = serviceProvider.CreateScope();
        var pedidoRepository = scope.ServiceProvider.GetRequiredService<IPedidoRepository>();

        var pedido = await pedidoRepository.ObterPorId(message.PedidoId);
        pedido.CancelarPedido();

        pedidoRepository.Atualizar(pedido);

        if (!await pedidoRepository.UnitOfWork.Commit())
        {
            throw new DomainException($"Problemas ao cancelar o pedido {message.PedidoId}");
        }
    }

    private async Task FinalizarPedido(PedidoPagoIntegrationEvent message)
    {
        using var scope = serviceProvider.CreateScope();
        var pedidoRepository = scope.ServiceProvider.GetRequiredService<IPedidoRepository>();

        var pedido = await pedidoRepository.ObterPorId(message.PedidoId);
        pedido.FinalizarPedido();

        pedidoRepository.Atualizar(pedido);

        if (!await pedidoRepository.UnitOfWork.Commit())
        {
            throw new DomainException($"Problemas ao finalizar o pedido {message.PedidoId}");
        }
    }
}