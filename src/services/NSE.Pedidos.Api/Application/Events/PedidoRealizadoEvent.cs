using NSE.Core.Messages;

namespace NSE.Pedidos.Api.Application.Events;

public class PedidoRealizadoEvent(int pedidoId, int clienteId) : Event
{
    public int PedidoId { get; private set; } = pedidoId;
    public int ClienteId { get; private set; } = clienteId;
}