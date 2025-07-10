using NSE.Catalogo.Api.Services;
using NSE.Core.Utils;
using NSE.MessageBus;

namespace NSE.Catalogo.Api.Configuration;

public static class MessageBusConfig
{
    public static void AddMessageBusConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"))
            .AddHostedService<CatalogoIntegrationHandler>();
    }
}