using Microsoft.Extensions.DependencyInjection;

namespace NSE.MessageBus;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMessageBus(this IServiceCollection serviceCollection, string connection)
    {
        if (string.IsNullOrEmpty(connection)) throw new ArgumentException();

        serviceCollection.AddSingleton<IMessageBus>(new MessageBus(connection));

        return serviceCollection;
    }
}