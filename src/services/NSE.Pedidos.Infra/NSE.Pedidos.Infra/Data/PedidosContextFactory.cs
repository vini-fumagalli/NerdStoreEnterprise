using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NSE.Pedidos.Infra.Data;

public class PedidosContextFactory : IDesignTimeDbContextFactory<PedidosContext>
{
    public PedidosContext CreateDbContext(string[] args)
    {
        var ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "development";

        var rootDirectory = Directory.GetCurrentDirectory().Replace("NSE.Pedidos.Infra\\NSE.Pedidos.Infra", "");
        var file = Path.Combine(rootDirectory, "NSE.Pedidos.Api", $"appsettings.{ambiente}.json");
        var jsonString = File.ReadAllText(file);
        var json = JsonSerializer.Deserialize<JsonNode>(jsonString);
        string connectionString = (string)json!["ConnectionStrings"]!["DefaultConnection"]!;
        
        var optionsBuilder = new DbContextOptionsBuilder<PedidosContext>();
        optionsBuilder.UseSqlServer(connectionString);
        
        return new PedidosContext(optionsBuilder.Options, null);
    }
}