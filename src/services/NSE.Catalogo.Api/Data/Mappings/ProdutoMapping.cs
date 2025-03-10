using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSE.Catalogo.Api.Models;

namespace NSE.Catalogo.Api.Data.Mappings;

public class ProdutoMapping : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasColumnType("varchar(250)");
        
        builder.Property(p => p.Descricao)
            .IsRequired()
            .HasColumnType("varchar(500)");
        
        builder.Property(p => p.Imagem)
            .IsRequired()
            .HasColumnType("varchar(250)");

        builder.ToTable("Produtos");
    }
}