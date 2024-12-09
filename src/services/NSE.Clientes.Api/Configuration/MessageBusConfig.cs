using NSE.Clientes.Api.Services;
using NSE.Core.Utils;
using NSE.MessageBus;

namespace NSE.Clientes.Api.Configuration;

public static class MessageBusConfig
{
    public static void AddMessageBusConfiguration(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"))
            .AddHostedService<RegistroClienteIntegrationHandler>();
    }
}