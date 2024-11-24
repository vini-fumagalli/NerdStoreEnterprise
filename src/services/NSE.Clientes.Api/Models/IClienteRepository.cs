using NSE.Clientes.Api.Application.Commands;
using NSE.Core.Data;

namespace NSE.Clientes.Api.Models;

public interface IClienteRepository : IRepository<Cliente>
{
    Task Adicionar(Cliente cliente);
    Task<IEnumerable<Cliente>> ObterTodos();
    Task<Cliente> ObterPorCpf(string cpf);
}