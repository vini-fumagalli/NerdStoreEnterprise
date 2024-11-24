using MediatR;
using NSE.Core.Messages;

namespace NSE.Clientes.Api.Application.Events;

public class ClienteRegistradoEvent : Event
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string Cpf { get; private set; }

    public ClienteRegistradoEvent(int id, string nome, string email, string cpf)
    {
        AggregateId = id;
        Id = id;
        Nome = nome;
        Email = email;
        Cpf = cpf;
    }
}