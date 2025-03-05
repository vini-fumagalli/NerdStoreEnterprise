using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.Bff.Compras.Models;
using NSE.Bff.Compras.Services.Interfaces;
using NSE.WebApi.Core.Controller;

namespace NSE.Bff.Compras.Controllers;

[Authorize]
[ApiController]
[Route("api/compras/[controller]")]
public class PedidosController(
    ICatalogoService catalogoService,
    ICarrinhoService carrinhoService,
    IPedidoService pedidoService,
    IClienteService clienteService
    ) : MainController
{
     [HttpPost]
     public async Task<IActionResult> AdicionarPedido(PedidoDTO pedido)
     {
         var carrinho = await carrinhoService.ObterCarrinho();
         var produtos = await catalogoService.ObterItens(carrinho.Itens.Select(p => p.ProdutoId));
         var endereco = await clienteService.ObterEndereco();

         if (!await ValidarCarrinhoProdutos(carrinho, produtos)) return CustomResponse();

         pedido.PopularDadosPedido(carrinho, endereco);
         return CustomResponse(await pedidoService.FinalizarPedido(pedido));
     }
     
     [HttpGet("ultimo")]
     public async Task<IActionResult> UltimoPedido()
     {
         var pedido = await pedidoService.ObterUltimoPedido();
         if (pedido is null)
         {
             AdicionarErroProcessamento("Pedido não encontrado!");
             return CustomResponse();
         }

         return CustomResponse(pedido);
     }

     [HttpGet("lista-cliente")]
     public async Task<IActionResult> ListaPorCliente()
     {
         var pedidos = await pedidoService.ObterListaPorClienteId();
         return pedidos == null ? NotFound() : CustomResponse(pedidos);
     }
     
     private async Task<bool> ValidarCarrinhoProdutos(CarrinhoDTO carrinho, IEnumerable<ItemProdutoDTO> produtos)
    {
        if (carrinho.Itens.Count != produtos.Count())
        {
            var itensIndisponiveis = carrinho.Itens.Select(c => c.ProdutoId).Except(produtos.Select(p => p.Id)).ToList();

            foreach (var itemId in itensIndisponiveis)
            {
                var itemCarrinho = carrinho.Itens.FirstOrDefault(c => c.ProdutoId == itemId);
                AdicionarErroProcessamento($"O item {itemCarrinho.Nome} não está mais disponível no catálogo, o remova do carrinho para prosseguir com a compra");
            }

            return false;
        }

        foreach (var itemCarrinho in carrinho.Itens)
        {
            var produtoCatalogo = produtos.FirstOrDefault(p => p.Id == itemCarrinho.ProdutoId);

            if (produtoCatalogo.Valor != itemCarrinho.Valor)
            {
                var msgErro = $"O produto {itemCarrinho.Nome} mudou de valor (de: " +
                              $"{string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", itemCarrinho.Valor)} para: " +
                              $"{string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", produtoCatalogo.Valor)}) desde que foi adicionado ao carrinho.";

                AdicionarErroProcessamento(msgErro);

                var responseRemover = await carrinhoService.RemoverItemCarrinho(itemCarrinho.ProdutoId);
                if (ResponsePossuiErros(responseRemover))
                {
                    AdicionarErroProcessamento($"Não foi possível remover automaticamente o produto {itemCarrinho.Nome} do seu carrinho, _" +
                                               "remova e adicione novamente caso ainda deseje comprar este item");
                    return false;
                }

                itemCarrinho.Valor = produtoCatalogo.Valor;
                var responseAdicionar = await carrinhoService.AdicionarItemCarrinho(itemCarrinho);

                if (ResponsePossuiErros(responseAdicionar))
                {
                    AdicionarErroProcessamento($"Não foi possível atualizar automaticamente o produto {itemCarrinho.Nome} do seu carrinho, _" +
                                               "adicione novamente caso ainda deseje comprar este item");
                    return false;
                }

                LimparErrosProcessamento();
                AdicionarErroProcessamento(msgErro + " Atualizamos o valor em seu carrinho, realize a conferência do pedido e se preferir remova o produto");

                return false;
            }
        }

        return true;
    }
}