using NSE.Pedidos.Api.Application.DTO;

namespace NSE.Pedidos.Api.Application.Queries;

public interface IVoucherQueries
{
    Task<VoucherDTO> ObterVoucherPorCodigo(string codigo);
}