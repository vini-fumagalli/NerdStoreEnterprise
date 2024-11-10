using NSE.Catalogo.Api.Configuration;
using NSE.WebApi.Core.Identidade;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddEnviromentConfiguration(builder.Environment);
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddRepositoriesConfiguration();
builder.Services.AddCorsConfiguration();
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