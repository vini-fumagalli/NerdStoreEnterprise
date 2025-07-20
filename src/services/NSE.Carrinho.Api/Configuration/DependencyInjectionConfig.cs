using Microsoft.EntityFrameworkCore;
using NSE.Carrinho.Api.Data;
using NSE.Carrinho.Api.Services;
using NSE.Core.Utils;
using NSE.MessageBus;
using NSE.WebApi.Core.Usuario;

namespace NSE.Carrinho.Api.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection serviceCollection, ConfigurationManager configuration)
    {
        serviceCollection.AddHttpContextAccessor();
        serviceCollection.AddScoped<IAspNetUser, AspNetUser>();
        
        serviceCollection.AddDbContext<CarrinhoContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        serviceCollection.AddCors(options =>
        {
            options.AddPolicy("Total", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });
        
        serviceCollection.AddMessageBusConfiguration(configuration);
    }

    public static void UseCorsConfiguration(this IApplicationBuilder app)
    {
        app.UseCors("Total");
    }

    private static void AddMessageBusConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"))
            .AddHostedService<CarrinhoIntegrationHandler>();
    }
}