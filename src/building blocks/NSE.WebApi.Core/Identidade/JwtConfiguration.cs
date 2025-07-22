using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.Security.JwtExtensions;

namespace NSE.WebApi.Core.Identidade;

public static class JwtConfiguration
{
    public static void AddJwtConfiguration(this IServiceCollection service, IConfiguration configuration)
    {
        var appSettingsSection = configuration.GetSection("AppSettings");
        service.Configure<AppSettings>(appSettingsSection);

        var appSettings = appSettingsSection.Get<AppSettings>();

        service.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.SetJwksOptions(new JwkOptions(appSettings.AutenticacaoJwksUrl));
        });
    }
}