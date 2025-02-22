using Microsoft.Extensions.DependencyInjection;

namespace NSE.MessageBus;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMessageBus(this IServiceCollection serviceCollection, string connection)
    {
        ArgumentException.ThrowIfNullOrEmpty(connection);

        serviceCollection.AddSingleton<IMessageBus>(new MessageBus(connection));

        return serviceCollection;
    }
}