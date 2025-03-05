using NSE.Pedidos.Api.Application.DTO;
using NSE.Pedidos.Domain.Vouchers;

namespace NSE.Pedidos.Api.Application.Queries;

public class VoucherQueries(IVoucherRepository repository) : IVoucherQueries
{
    public async Task<VoucherDTO> ObterVoucherPorCodigo(string codigo)
    {
        var voucher = await repository.ObterVoucherPorCodigo(codigo);
        
        if (voucher is null) return null;
        
        if (!voucher.EstaValidoParaUtilizacao()) return null;

        return new VoucherDTO(voucher);
    }
}