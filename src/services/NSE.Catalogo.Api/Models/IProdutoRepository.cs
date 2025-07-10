using Microsoft.EntityFrameworkCore;
using NSE.Core.Data;

namespace NSE.Catalogo.Api.Models;

public interface IProdutoRepository : IRepository<Produto>
{
    Task<IEnumerable<Produto>> ObterTodos();
    Task<Produto> ObterPorId(int id);
    Task Adicionar(Produto produto);
    void Atualizar(Produto produto);
    Task<List<Produto>> ObterProdutosPorId(IEnumerable<int> idCollection);
}