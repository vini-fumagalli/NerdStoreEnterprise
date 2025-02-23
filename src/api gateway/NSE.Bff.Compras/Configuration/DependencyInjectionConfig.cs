using NSE.Bff.Compras.Extensions;
using NSE.Bff.Compras.Services;
using NSE.Bff.Compras.Services.Interfaces;
using NSE.WebApi.Core.Extensions;
using NSE.WebApi.Core.Usuario;
using Polly;

namespace NSE.Bff.Compras.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddHttpContextAccessor();
        serviceCollection.AddTransient<HttpClientAuthorizationDelegatingHandler>();
        
        serviceCollection.AddHttpClient<ICatalogoService, CatalogoService>().AddMessageAndPolicyHandler();
        serviceCollection.AddHttpClient<ICarrinhoService, CarrinhoService>().AddMessageAndPolicyHandler();
        
        serviceCollection.AddScoped<IAspNetUser, AspNetUser>();
        serviceCollection.Configure<AppServicesSettings>(configuration);
        
        serviceCollection.AddCors(options =>
        {
            options.AddPolicy("Total", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });
    }
    
    private static void AddMessageAndPolicyHandler(this IHttpClientBuilder builder)
    {
        builder.AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
            .AddPolicyHandler(PollyExtensions.EsperarTentar())
            .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
    }
    
    public static void UseCorsConfiguration(this IApplicationBuilder app)
    {
        app.UseCors("Total");
    }
}