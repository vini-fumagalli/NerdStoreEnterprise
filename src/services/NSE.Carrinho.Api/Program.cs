using NSE.Carrinho.Api.Configuration;
using NSE.WebApi.Core.Identidade;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.RegisterServices(builder.Configuration);
builder.Services.AddSwaggerGen();
builder.Services.AddJwtConfiguration(builder.Configuration);

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
app.Run();