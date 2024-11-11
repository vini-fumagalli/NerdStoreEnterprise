using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NSE.Identidade.Api.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser<int>,IdentityRole<int>,int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}