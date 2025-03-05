using System.ComponentModel.DataAnnotations;
using NSE.Core.Validation;

namespace NSE.Bff.Compras.Models;

public class PedidoDTO
{
    public int Codigo { get; set; }
    public int Status { get; set; }
    public DateTime Data { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal Desconto { get; set; }
    public string VoucherCodigo { get; set; }
    public bool VoucherUtilizado { get; set; }
    public List<ItemCarrinhoDTO> PedidoItems { get; set; }
    public EnderecoDTO Endereco { get; set; }
    
    [Required(ErrorMessage = "Informe o número do cartão")]
    public string NumeroCartao { get; set; }

    [Required(ErrorMessage = "Informe o nome do portador do cartão")]
    public string NomeCartao { get; set; }

    [RegularExpression(@"(0[1-9]|1[0-2])\/[0-9]{2}", ErrorMessage = "O vencimento deve estar no padrão MM/AA")]
    [CartaoExpiracao(ErrorMessage = "Cartão Expirado")]
    [Required(ErrorMessage = "Informe o vencimento")]
    public string ExpiracaoCartao { get; set; }

    [Required(ErrorMessage = "Informe o código de segurança")]
    public string CvvCartao { get; set; }

    
    public void PopularDadosPedido(CarrinhoDTO carrinho, EnderecoDTO endereco)
    {
        VoucherCodigo = carrinho.Voucher.Codigo;
        VoucherUtilizado = carrinho.VoucherUtilizado;
        ValorTotal = carrinho.ValorTotal;
        Desconto = carrinho.Desconto;
        PedidoItems = carrinho.Itens;
        Endereco = endereco;
    }
}