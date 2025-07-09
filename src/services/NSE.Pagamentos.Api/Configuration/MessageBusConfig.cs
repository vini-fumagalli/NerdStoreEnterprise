using NSE.Core.Utils;
using NSE.MessageBus;
using NSE.Pagamentos.Api.Services;

namespace NSE.Pagamentos.Api.Configuration;

public static class MessageBusConfig
{
    public static void AddMessageBusConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"))
            .AddHostedService<PagamentoIntegrationHandler>();
    }
}