using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.Core.Mediator;
using NSE.Pedidos.Api.Application.Commands;
using NSE.Pedidos.Api.Application.Queries;
using NSE.WebApi.Core.Controller;
using NSE.WebApi.Core.Usuario;

namespace NSE.Pedidos.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PedidosController(
    IMediatorHandler mediatorHandler,
    IAspNetUser user,
    IPedidoQueries queries
    ) : MainController
{
    [HttpPost]
    public async Task<IActionResult> AdicionarPedido([FromBody] AdicionarPedidoCommand command)
    {
        command.SetClienteId(user.ObterUserId());
        return CustomResponse(await mediatorHandler.EnviarComando(command));
    }

    [HttpGet("ultimo")]
    public async Task<IActionResult> UltimoPedido()
    {
        var pedido = await queries.ObterUltimoPedido(user.ObterUserId());
        return pedido is null ? NotFound() : CustomResponse(pedido);
    }

    [HttpGet("lista-cliente")]
    public async Task<IActionResult> ListaPorCliente()
    {
        var pedidos = await queries.ObterListaPorClienteId(user.ObterUserId());
        return pedidos is null ? NotFound() : CustomResponse(pedidos);
    }
}