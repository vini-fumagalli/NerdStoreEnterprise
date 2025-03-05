using Microsoft.EntityFrameworkCore;
using NSE.Carrinho.Api.Data;
using NSE.Core.Messages.Integration;
using NSE.MessageBus;

namespace NSE.Carrinho.Api.Services;

public class CarrinhoIntegrationHandler(IMessageBus bus, IServiceProvider serviceProvider) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SetSubscribers();
        return Task.CompletedTask;
    }

    private void SetSubscribers()
    {
        bus.SubscribeAsync<PedidoRealizadoIntegrationEvent>("PedidoRealizado", ApagarCarrinho);
    }

    private async Task ApagarCarrinho(PedidoRealizadoIntegrationEvent message)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CarrinhoContext>();
        
        var carrinho = await context.CarrinhoCliente.FirstOrDefaultAsync(c => c.ClienteId == message.ClienteId);
        if (carrinho != null)
        {
            context.CarrinhoCliente.Remove(carrinho);
            await context.SaveChangesAsync();
        }
    }
}