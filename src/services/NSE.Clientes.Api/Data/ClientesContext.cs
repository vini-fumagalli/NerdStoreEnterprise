using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using NSE.Clientes.Api.Models;
using NSE.Core.Data;
using NSE.Core.DomainObjects;
using NSE.Core.Mediator;
using NSE.Core.Messages;

namespace NSE.Clientes.Api.Data;

public class ClientesContext : DbContext, IUnitOfWork
{
    private readonly IMediatorHandler _mediatorHandler;
    public ClientesContext(DbContextOptions<ClientesContext> options, IMediatorHandler mediatorHandler)
        : base(options)
    {
        _mediatorHandler = mediatorHandler; 
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.AutoDetectChangesEnabled = false;
    }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Endereco> Enderecos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        IgnorarPropriedades(modelBuilder);
        TravarVarcharMax(modelBuilder);
        TravarDeleteEmCascata(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientesContext).Assembly);
    }

    private static void IgnorarPropriedades(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Event>();
        modelBuilder.Ignore<ValidationResult>();
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
        var sucesso = await base.SaveChangesAsync() > 0;

        if (sucesso) await _mediatorHandler.PublicarEventos(this);

        return sucesso;
    }
}

public static class MediatorExtension
{
    public static async Task PublicarEventos<T>(this IMediatorHandler mediatorHandler, T context) where T : DbContext
    {
        var domainEntities = context.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.Notificacoes != null && x.Entity.Notificacoes.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.Notificacoes)
            .ToList();
        
        domainEntities.ToList().ForEach(de => de.Entity.LimparEventos());

        var tasks = domainEvents
            .Select(async domainEvent =>
            {
                await mediatorHandler.PublicarEvento(domainEvent);
            });

        await Task.WhenAll(tasks);
    }
}