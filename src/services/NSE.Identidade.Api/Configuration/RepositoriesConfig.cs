using NSE.Identidade.Api.Data.Interfaces;
using NSE.Identidade.Api.Data.Repositories;
using NSE.Identidade.Api.Models;

namespace NSE.Identidade.Api.Configuration;

public static class RepositoriesConfig
{
    public static void AddRepositoriesConfiguration(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();
        serviceCollection.AddScoped<ICodAutRepository, CodAutRepository>();
    }
}