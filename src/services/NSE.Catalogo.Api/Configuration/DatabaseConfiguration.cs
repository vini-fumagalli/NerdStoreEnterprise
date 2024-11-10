using Microsoft.EntityFrameworkCore;
using NSE.Catalogo.Api.Data;

namespace NSE.Catalogo.Api.Configuration;

public static class DatabaseConfiguration
{
    public static void AddDatabaseConfiguration(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<CatalogoContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }
}