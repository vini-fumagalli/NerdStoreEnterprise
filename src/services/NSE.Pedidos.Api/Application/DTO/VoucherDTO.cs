using NSE.Pedidos.Domain.Vouchers;

namespace NSE.Pedidos.Api.Application.DTO;

public class VoucherDTO
{
    public string Codigo { get; set; }
    public decimal? Percentual { get; set; }
    public decimal? ValorDesconto { get; set; }
    public int TipoDesconto { get; set; }

    public VoucherDTO(Voucher voucher)
    {
        Codigo = voucher.Codigo;
        TipoDesconto = (int)voucher.TipoDesconto;
        Percentual = voucher.Percentual;
        ValorDesconto = voucher.ValorDesconto;
    }
}