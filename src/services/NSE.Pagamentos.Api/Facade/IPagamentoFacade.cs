using NSE.Pagamentos.Api.Models;

namespace NSE.Pagamentos.Api.Facade;

public interface IPagamentoFacade
{
    Task<Transacao> AutorizarPagamento(Pagamento pagamento);
    Task<Transacao> CapturarPagamento(Transacao transacao);
    Task<Transacao> CancelarAutorizacao(Transacao transacao);
}