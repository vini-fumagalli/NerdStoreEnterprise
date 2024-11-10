using Microsoft.EntityFrameworkCore;
using NSE.Catalogo.Api.Models;
using NSE.Core.Data;

namespace NSE.Catalogo.Api.Data.Repository;

public class ProdutoRepository(CatalogoContext context) : IProdutoRepository
{
    public IUnitOfWork UnitOfWork => context;
    
    
    public async Task<IEnumerable<Produto>> ObterTodos()
    {
        return await context.Produtos.AsNoTracking().ToListAsync();
    }

    public async Task<Produto> ObterPorId(int id)
    {
        return await context.Produtos.FindAsync(id);
    }

    public async Task Adicionar(Produto produto)
    {
        await context.Produtos.AddAsync(produto);
    }

    public void Atualizar(Produto produto)
    {
        context.Produtos.Update(produto);
    }

    public void Dispose()
    {
        context?.Dispose();
    }
}