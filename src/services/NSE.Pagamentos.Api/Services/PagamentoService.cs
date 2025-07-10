using FluentValidation.Results;
using NSE.Core.DomainObjects;
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

    public async Task<ResponseMessage> CapturarPagamento(int pedidoId)
    {
        var transacoes = await repository.ObterTransacaoesPorPedidoId(pedidoId);
        var transacaoAutorizada = transacoes?.FirstOrDefault(t => t.Status == StatusTransacao.Autorizado);
        var validationResult = new ValidationResult();

        if (transacaoAutorizada == null) throw new DomainException($"Transação não encontrada para o pedido {pedidoId}");

        var transacao =  await facade.CapturarPagamento(transacaoAutorizada);

        if (transacao.Status != StatusTransacao.Pago)
        {
            validationResult.Errors.Add(new ValidationFailure("Pagamento",
                $"Não foi possível capturar o pagamento do pedido {pedidoId}"));

            return new ResponseMessage(validationResult);
        }

        transacao.PagamentoId = transacaoAutorizada.PagamentoId;
        repository.AdicionarTransacao(transacao);

        if (!await repository.UnitOfWork.Commit())
        {
            validationResult.Errors.Add(new ValidationFailure("Pagamento",
                $"Não foi possível persistir a captura do pagamento do pedido {pedidoId}"));
        }

        return new ResponseMessage(validationResult);
    }

    public async Task<ResponseMessage> CancelarPagamento(int pedidoId)
    {
        var transacoes = await repository.ObterTransacaoesPorPedidoId(pedidoId);
        var transacaoAutorizada = transacoes?.FirstOrDefault(t => t.Status == StatusTransacao.Autorizado);
        var validationResult = new ValidationResult();

        if (transacaoAutorizada == null) throw new DomainException($"Transação não encontrada para o pedido {pedidoId}");

        var transacao = await facade.CancelarAutorizacao(transacaoAutorizada);

        if (transacao.Status != StatusTransacao.Cancelado)
        {
            validationResult.Errors.Add(new ValidationFailure("Pagamento",
                $"Não foi possível cancelar o pagamento do pedido {pedidoId}"));

            return new ResponseMessage(validationResult);
        }

        transacao.PagamentoId = transacaoAutorizada.PagamentoId;
        repository.AdicionarTransacao(transacao);

        if (!await repository.UnitOfWork.Commit())
        {
            validationResult.Errors.Add(new ValidationFailure("Pagamento",
                $"Não foi possível persistir o cancelamento do pagamento do pedido {pedidoId}"));
        }

        return new ResponseMessage(validationResult);
    }
}