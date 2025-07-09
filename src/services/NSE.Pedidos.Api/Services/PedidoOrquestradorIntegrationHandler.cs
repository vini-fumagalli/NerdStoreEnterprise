using NSE.Core.Messages.Integration;
using NSE.MessageBus;
using NSE.Pedidos.Api.Application.Queries;

namespace NSE.Pedidos.Api.Services;

public class PedidoOrquestradorIntegrationHandler(
    ILogger<PedidoOrquestradorIntegrationHandler> logger,
    IServiceProvider serviceProvider
    ) : IHostedService, IDisposable
{
    private Timer _timer;
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Serviço de pedidos iniciado.");

        _timer = new Timer(ProcessarPedidos, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
        
        return Task.CompletedTask;
    }

    private async void ProcessarPedidos(object state)
    {
        using var scope = serviceProvider.CreateScope();
        var pedidoQueries = scope.ServiceProvider.GetRequiredService<IPedidoQueries>();
        var pedido = await pedidoQueries.ObterPedidosAutorizados();

        if (pedido == null) return;

        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        var pedidoAutorizado = new PedidoAutorizadoIntegrationEvent(pedido.ClienteId, pedido.Id,
            pedido.PedidoItems.ToDictionary(p => p.ProdutoId, p => p.Quantidade));

        await bus.PublishAsync(pedidoAutorizado);

        logger.LogInformation($"Pedido ID: {pedido.Id} foi encaminhado para baixa no estoque.");
    } 
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Serviço de pedidos finalizado");
        
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
        _timer?.Dispose();
    }
}