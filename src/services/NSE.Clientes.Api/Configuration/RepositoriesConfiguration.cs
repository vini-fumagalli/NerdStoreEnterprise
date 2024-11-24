using FluentValidation.Results;
using MediatR;
using NSE.Clientes.Api.Application.Events;
using NSE.Clientes.Api.Application.Commands;
using NSE.Clientes.Api.Data.Repositories;
using NSE.Clientes.Api.Models;
using NSE.Core.Mediator;

namespace NSE.Clientes.Api.Configuration;

public static class RepositoriesConfiguration
{
    public static void AddRepositoriesConfiguration(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediatR(typeof(Program));
        serviceCollection.AddScoped<IMediatorHandler, MediatorHandler>();
        serviceCollection.AddScoped<IRequestHandler<RegistrarClienteCommand, ValidationResult>, ClienteCommandHandler>();
        serviceCollection.AddScoped<INotificationHandler<ClienteRegistradoEvent>, ClienteEventHandler>();

        serviceCollection.AddScoped<IClienteRepository, ClienteRepository>();
    }
}