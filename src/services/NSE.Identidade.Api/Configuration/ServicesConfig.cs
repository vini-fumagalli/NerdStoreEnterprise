using NSE.Identidade.Api.Services;
using NSE.Identidade.Api.Services.Interfaces;
using NSE.Identidade.Api.Utils;
using NSE.WebApi.Core.Usuario;

namespace NSE.Identidade.Api.Configuration;

public static class ServicesConfig
{
    public static void AddServicesConfiguration(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddOptions();
        serviceCollection.Configure<TokenSettings>(configuration.GetSection("TokenSettings"));
        serviceCollection.AddTransient<IEmailService, EmailService>();
        serviceCollection.AddScoped<IAspNetUser, AspNetUser>();
    }
}