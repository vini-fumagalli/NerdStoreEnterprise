using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSE.Identidade.Api.Data;
using NSE.Identidade.Api.Extensions;
using NSE.WebApi.Core.Identidade;

namespace NSE.Identidade.Api.Configuration;

public static class IdentityConfig
{
    public static void AddIdentityConfiguration(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddDbContext<IdentidadeDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        service.AddDbContext<ApplicationDbContext>(options => 
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        service.AddDefaultIdentity<IdentityUser<int>>()
            .AddRoles<IdentityRole<int>>()
            .AddErrorDescriber<IdentityMensagensPortugues>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        
        service.AddJwtConfiguration(configuration);
    }
}