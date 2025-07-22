using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSE.Identidade.Api.Data;
using NSE.Identidade.Api.Extensions;

namespace NSE.Identidade.Api.Configuration;

public static class IdentityConfig
{
    public static void AddIdentityConfiguration(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddJwksManager()
            .PersistKeysToDatabaseStore<ApplicationDbContext>()
            .UseJwtValidation();
        
        service.AddDbContext<IdentidadeDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        service.AddDbContext<ApplicationDbContext>(options => 
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        service.AddDefaultIdentity<IdentityUser<int>>()
            .AddRoles<IdentityRole<int>>()
            .AddErrorDescriber<IdentityMensagensPortugues>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }
}