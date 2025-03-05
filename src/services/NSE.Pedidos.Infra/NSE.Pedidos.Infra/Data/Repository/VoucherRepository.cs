using Microsoft.EntityFrameworkCore;
using NSE.Core.Data;
using NSE.Pedidos.Domain.Vouchers;

namespace NSE.Pedidos.Infra.Data.Repository;

public class VoucherRepository(PedidosContext context) : IVoucherRepository
{
    public IUnitOfWork UnitOfWork => context;
    
    public async Task<Voucher> ObterVoucherPorCodigo(string codigo)
    {
        return await context.Vouchers.FirstOrDefaultAsync(v => v.Codigo == codigo); 
    }

    public void Atualizar(Voucher voucher)
    {
        context.Vouchers.Update(voucher);
    }
    
    public void Dispose()
    {
        context?.Dispose();
    }
}