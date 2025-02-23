using NSE.Bff.Compras.Models;
using NSE.Core.Communication;

namespace NSE.Bff.Compras.Services.Interfaces;

public interface ICarrinhoService
{
    Task<CarrinhoDTO> ObterCarrinho();
    Task<ResponseResult> AdicionarItemCarrinho(ItemCarrinhoDTO produto);
    Task<ResponseResult> AtualizarItemCarrinho(int produtoId, ItemCarrinhoDTO carrinho);
    Task<ResponseResult> RemoverItemCarrinho(int produtoId);
    Task<ResponseResult> AplicarVoucherCarrinho(VoucherDTO voucher);
}