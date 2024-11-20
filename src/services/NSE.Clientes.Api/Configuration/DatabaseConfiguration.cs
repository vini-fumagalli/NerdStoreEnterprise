using Microsoft.EntityFrameworkCore;
using NSE.Clientes.Api.Data;

namespace NSE.Clientes.Api.Configuration;

public static class DatabaseConfiguration
{
    public static void AddDatabaseConfiguration(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<ClientesContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }
}