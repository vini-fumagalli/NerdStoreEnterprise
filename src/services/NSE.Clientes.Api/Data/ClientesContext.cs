using Microsoft.EntityFrameworkCore;
using NSE.Clientes.Api.Models;
using NSE.Core.Data;

namespace NSE.Clientes.Api.Data;

public class ClientesContext : DbContext, IUnitOfWork
{
    public ClientesContext(DbContextOptions<ClientesContext> options)
        : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.AutoDetectChangesEnabled = false;
    }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Endereco> Enderecos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        TravarVarcharMax(modelBuilder);
        TravarDeleteEmCascata(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientesContext).Assembly);
    }

    private static void TravarDeleteEmCascata(ModelBuilder modelBuilder)
    {
        var foreignKeys = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()).ToList();
        
        foreignKeys.ForEach(fk => fk.DeleteBehavior = DeleteBehavior.ClientSetNull);
    }
    
    private static void TravarVarcharMax(ModelBuilder modelBuilder)
    {
        var varcharProperties = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(e => e.GetProperties().Where(p => p.ClrType == typeof(string)))
            .ToList();
        
        varcharProperties.ForEach(vp => vp.SetColumnType("varchar(100)"));
    }

    public async Task<bool> Commit()
    {
        return await base.SaveChangesAsync() > 0;
    }
}