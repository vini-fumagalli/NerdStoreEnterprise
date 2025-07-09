using NSE.Core.DomainObjects;

namespace NSE.Pagamentos.Api.Models;

public class Transacao : Entity
{
    public string CodigoAutorizacao { get; set; }
    public string BandeiraCartao { get; set; }
    public DateTime? DataTransacao { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal CustoTransacao { get; set; }
    public StatusTransacao Status { get; set; }
    public string TID { get; set; } 
    public string NSU { get; set; } 
    public int PagamentoId { get; set; }
    public Pagamento Pagamento { get; set; }
}