using NSE.Catalogo.Api.Models;
using NSE.Core.DomainObjects;
using NSE.Core.Messages.Integration;
using NSE.MessageBus;

namespace NSE.Catalogo.Api.Services;

public class CatalogoIntegrationHandler(
    IMessageBus bus,
    IServiceProvider serviceProvider
    ) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SetSubscribers();
        return Task.CompletedTask;
    }
    
    private void SetSubscribers()
    {
        bus.SubscribeAsync<PedidoAutorizadoIntegrationEvent>("PedidoAutorizado", BaixarEstoque);
    }

    private async Task BaixarEstoque(PedidoAutorizadoIntegrationEvent message)
    {
        using var scope = serviceProvider.CreateScope();
        var produtosComEstoque = new List<Produto>();
        var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();

        var idsProdutos = message.Itens.Select(c => c.Key);
        var produtos = await produtoRepository.ObterProdutosPorId(idsProdutos);

        if (produtos.Count != message.Itens.Count)
        {
            CancelarPedidoSemEstoque(message);
            return;
        }

        foreach (var produto in produtos)
        {
            var quantidadeProduto = message.Itens.FirstOrDefault(p => p.Key == produto.Id).Value;

            if (!produto.EstaDisponivel(quantidadeProduto)) continue;
            
            produto.RetirarEstoque(quantidadeProduto);
            produtosComEstoque.Add(produto);
        }

        if (produtosComEstoque.Count != message.Itens.Count)
        {
            CancelarPedidoSemEstoque(message);
            return;
        }

        foreach (var produto in produtosComEstoque)
        {
            produtoRepository.Atualizar(produto);
        }

        if (!await produtoRepository.UnitOfWork.Commit())
        {
            throw new DomainException($"Problemas ao atualizar estoque do pedido {message.PedidoId}");
        }

        var pedidoBaixado = new PedidoBaixadoEstoqueIntegrationEvent(message.ClienteId, message.PedidoId);
        await bus.PublishAsync(pedidoBaixado);
    }

    public async void CancelarPedidoSemEstoque(PedidoAutorizadoIntegrationEvent message)
    {
        var pedidoCancelado = new PedidoCanceladoIntegrationEvent(message.ClienteId, message.PedidoId);
        await bus.PublishAsync(pedidoCancelado);
    }
}