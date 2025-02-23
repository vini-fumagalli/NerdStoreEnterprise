using Microsoft.EntityFrameworkCore;
using NSE.Carrinho.Api.Data;
using NSE.WebApi.Core.Usuario;

namespace NSE.Carrinho.Api.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection serviceCollection, 
        ConfigurationManager configuration, IWebHostEnvironment environment)
    {
        serviceCollection.AddHttpContextAccessor();
        serviceCollection.AddScoped<IAspNetUser, AspNetUser>();
        
        serviceCollection.AddDbContext<CarrinhoContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        serviceCollection.AddCors(options =>
        {
            options.AddPolicy("Total", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });
        
        configuration
            .AddJsonFile($"appsettings.{environment.EnvironmentName.ToLower()}.json", true, true)
            .AddEnvironmentVariables();
    }

    public static void UseCorsConfiguration(this IApplicationBuilder app)
    {
        app.UseCors("Total");
    }
}