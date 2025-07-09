using Microsoft.Extensions.Options;
using NSE.Pagamentos.Api.Models;
using NSE.Pagamentos.NerdsPag;

namespace NSE.Pagamentos.Api.Facade;

public class PagamentoCartaoCreditoFacade(
    IOptions<PagamentoConfig> pagamentoConfig
    ) : IPagamentoFacade
{
    private readonly PagamentoConfig _pagamentoConfig = pagamentoConfig.Value;
    
    public async Task<Transacao> AutorizarPagamento(Pagamento pagamento)
    {
        var nerdsPagSvc = new NerdsPagService(_pagamentoConfig.DefaultApiKey, _pagamentoConfig.DefaultEncryptionKey);

        var cardHashGen = new CardHash(nerdsPagSvc)
        {
            CardNumber = pagamento.CartaoCredito.NumeroCartao,
            CardHolderName = pagamento.CartaoCredito.NomeCartao,
            CardExpirationDate = pagamento.CartaoCredito.MesAnoVencimento,
            CardCvv = pagamento.CartaoCredito.CVV
        };

        var cardHash = cardHashGen.Generate();
        
        var transacao = new Transaction(nerdsPagSvc)
        {
            CardHash = cardHash,
            CardNumber = pagamento.CartaoCredito.NumeroCartao,
            CardHolderName = pagamento.CartaoCredito.NomeCartao,
            CardExpirationDate = pagamento.CartaoCredito.MesAnoVencimento,
            CardCvv = pagamento.CartaoCredito.CVV,
            PaymentMethod = PaymentMethod.CreditCard,
            Amount = pagamento.Valor
        };

        return ParaTransacao(await transacao.AuthorizeCardTransaction());
    }

    public Task<Transacao> CapturarPagamento(Transacao transacao)
    {
        throw new NotImplementedException();
    }

    public Task<Transacao> CancelarAutorizacao(Transacao transacao)
    {
        throw new NotImplementedException();
    }
    
    public static Transacao ParaTransacao(Transaction transaction)
    {
        return new Transacao
        {
            Status = (StatusTransacao)transaction.Status,
            ValorTotal = transaction.Amount,
            BandeiraCartao = transaction.CardBrand,
            CodigoAutorizacao = transaction.AuthorizationCode,
            CustoTransacao = transaction.Cost,
            DataTransacao = transaction.TransactionDate,
            NSU = transaction.Nsu,
            TID = transaction.Tid
        };
    }
}