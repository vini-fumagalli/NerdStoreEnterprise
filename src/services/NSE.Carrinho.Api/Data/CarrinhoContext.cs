using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using NSE.Carrinho.Api.Models;
using NSE.Core.Data;
using NSE.Core.Messages;

namespace NSE.Carrinho.Api.Data;

public class CarrinhoContext : DbContext, IUnitOfWork
{
    public CarrinhoContext(DbContextOptions<CarrinhoContext> options)
        : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.AutoDetectChangesEnabled = false;
    }
    
    public DbSet<CarrinhoItem> CarrinhoItens { get; set; }
    public DbSet<CarrinhoCliente> CarrinhoCliente { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                     e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
            property.SetColumnType("varchar(100)");

        modelBuilder.Ignore<ValidationResult>();
        modelBuilder.Ignore<Event>();

        modelBuilder.Entity<CarrinhoCliente>()
            .HasIndex(c => c.ClienteId)
            .HasName("IDX_Cliente");
        
        modelBuilder.Entity<CarrinhoCliente>()
            .Ignore(c => c.Voucher)
            .OwnsOne(c => c.Voucher, v =>
            {
                v.Property(vc => vc.Codigo)
                    .HasColumnName("VoucherCodigo")
                    .HasColumnType("varchar(50)");

                v.Property(vc => vc.TipoDesconto)
                    .HasColumnName("TipoDesconto");

                v.Property(vc => vc.Percentual)
                    .HasColumnName("Percentual");

                v.Property(vc => vc.ValorDesconto)
                    .HasColumnName("ValorDesconto");
            });

        modelBuilder.Entity<CarrinhoCliente>()
            .HasMany(c => c.Itens)
            .WithOne(i => i.CarrinhoCliente)
            .HasForeignKey(c => c.CarrinhoId);

        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())) 
            relationship.DeleteBehavior = DeleteBehavior.Cascade;
    }

    public async Task<bool> Commit()
    {
        return await base.SaveChangesAsync() > 0;
    }
}