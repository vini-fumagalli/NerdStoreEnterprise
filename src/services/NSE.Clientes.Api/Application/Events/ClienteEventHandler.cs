using MediatR;

namespace NSE.Clientes.Api.Application.Events;

public class ClienteEventHandler : INotificationHandler<ClienteRegistradoEvent>
{
    public Task Handle(ClienteRegistradoEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}