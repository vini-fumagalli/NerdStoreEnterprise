namespace NSE.Clientes.Api.Configuration;

public static class CorsConfiguration
{
    public static void AddCorsConfiguration(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddCors(options =>
        {
            options.AddPolicy("Total", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });
    }

    public static void UseCorsConfiguration(this IApplicationBuilder app) => app.UseCors("Total");
}