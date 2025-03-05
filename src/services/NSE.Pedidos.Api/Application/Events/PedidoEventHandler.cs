using MediatR;
using NSE.Core.Messages.Integration;
using NSE.MessageBus;

namespace NSE.Pedidos.Api.Application.Events;

public class PedidoEventHandler(IMessageBus bus) : INotificationHandler<PedidoRealizadoEvent>
{
    public async Task Handle(PedidoRealizadoEvent message, CancellationToken cancellationToken)
    {
        await bus.PublishAsync(new PedidoRealizadoIntegrationEvent(message.ClienteId));
    }
}