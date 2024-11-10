using NSE.Catalogo.Api.Data.Repository;
using NSE.Catalogo.Api.Models;

namespace NSE.Catalogo.Api.Configuration;

public static class RepositoriesConfiguration
{
    public static void AddRepositoriesConfiguration(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IProdutoRepository, ProdutoRepository>();
    }
}