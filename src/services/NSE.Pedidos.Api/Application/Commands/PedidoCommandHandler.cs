using FluentValidation.Results;
using MediatR;
using NSE.Core.Messages;
using NSE.Core.Messages.Integration;
using NSE.MessageBus;
using NSE.Pedidos.Api.Application.DTO;
using NSE.Pedidos.Api.Application.Events;
using NSE.Pedidos.Domain.Pedidos;
using NSE.Pedidos.Domain.Vouchers;
using NSE.Pedidos.Domain.Vouchers.Validations;

namespace NSE.Pedidos.Api.Application.Commands;

public class PedidoCommandHandler(
    IPedidoRepository pedidoRepository,
    IVoucherRepository voucherRepository,
    IMessageBus bus
    ) : CommandHandler, IRequestHandler<AdicionarPedidoCommand, ValidationResult>
{
    public async Task<ValidationResult> Handle(AdicionarPedidoCommand message, CancellationToken cancellationToken)
    {
        if (!message.EhValido()) return message.ValidationResult;
        
        var pedido = MapearPedido(message);
        
        if (!await AplicarVoucher(message, pedido)) return ValidationResult;
        
        if (!ValidarPedido(pedido)) return ValidationResult;
        
        if (!await ProcessarPagamento(pedido, message)) return ValidationResult;
        
        pedido.AutorizarPedido();
        pedido.AdicionarEvento(new PedidoRealizadoEvent(pedido.Id, pedido.ClienteId));
        pedidoRepository.Adicionar(pedido);
        return await PersitirDados(pedidoRepository.UnitOfWork);
    }
    
    private static Pedido MapearPedido(AdicionarPedidoCommand message)
    {
        var endereco = new Endereco
        {
            Logradouro = message.Endereco.Logradouro,
            Numero = message.Endereco.Numero,
            Complemento = message.Endereco.Complemento,
            Bairro = message.Endereco.Bairro,
            Cep = message.Endereco.Cep,
            Cidade = message.Endereco.Cidade,
            Estado = message.Endereco.Estado
        };

        var pedido = new Pedido(message.ClienteId, message.ValorTotal, 
            message.PedidoItems.Select(PedidoItemDTO.ParaPedidoItem).ToList(), message.VoucherUtilizado, 
            message.Desconto);

        pedido.AtribuirEndereco(endereco);
        return pedido;
    }

    private async Task<bool> AplicarVoucher(AdicionarPedidoCommand message, Pedido pedido)
    {
        if (!message.VoucherUtilizado) return true;

        var voucher = await voucherRepository.ObterVoucherPorCodigo(message.VoucherCodigo);
        if (voucher == null)
        {
            AdicionarErro("O voucher informado não existe!");
            return false;
        }
        
        var voucherValidation = await new VoucherValidation().ValidateAsync(voucher);
        if (!voucherValidation.IsValid)
        {
            voucherValidation.Errors.ToList().ForEach(m => AdicionarErro(m.ErrorMessage));
            return false;
        }

        pedido.AtribuirVoucher(voucher);
        voucher.DebitarQuantidade();

        voucherRepository.Atualizar(voucher);

        return true;
    }

    private bool ValidarPedido(Pedido pedido)
    {
        var pedidoValorOriginal = pedido.ValorTotal;
        var pedidoDesconto = pedido.Desconto;

        pedido.CalcularValorPedido();

        if (pedido.ValorTotal != pedidoValorOriginal)
        {
            AdicionarErro("O valor total do pedido não confere com o cálculo do pedido");
            return false;
        }

        if (pedido.Desconto != pedidoDesconto)
        {
            AdicionarErro("O valor total não confere com o cálculo do pedido");
            return false;
        }

        return true;
    }

    public async Task<bool> ProcessarPagamento(Pedido pedido, AdicionarPedidoCommand message)
    {
        var pedidoIniciado = new PedidoIniciadoIntegrationEvent
        {
            PedidoId = pedido.Id,
            ClienteId = pedido.ClienteId,
            Valor = pedido.ValorTotal,
            TipoPagamento = 1,
            NomeCartao = message.NomeCartao,
            NumeroCartao = message.NumeroCartao,
            MesAnoVencimento = message.ExpiracaoCartao,
            CVV = message.CvvCartao
        };

        var result = await bus.RequestAsync<PedidoIniciadoIntegrationEvent, ResponseMessage>(pedidoIniciado);

        if (result.ValidationResult.IsValid) return true;

        foreach (var erro in result.ValidationResult.Errors)
        {
            AdicionarErro(erro.ErrorMessage);
        }
        
        return false;
    }
}