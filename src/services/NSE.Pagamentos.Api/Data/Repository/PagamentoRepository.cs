using Microsoft.EntityFrameworkCore;
using NSE.Core.Data;
using NSE.Pagamentos.Api.Models;

namespace NSE.Pagamentos.Api.Data.Repository;

public class PagamentoRepository(PagamentoContext context) : IPagamentoRepository
{
    public IUnitOfWork UnitOfWork => context;
    
    public void AdicionarPagamento(Pagamento pagamento)
    {
        context.Pagamentos.Add(pagamento);
    }

    public void AdicionarTransacao(Transacao transacao)
    {
        context.Transacoes.Add(transacao);
    }

    public async Task<Pagamento> ObterPagamentoPorPedidoId(int pedidoId)
    {
        return await context.Pagamentos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);
    }

    public async Task<IEnumerable<Transacao>> ObterTransacaoesPorPedidoId(int pedidoId)
    {
        return await context.Transacoes
            .AsNoTracking()
            .Where(t => t.Pagamento.PedidoId == pedidoId)
            .ToListAsync();
    }

    public void Dispose()
    {
        context.Dispose();
    }
}