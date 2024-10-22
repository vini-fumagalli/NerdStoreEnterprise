namespace NSE.Identidade.Api.Configuration;

public static class EnviromentConfig
{
    public static void AddEnviromentConfiguration(this ConfigurationManager configuration, IHostEnvironment environment)
    {
        configuration
            .AddJsonFile($"appsettings.{environment.EnvironmentName.ToLower()}.json", true, true)
            .AddEnvironmentVariables();
    }
}