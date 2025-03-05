using System.Security.AccessControl;
using Microsoft.EntityFrameworkCore;
using NSE.Core.Data;
using NSE.Pedidos.Domain.Pedidos;

namespace NSE.Pedidos.Infra.Data.Repository;

public class PedidoRepository(PedidosContext context) : IPedidoRepository
{
    public IUnitOfWork UnitOfWork => context;
    
    public async Task<Pedido> ObterPorId(int id)
    {
        return await context.Pedidos.FindAsync(id);
    }

    public async Task<IEnumerable<Pedido>> ObterListaPorClienteId(int clienteId)
    {
        return await context.Pedidos
            .Include(p => p.PedidoItems)
            .AsNoTracking()
            .Where(p => p.ClienteId == clienteId)
            .ToListAsync();
    }

    public void Adicionar(Pedido pedido)
    {
        context.Pedidos.Add(pedido);
    }

    public void Atualizar(Pedido pedido)
    {
        context.Update(pedido);
    }

    public async Task<PedidoItem> ObterItemPorId(int id)
    {
        return await context.PedidoItems.FindAsync(id); 
    }

    public async Task<PedidoItem> ObterItemPorPedido(int pedidoId, int produtoId)
    {
        return await context.PedidoItems.FirstOrDefaultAsync(p => p.ProdutoId == produtoId && p.PedidoId == pedidoId);
    }

    public async Task<Pedido> ObterUltimoPedido(int clienteId)
    {
        return await context.Pedidos
            .Where(p => 
                p.ClienteId == clienteId &&
                p.DataCadastro >= DateTime.Now.AddMinutes(-3) && 
                p.DataCadastro <= DateTime.Now &&
                p.PedidoStatus == PedidoStatus.Autorizado)
            .OrderByDescending(p => p.DataCadastro)
            .FirstOrDefaultAsync();
    }

    public void Dispose()
    {
        context?.Dispose();
    }
}