using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.Catalogo.Api.Models;
using NSE.WebApi.Core.Controller;
using NSE.WebApi.Core.Identidade;

namespace NSE.Catalogo.Api.Controllers;

[Route("api/catalogo")]
[Authorize]
public class CatalogoController(IProdutoRepository repository) : MainController
{
    [AllowAnonymous]
    [HttpGet("produtos")]
    public async Task<IEnumerable<Produto>> Get()
    {
        return await repository.ObterTodos();
    }
    
    [ClaimsAuthorize(Acesso.Catalogo, [Permissao.Ler])]
    [HttpGet("produtos/{id:int}")]
    public async Task<Produto> GetById(int id)
    {
        return await repository.ObterPorId(id);
    }
}