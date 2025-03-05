using NSE.Bff.Compras.Models;
using NSE.Core.Communication;

namespace NSE.Bff.Compras.Services.Interfaces;

public interface IPedidoService
{
    Task<ResponseResult> FinalizarPedido(PedidoDTO pedido);
    Task<PedidoDTO> ObterUltimoPedido();
    Task<IEnumerable<PedidoDTO>> ObterListaPorClienteId();
    Task<VoucherDTO> ObterVoucherPorCodigo(string codigo);
}