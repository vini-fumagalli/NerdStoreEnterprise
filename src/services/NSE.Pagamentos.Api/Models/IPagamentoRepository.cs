using NSE.Core.Data;

namespace NSE.Pagamentos.Api.Models;

public interface IPagamentoRepository : IRepository<Pagamento>
{
    void AdicionarPagamento(Pagamento pagamento);
    void AdicionarTransacao(Transacao transacao);
    Task<Pagamento> ObterPagamentoPorPedidoId(int pedidoId);
    Task<IEnumerable<Transacao>> ObterTransacaoesPorPedidoId(int pedidoId);
}