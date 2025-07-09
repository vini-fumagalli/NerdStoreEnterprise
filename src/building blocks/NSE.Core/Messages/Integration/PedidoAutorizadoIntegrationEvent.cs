namespace NSE.Core.Messages.Integration;

public class PedidoAutorizadoIntegrationEvent(int clienteId, int pedidoId, IDictionary<int, int> itens)
    : IntegrationEvent
{
    public int ClienteId { get; private set; } = clienteId;
    public int PedidoId { get; private set; } = pedidoId;
    public IDictionary<int, int> Itens { get; private set; } = itens;
}