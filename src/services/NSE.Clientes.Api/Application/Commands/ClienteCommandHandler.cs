using FluentValidation.Results;
using MediatR;
using NSE.Clientes.Api.Application.Events;
using NSE.Clientes.Api.Models;
using NSE.Core.Messages;

namespace NSE.Clientes.Api.Application.Commands;

public class ClienteCommandHandler(IClienteRepository repository) : CommandHandler, IRequestHandler<RegistrarClienteCommand, ValidationResult>
{
    public async Task<ValidationResult> Handle(RegistrarClienteCommand message, CancellationToken cancellationToken)
    {
        if (!message.EhValido()) return message.ValidationResult;

        var cliente = new Cliente(message.Id, message.Nome, message.Email, message.Cpf);
        var clienteExistente = await repository.ObterPorCpf(cliente.Cpf.Numero);

        if (clienteExistente != null)
        {
            AdicionarErro("Este CPF já está em uso.");
            return ValidationResult;
        }

        await repository.Adicionar(cliente);

        cliente.AdicionarEvento(new ClienteRegistradoEvent(message.Id, message.Nome, message.Email, message.Cpf));
        
        return await PersitirDados(repository.UnitOfWork);
    }
}