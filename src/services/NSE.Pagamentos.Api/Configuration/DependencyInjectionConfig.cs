using NSE.Pagamentos.Api.Data.Repository;
using NSE.Pagamentos.Api.Facade;
using NSE.Pagamentos.Api.Models;
using NSE.Pagamentos.Api.Services;
using NSE.WebApi.Core.Usuario;

namespace NSE.Pagamentos.Api.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IAspNetUser, AspNetUser>();
        services.AddScoped<IPagamentoService, PagamentoService>();
        services.AddScoped<IPagamentoFacade, PagamentoCartaoCreditoFacade>();
        services.AddScoped<IPagamentoRepository, PagamentoRepository>();
    }
}