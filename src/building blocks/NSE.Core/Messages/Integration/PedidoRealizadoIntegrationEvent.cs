namespace NSE.Core.Messages.Integration;

public class PedidoRealizadoIntegrationEvent(int clienteId) : IntegrationEvent
{
    public int ClienteId { get; private set; } = clienteId;
}