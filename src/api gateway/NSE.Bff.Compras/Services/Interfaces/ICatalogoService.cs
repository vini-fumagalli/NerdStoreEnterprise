using NSE.Bff.Compras.Models;

namespace NSE.Bff.Compras.Services.Interfaces;

public interface ICatalogoService
{
    Task<ItemProdutoDTO> ObterPorId(int id);
    Task<IEnumerable<ItemProdutoDTO>> ObterItens(IEnumerable<int> ids);
}               