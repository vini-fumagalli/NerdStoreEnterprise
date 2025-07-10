namespace NSE.Core.Messages.Integration;

public class PedidoBaixadoEstoqueIntegrationEvent(int clienteId, int pedidoId) : IntegrationEvent
{
    public int ClienteId { get; private set; } = clienteId;
    public int PedidoId { get; private set; } = pedidoId;
}