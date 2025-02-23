using System.Text.Json.Serialization;
using FluentValidation;
using NSE.Core.DomainObjects;

namespace NSE.Carrinho.Api.Models;

public class CarrinhoItem : Entity
{
    public int ProdutoId { get; set; }
    public string Nome { get; set; }
    public int Quantidade { get; set; }
    public decimal Valor { get; set; }
    public string Imagem { get; set; }
    public int CarrinhoId { get; set; }
    [JsonIgnore] public CarrinhoCliente CarrinhoCliente { get; set; }
    
    public CarrinhoItem() { }
    
    
    public void AssociarCarrinho(int carrinhoId) => CarrinhoId = carrinhoId;

    public decimal CalcularValor() => Quantidade * Valor;

    public void AdicionarUnidades(int unidades) => Quantidade += unidades;

    public void AtualizarUnidades(int unidades) => Quantidade = unidades;

    public bool EhValido() => new ItemCarrinhoValidation().Validate(this).IsValid;
    
    
    public class ItemCarrinhoValidation : AbstractValidator<CarrinhoItem>
    {
        public ItemCarrinhoValidation()
        {
            RuleFor(c => c.Nome)
                .NotEmpty()
                .WithMessage("O nome do produto não foi informado");

            RuleFor(c => c.Quantidade)
                .GreaterThan(0)
                .WithMessage(item => $"A quantidade miníma para o {item.Nome} é 1");

            RuleFor(c => c.Quantidade)
                .LessThanOrEqualTo(5)
                .WithMessage(item => $"A quantidade máxima do {item.Nome} é 5");

            RuleFor(c => c.Valor)
                .GreaterThan(0)
                .WithMessage(item => $"O valor do {item.Nome} precisa ser maior que 0");
        }
    }
}