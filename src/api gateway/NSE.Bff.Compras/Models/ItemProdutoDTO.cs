namespace NSE.Bff.Compras.Models;

public class ItemProdutoDTO
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public decimal Valor { get; set; }
    public string Imagem { get; set; }
    public int QuantidadeEstoque { get; set; }
}