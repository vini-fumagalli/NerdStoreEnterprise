using Microsoft.AspNetCore.Mvc;
using NSE.Clientes.Api.Application.Commands;
using NSE.Clientes.Api.Models;
using NSE.Core.Mediator;
using NSE.WebApi.Core.Controller;
using NSE.WebApi.Core.Usuario;

namespace NSE.Clientes.Api.Controllers;

[Route("api/[controller]")]
public class ClientesController(
    IMediatorHandler mediatorHandler,
    IClienteRepository repository,
    IAspNetUser user
    ) : MainController
{
    [HttpGet("endereco")]
    public async Task<IActionResult> ObterEndereco()
    {
        var endereco = await repository.ObterEnderecoPorClienteId(user.ObterUserId());
        return endereco is null ? NotFound() : CustomResponse(endereco);
    }
    
    [HttpPost("endereco")]
    public async Task<IActionResult> AdicionarEndereco([FromBody] AdicionarEnderecoCommand command)
    {
        command.SetClienteId(user.ObterUserId());
        return CustomResponse(await mediatorHandler.EnviarComando(command));
    }
}