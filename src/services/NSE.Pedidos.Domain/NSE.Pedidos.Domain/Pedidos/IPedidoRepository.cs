using NSE.Core.Data;

namespace NSE.Pedidos.Domain.Pedidos;

public interface IPedidoRepository : IRepository<Pedido>
{
    Task<Pedido> ObterPorId(int id);
    Task<IEnumerable<Pedido>> ObterListaPorClienteId(int clienteId);
    void Adicionar(Pedido pedido);
    void Atualizar(Pedido pedido);
    Task<PedidoItem> ObterItemPorId(int id);
    Task<PedidoItem> ObterItemPorPedido(int pedidoId, int produtoId);
    Task<Pedido> ObterUltimoPedido(int clienteId);
}