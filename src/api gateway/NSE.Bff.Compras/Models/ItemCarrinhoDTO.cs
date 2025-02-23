namespace NSE.Bff.Compras.Models;

public class ItemCarrinhoDTO
{
    public int ProdutoId { get; set; }
    public string Nome { get; set; }
    public decimal Valor { get; set; }
    public string Imagem { get; set; }
    public int Quantidade { get; set; }

    public void AdicionarCatalogoInfo(ItemProdutoDTO produtoDto)
    {
        Nome = produtoDto.Nome;
        Valor = produtoDto.Valor;
        Imagem = produtoDto.Imagem;
    }
}