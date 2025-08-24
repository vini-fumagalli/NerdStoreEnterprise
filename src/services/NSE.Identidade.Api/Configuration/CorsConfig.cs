namespace NSE.Identidade.Api.Configuration;

public static class CorsConfig
{
    public static void AddCorsConfiguration(this IServiceCollection service)
    {
        service.AddCors(options =>
        {
            options.AddPolicy("Total", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });
    }

    public static void UseCorsConfiguration(this IApplicationBuilder app)
    {
        app.UseCors("Total");
    }
}