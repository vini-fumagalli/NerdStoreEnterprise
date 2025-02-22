using FluentValidation.Results;
using NSE.Clientes.Api.Application.Commands;
using NSE.Core.Mediator;
using NSE.Core.Messages.Integration;
using NSE.MessageBus;

namespace NSE.Clientes.Api.Services;

public class RegistroClienteIntegrationHandler(
    IMessageBus bus,
    IServiceProvider serviceProvider) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SetResponder();
        return Task.CompletedTask;
    }
    
    private void SetResponder()
    {
        bus.RespondAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(RegistrarCliente);
        bus.AdvancedBus.Connected += OnConnect;
    }

    private void OnConnect(object s, EventArgs e) => SetResponder();
    
    
    private async Task<ResponseMessage> RegistrarCliente(UsuarioRegistradoIntegrationEvent message)
    {
        var clienteCommand = new RegistrarClienteCommand(message.Id, message.Nome, message.Email, message.Cpf);
        ValidationResult sucesso;

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
            sucesso = await mediator.EnviarComando(clienteCommand);
        }

        return new ResponseMessage(sucesso);
    }
}