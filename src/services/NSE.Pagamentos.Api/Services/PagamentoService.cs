using FluentValidation.Results;
using NSE.Core.Messages.Integration;
using NSE.Pagamentos.Api.Facade;
using NSE.Pagamentos.Api.Models;

namespace NSE.Pagamentos.Api.Services;

public class PagamentoService(
    IPagamentoFacade facade,
    IPagamentoRepository repository
    ) : IPagamentoService
{
    public async Task<ResponseMessage> AutorizarPagamento(Pagamento pagamento)
    {
        var transacao = await facade.AutorizarPagamento(pagamento);
        var validationResult = new ValidationResult();

        if (transacao.Status != StatusTransacao.Autorizado)
        {
            validationResult.Errors.Add(new ValidationFailure("Pagamento", 
                "Pagamento recusado, entre em contato com a operadora de cartão"));

            return new ResponseMessage(validationResult);
        }
        
        pagamento.AdicionarTransacao(transacao);
        repository.AdicionarPagamento(pagamento);

        if (!await repository.UnitOfWork.Commit())
        {
            validationResult.Errors.Add(new ValidationFailure("Pagamento",
                "Houve um erro ao realizar pagamento."));
        }

        return new ResponseMessage(validationResult);
    }
}