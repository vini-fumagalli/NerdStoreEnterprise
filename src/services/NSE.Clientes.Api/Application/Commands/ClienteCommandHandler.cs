using FluentValidation.Results;
using MediatR;
using NSE.Clientes.Api.Application.Events;
using NSE.Clientes.Api.Models;
using NSE.Core.Messages;

namespace NSE.Clientes.Api.Application.Commands;

public class ClienteCommandHandler(IClienteRepository repository) : 
    CommandHandler, 
    IRequestHandler<RegistrarClienteCommand, ValidationResult>,
    IRequestHandler<AdicionarEnderecoCommand, ValidationResult>
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

        cliente.AdicionarEvento(new ClienteRegistradoEvent(message.Nome, message.Email, message.Cpf));
        
        return await PersitirDados(repository.UnitOfWork);
    }

    public async Task<ValidationResult> Handle(AdicionarEnderecoCommand message, CancellationToken cancellationToken)
    {
        if (!message.EhValido()) return message.ValidationResult;
        
        repository.AdicionarEndereco(new Endereco(message.Logradouro, message.Numero, message.Complemento, 
            message.Bairro, message.Cep, message.Cidade, message.Estado));
        
        return await PersitirDados(repository.UnitOfWork);
    }
}