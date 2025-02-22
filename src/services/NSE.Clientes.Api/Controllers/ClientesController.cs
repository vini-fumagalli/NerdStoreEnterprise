using Microsoft.AspNetCore.Mvc;
using NSE.Clientes.Api.Application.Commands;
using NSE.Core.Mediator;
using NSE.WebApi.Core.Controller;

namespace NSE.Clientes.Api.Controllers;

[Route("api/[controller]")]
public class ClientesController(IMediatorHandler mediatorHandler) : MainController
{
    // [HttpGet]
    // public async Task<IActionResult> Get()
    // {
    //     var resultado = await mediatorHandler.EnviarComando(
    //         new RegistrarClienteCommand(1, "Fumas", "vinifumagalli_@hotmail.com", "43460459875"));
    //     
    //     return CustomResponse(resultado);
    // }
}