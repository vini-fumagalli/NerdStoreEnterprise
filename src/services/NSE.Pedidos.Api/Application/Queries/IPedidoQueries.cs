using NSE.Pedidos.Api.Application.DTO;

namespace NSE.Pedidos.Api.Application.Queries;

public interface IPedidoQueries
{
    Task<PedidoDTO> ObterUltimoPedido(int clienteId);
    Task<IEnumerable<PedidoDTO>> ObterListaPorClienteId(int clienteId);
}