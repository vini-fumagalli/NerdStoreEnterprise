using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using NSE.Catalogo.Api.Models;
using NSE.Core.Data;
using NSE.Core.Messages;

namespace NSE.Catalogo.Api.Data;

public class CatalogoContext : DbContext, IUnitOfWork
{
    public CatalogoContext(DbContextOptions<CatalogoContext> options) : base(options) { }

    public DbSet<Produto> Produtos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        IgnorarPropriedades(modelBuilder);
        TravarVarcharMax(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogoContext).Assembly);
    }
    
    private static void IgnorarPropriedades(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Event>();
        modelBuilder.Ignore<ValidationResult>();
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