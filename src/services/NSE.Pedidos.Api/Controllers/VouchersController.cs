using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.Pedidos.Api.Application.Queries;
using NSE.WebApi.Core.Controller;

namespace NSE.Pedidos.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VouchersController(IVoucherQueries queries) : MainController
{
    [HttpGet("{codigo}")]
    public async Task<IActionResult> ObterPorCodigo([FromRoute] string codigo)
    {
        if (string.IsNullOrEmpty(codigo)) return NotFound();

        var voucher = await queries.ObterVoucherPorCodigo(codigo);
        
        return voucher is null ? NotFound() : Ok(voucher);
    }
}