using FluentValidation.Results;
using MediatR;
using NSE.Core.Messages;

namespace NSE.Core.Mediator;

public class MediatorHandler(IMediator mediator) : IMediatorHandler
{
    public async Task PublicarEvento<T>(T evento) where T : Event
    {
        await mediator.Publish(evento);
    }

    public async Task<ValidationResult> EnviarComando<T>(T comando) where T : Command
    {
        return await mediator.Send(comando);
    }
}