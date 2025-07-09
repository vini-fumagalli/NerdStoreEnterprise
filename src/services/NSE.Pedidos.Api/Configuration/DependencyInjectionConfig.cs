using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSE.Core.Mediator;
using NSE.Core.Utils;
using NSE.MessageBus;
using NSE.Pedidos.Api.Application.Commands;
using NSE.Pedidos.Api.Application.Events;
using NSE.Pedidos.Api.Application.Queries;
using NSE.Pedidos.Api.Services;
using NSE.Pedidos.Domain.Pedidos;
using NSE.Pedidos.Domain.Vouchers;
using NSE.Pedidos.Infra.Data;
using NSE.Pedidos.Infra.Data.Repository;
using NSE.WebApi.Core.Usuario;

namespace NSE.Pedidos.Api.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services, ConfigurationManager configuration, 
        IWebHostEnvironment environment)
    {
        services.AddMediatR(typeof(Program));
        
        configuration
            .SetBasePath(environment.ContentRootPath)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables();
        
        services.AddDbContext<PedidosContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddCors(options =>
        {
            options.AddPolicy("Total", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });

        services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"))
            .AddHostedService<PedidoOrquestradorIntegrationHandler>();

        services.AddHttpContextAccessor();
        services.AddScoped<IAspNetUser, AspNetUser>();
        
        services.AddScoped<IRequestHandler<AdicionarPedidoCommand, ValidationResult>, PedidoCommandHandler>();
        
        services.AddScoped<INotificationHandler<PedidoRealizadoEvent>, PedidoEventHandler>();
        
        services.AddScoped<IMediatorHandler, MediatorHandler>();
        services.AddScoped<IVoucherQueries, VoucherQueries>();
        services.AddScoped<IPedidoQueries, PedidoQueries>();
        
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<IVoucherRepository, VoucherRepository>();
    }

    public static void UseCorsConfiguration(this IApplicationBuilder app)
    {
        app.UseCors("Total");
    }
}