using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Security.Jwt.Core.Model;
using NetDevPack.Security.Jwt.Store.EntityFrameworkCore;

namespace NSE.Identidade.Api.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser<int>,IdentityRole<int>,int>, ISecurityKeyContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<KeyMaterial> SecurityKeys { get; set; }
}