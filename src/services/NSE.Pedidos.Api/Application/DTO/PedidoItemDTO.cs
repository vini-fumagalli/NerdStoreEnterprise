using NSE.Pedidos.Domain.Pedidos;

namespace NSE.Pedidos.Api.Application.DTO;

public class PedidoItemDTO
{
    public int PedidoId { get; set; }
    public int ProdutoId { get; set; }
    public string Nome { get; set; }
    public decimal Valor { get; set; }
    public string Imagem { get; set; }
    public int Quantidade { get; set; }

    public static PedidoItem ParaPedidoItem(PedidoItemDTO pedidoItemDTO)
    {
        return new PedidoItem(pedidoItemDTO.ProdutoId, pedidoItemDTO.Nome, pedidoItemDTO.Quantidade,
            pedidoItemDTO.Valor, pedidoItemDTO.Imagem);
    }
}