using NSE.Pedidos.Api.Application.DTO;
using NSE.Pedidos.Domain.Pedidos;

namespace NSE.Pedidos.Api.Application.Queries;

public class PedidoQueries(IPedidoRepository repository) : IPedidoQueries
{
    public async Task<PedidoDTO> ObterUltimoPedido(int clienteId)
    {
        var pedido = await repository.ObterUltimoPedido(clienteId);
        return PedidoDTO.ParaPedidoDTO(pedido);
    }

    public async Task<IEnumerable<PedidoDTO>> ObterListaPorClienteId(int clienteId)
    {
        var pedidos = await repository.ObterListaPorClienteId(clienteId);
        return pedidos.Select(PedidoDTO.ParaPedidoDTO);
    }

    public async Task<PedidoDTO> ObterPedidosAutorizados()
    {
        var pedidos = await repository.ObterPedidosAutorizados();
        return PedidoDTO.ParaPedidoDTO(pedidos);
    }
}