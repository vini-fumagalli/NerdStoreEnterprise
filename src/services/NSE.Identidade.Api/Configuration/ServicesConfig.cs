using NSE.Identidade.Api.Services;
using NSE.Identidade.Api.Services.Interfaces;

namespace NSE.Identidade.Api.Configuration;

public static class ServicesConfig
{
    public static void AddServicesConfiguration(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IEmailService, EmailService>();
    }
}