using Microsoft.EntityFrameworkCore;
using NSE.Identidade.Api.Models;

namespace NSE.Identidade.Api.Data;

public class IdentidadeDbContext : DbContext
{
    public DbSet<CodAut> CodAut { get; set; }
    public DbSet<RefreshTokens> RefreshTokens { get; set; }
    
    public IdentidadeDbContext(DbContextOptions<IdentidadeDbContext> options) : base(options) { }
}