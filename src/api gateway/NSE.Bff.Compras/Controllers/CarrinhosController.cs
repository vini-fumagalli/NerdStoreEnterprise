using Microsoft.AspNetCore.Mvc;
using NSE.Bff.Compras.Models;
using NSE.Bff.Compras.Services.Interfaces;
using NSE.WebApi.Core.Controller;
using Polly.Caching;

namespace NSE.Bff.Compras.Controllers;

[Route("api/compras/[controller]")]
public class CarrinhosController(
    ICatalogoService catalogoService,
    ICarrinhoService carrinhoService
    ) : MainController
{
    [HttpGet]
    public async Task<IActionResult> ObterCarrinho()
    {
        return CustomResponse(await carrinhoService.ObterCarrinho());
    }

    [HttpGet("quantidade")]
    public async Task<IActionResult> ObterQuantidadeCarrinho()
    {
        var quantidade = await carrinhoService.ObterCarrinho();
        return CustomResponse(quantidade?.Itens.Sum(i => i.Quantidade) ?? 0);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AdicionarItemCarrinho([FromBody] ItemCarrinhoDTO itemCarrinho)
    {
        var produto = await catalogoService.ObterPorId(itemCarrinho.ProdutoId);
        await ValidarItemCarrinho(produto, itemCarrinho.Quantidade, adicionarProduto: true);
        
        if (!OperacaoValida()) return CustomResponse();
        
        itemCarrinho.AdicionarCatalogoInfo(produto);
        return CustomResponse(await carrinhoService.AdicionarItemCarrinho(itemCarrinho));
    }

    [HttpPut("items/{produtoId:int}")]
    public async Task<IActionResult> AtualizarItemCarrinho([FromRoute] int produtoId,
        [FromBody] ItemCarrinhoDTO itemCarrinho)
    {
        var produto = await catalogoService.ObterPorId(produtoId);
        await ValidarItemCarrinho(produto, itemCarrinho.Quantidade);

        if (!OperacaoValida()) return CustomResponse();

        return Ok(await carrinhoService.AtualizarItemCarrinho(produtoId, itemCarrinho));
    }

    [HttpDelete("items/{produtoId:int}")]
    public async Task<IActionResult> RemoverItemCarrinho([FromRoute] int produtoId)
    {
        var produto = await catalogoService.ObterPorId(produtoId);

        if (produto != null) return CustomResponse(await carrinhoService.RemoverItemCarrinho(produtoId));
        
        AdicionarErroProcessamento("Produto inexistente!");
        return CustomResponse();
    }
    
    //TODO: ENDPOINT PARA APLICAR VOUCHER
    
    private async Task ValidarItemCarrinho(ItemProdutoDTO produto, int quantidade, bool adicionarProduto = false)
    {
        if (produto is null) AdicionarErroProcessamento("Produto inexistente!");
        
        if (quantidade < 1) AdicionarErroProcessamento($"Escolha ao menos uma unidade do produto {produto.Nome}");

        var carrinho = await carrinhoService.ObterCarrinho();
        var itemCarrinho = carrinho.Itens.FirstOrDefault(p => p.ProdutoId == produto.Id);

        if (itemCarrinho != null && adicionarProduto && itemCarrinho.Quantidade + quantidade > produto.QuantidadeEstoque)
        {
            AdicionarErroProcessamento($"O produto {produto.Nome} possui {produto.QuantidadeEstoque} " +
                                       $"unidades em estoque, você selecionou {quantidade}");
            return;
        }

        if (quantidade > produto.QuantidadeEstoque)
        {
            AdicionarErroProcessamento($"O produto {produto.Nome} possui {produto.QuantidadeEstoque} " +
                                       $"unidades em estoque, você selecionou {quantidade}");
        }
    }
}