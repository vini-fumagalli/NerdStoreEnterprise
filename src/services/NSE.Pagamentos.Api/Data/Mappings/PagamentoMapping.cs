using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSE.Pagamentos.Api.Models;

namespace NSE.Pagamentos.Api.Data.Mappings;

public class PagamentoMapping : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Ignore(c => c.CartaoCredito);
        
        builder.HasMany(c => c.Transacoes)
            .WithOne(c => c.Pagamento)
            .HasForeignKey(c => c.PagamentoId);

        builder.ToTable("Pagamentos");
    }
}