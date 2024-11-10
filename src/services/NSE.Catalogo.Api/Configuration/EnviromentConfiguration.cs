namespace NSE.Catalogo.Api.Configuration;

public static class EnviromentConfiguration
{
    public static void AddEnviromentConfiguration(this ConfigurationManager configuration, IHostEnvironment environment)
    {
        configuration
            .AddJsonFile($"appsettings.{environment.EnvironmentName.ToLower()}.json", true, true)
            .AddEnvironmentVariables();
    }
}