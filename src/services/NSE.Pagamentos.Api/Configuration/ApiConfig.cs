using Microsoft.EntityFrameworkCore;
using NSE.Pagamentos.Api.Data;
using NSE.Pagamentos.Api.Facade;

namespace NSE.Pagamentos.Api.Configuration;

public static class ApiConfig
{
    public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PagamentoContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.Configure<PagamentoConfig>(configuration.GetSection("PagamentoConfig"));

        services.AddCors(options =>
        {
            options.AddPolicy("Total",
                builder =>
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
        });
    }
    
    public static void UseApiConfiguration(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("Total");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}