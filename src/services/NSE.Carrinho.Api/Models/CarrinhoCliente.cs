using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.Results;
using NSE.Core.DomainObjects;

namespace NSE.Carrinho.Api.Models;

public class CarrinhoCliente : Entity
{
    public int ClienteId { get; set; }
    public decimal ValorTotal { get; set; }
    public List<CarrinhoItem> Itens { get; set; } = [];
    public bool VoucherUtilizado { get; set; }
    public decimal Desconto { get; set; }

    public Voucher Voucher { get; set; }
    [JsonIgnore] public ValidationResult ValidationResult { get; set; }
    
    public CarrinhoCliente() { }

    public CarrinhoCliente(int clienteId)
    {
        ClienteId = clienteId;
    }

    public void AplicarVoucher(Voucher voucher)
    {
        Voucher = voucher;
        VoucherUtilizado = true;
        CalcularValorCarrinho();
    }
    
    private void CalcularValorCarrinho()
    {
        ValorTotal = Itens.Sum(p => p.CalcularValor());
        CalcularValorTotalDesconto();
    }

    private void CalcularValorTotalDesconto()
    {
        if (!VoucherUtilizado) return;

        decimal desconto = 0;
        var valor = ValorTotal;

        if (Voucher.TipoDesconto == TipoDescontoVoucher.Porcentagem)
        {
            if (Voucher.Percentual.HasValue)
            {
                desconto = (valor * Voucher.Percentual.Value) / 100;
                valor -= desconto;
            }
        }
        else
        {
            if (Voucher.ValorDesconto.HasValue)
            {
                desconto = Voucher.ValorDesconto.Value;
                valor -= desconto;
            }
        }

        ValorTotal = valor < 0 ? 0 : valor;
        Desconto = desconto;
    }

    public bool CarrinhoItemExistente(CarrinhoItem item)
    {
        return Itens.Any(p => p.ProdutoId == item.ProdutoId);
    }

    private CarrinhoItem ObterPorProdutoId(int produtoId)
    {
        return Itens.FirstOrDefault(p => p.ProdutoId == produtoId);
    }

    public void AdicionarOuAtualizarItem(CarrinhoItem item)
    {
        item.AssociarCarrinho(Id);

        if (CarrinhoItemExistente(item))
        {
            var itemExistente = ObterPorProdutoId(item.ProdutoId);
            itemExistente.AdicionarUnidades(item.Quantidade);

            item = itemExistente;
            Itens.Remove(itemExistente);
        }

        Itens.Add(item);
        CalcularValorCarrinho();
    }

    private void AtualizarItem(CarrinhoItem item)
    {
        item.AssociarCarrinho(Id);

        var itemExistente = ObterPorProdutoId(item.ProdutoId);

        Itens.Remove(itemExistente);
        Itens.Add(item);

        CalcularValorCarrinho();
    }

    public void AtualizarUnidades(CarrinhoItem item, int unidades)
    {
        item.AtualizarUnidades(unidades);
        AtualizarItem(item);
    }

    public void RemoverItem(CarrinhoItem item)
    {
        Itens.Remove(ObterPorProdutoId(item.ProdutoId));
        CalcularValorCarrinho();
    }

    public bool EhValido()
    {
        var erros = Itens.SelectMany(i => new CarrinhoItem.ItemCarrinhoValidation().Validate(i).Errors).ToList();
        erros.AddRange(new CarrinhoClienteValidation().Validate(this).Errors);
        ValidationResult = new ValidationResult(erros);
        
        return ValidationResult.IsValid;
    }
    
    public class CarrinhoClienteValidation : AbstractValidator<CarrinhoCliente>
    {
        public CarrinhoClienteValidation()
        {
            RuleFor(c => c.ClienteId)
                .NotEqual(0)
                .WithMessage("Cliente não reconhecido");

            RuleFor(c => c.Itens.Count)
                .GreaterThan(0)
                .WithMessage("O carrinho não possui itens");

            RuleFor(c => c.ValorTotal)
                .GreaterThan(0)
                .WithMessage("O valor total do carrinho precisa ser maior que 0");
        }
    }
}