using NSE.Identidade.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCorsConfiguration();
builder.Services.AddIdentityConfiguration(builder.Configuration);
builder.Services.AddRepositoriesConfiguration();
builder.Services.AddServicesConfiguration(builder.Configuration);
builder.Services.AddMessageBusConfiguration(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCorsConfiguration();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseJwksDiscovery();
app.Run();