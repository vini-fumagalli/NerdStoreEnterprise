using Microsoft.EntityFrameworkCore;
using NSE.Clientes.Api.Models;
using NSE.Core.Data;

namespace NSE.Clientes.Api.Data.Repositories;

public class ClienteRepository(ClientesContext context) : IClienteRepository
{
    public IUnitOfWork UnitOfWork => context;
    
    public async Task Adicionar(Cliente cliente)
    {
        await context.Clientes.AddAsync(cliente);
    }

    public async Task<IEnumerable<Cliente>> ObterTodos()
    {
        return await context.Clientes.AsNoTracking().ToListAsync();
    }

    public async Task<Cliente> ObterPorCpf(string cpf)
    {
        return await context.Clientes.SingleOrDefaultAsync(c => c.Cpf.Numero == cpf);
    }
    
    public void Dispose()
    {
        context.Dispose();
    }
}