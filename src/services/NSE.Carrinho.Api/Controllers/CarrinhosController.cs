using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSE.Carrinho.Api.Data;
using NSE.Carrinho.Api.Models;
using NSE.Core.Data;
using NSE.WebApi.Core.Controller;
using NSE.WebApi.Core.Usuario;

namespace NSE.Carrinho.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
public class CarrinhosController(IAspNetUser user, CarrinhoContext context) : MainController
{
    private IUnitOfWork _unitOfWork => context;
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return CustomResponse(await ObterCarrinhoCliente() ?? new CarrinhoCliente());
    }

    //Serve para criar um carrinho, adicionar mais itens dentro desse carrinho ou 
    //adicionar mais quantidades de um item ja existente no carrinho
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CarrinhoItem item)
    {
        var carrinho = await ObterCarrinhoCliente();
        ManipularCarrinho(carrinho, item);

        if (!OperacaoValida()) return CustomResponse();

        await Commit();
        return CustomResponse();
    }

    //Serve somente para atualizar a qtd de um item específico dentro do carrinho q já existe
    [HttpPut("{produtoId:int}")]
    public async Task<IActionResult> Update([FromRoute] int produtoId, [FromBody] CarrinhoItem item)
    {
        var carrinho = await ObterCarrinhoCliente();
        var itemCarrinho = await ObterItemCarrinhoValidado(produtoId, carrinho, item);
        if (itemCarrinho == null) return CustomResponse();

        carrinho.AtualizarUnidades(itemCarrinho, item.Quantidade);
        ValidarCarrinho(carrinho);
        
        if (!OperacaoValida()) return CustomResponse();
        
        context.CarrinhoCliente.Update(carrinho);
        await Commit();
        return CustomResponse();
    }

    [HttpDelete("{produtoId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int produtoId)
    {
        var carrinho = await ObterCarrinhoCliente();

        var itemCarrinho = await ObterItemCarrinhoValidado(produtoId, carrinho);
        if (itemCarrinho == null) return CustomResponse();

        ValidarCarrinho(carrinho);
        if (!OperacaoValida()) return CustomResponse();

        carrinho.RemoverItem(itemCarrinho);
        
        context.CarrinhoItens.Remove(itemCarrinho);
        await Commit();
        return CustomResponse();
    }

    private async Task<CarrinhoCliente?> ObterCarrinhoCliente()
    {
        return await context.CarrinhoCliente
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(c => c.ClienteId == user.ObterUserId());
    }

    private void ManipularCarrinho(CarrinhoCliente carrinho, CarrinhoItem item)
    {
        if (carrinho is null)
        {
            ManipularNovoCarrinho(item);
            return;
        }
        
        ManipularCarrinhoExistente(carrinho, item);
    }
    
    private void ManipularNovoCarrinho(CarrinhoItem item)
    {
        var carrinho = new CarrinhoCliente(user.ObterUserId());
        carrinho.AdicionarOuAtualizarItem(item);

        if (!ValidarCarrinho(carrinho)) return;
        
        context.CarrinhoCliente.Add(carrinho);
    }
    
    private void ManipularCarrinhoExistente(CarrinhoCliente carrinho, CarrinhoItem item)
    {
        carrinho.AdicionarOuAtualizarItem(item);
        
        if (!ValidarCarrinho(carrinho)) return;

        context.CarrinhoCliente.Update(carrinho);
    }
    
    private async Task<CarrinhoItem> ObterItemCarrinhoValidado(int produtoId, CarrinhoCliente carrinho, CarrinhoItem item = null)
    {
        if (item != null && produtoId != item.ProdutoId)
        {
            AdicionarErroProcessamento("O item não corresponde ao informado");
            return null;
        }

        if (carrinho == null)
        {
            AdicionarErroProcessamento("Carrinho não encontrado");
            return null;
        }

        var itemCarrinho = await context.CarrinhoItens
            .FirstOrDefaultAsync(i => i.CarrinhoId == carrinho.Id && i.ProdutoId == produtoId);

        if (itemCarrinho == null || !carrinho.CarrinhoItemExistente(itemCarrinho))
        {
            AdicionarErroProcessamento("O item não está no carrinho");
            return null;
        }

        return itemCarrinho;
    }
    
    private async Task Commit()
    {
        if (await _unitOfWork.Commit()) return;
        
        AdicionarErroProcessamento("Não foi possível persistir os dados no banco");
    }
    
    private bool ValidarCarrinho(CarrinhoCliente carrinho)
    {
        if (carrinho.EhValido()) return true;

        carrinho.ValidationResult?.Errors?.ToList().ForEach(e => AdicionarErroProcessamento(e.ErrorMessage));
        return false;
    }
}