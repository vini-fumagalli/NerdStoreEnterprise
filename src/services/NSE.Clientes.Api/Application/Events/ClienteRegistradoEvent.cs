using MediatR;
using NSE.Core.Messages;

namespace NSE.Clientes.Api.Application.Events;

public class ClienteRegistradoEvent(string nome, string email, string cpf) : Event
{
    public string Nome { get; private set; } = nome;
    public string Email { get; private set; } = email;
    public string Cpf { get; private set; } = cpf;
}