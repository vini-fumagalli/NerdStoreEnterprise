using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSE.Identidade.Api.Models;

namespace NSE.Identidade.Api.Controllers;

[ApiController]
[Route("api/identidade")]
public class AuthController(
    SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> usereManager) : ControllerBase
{
    [HttpPost("nova-conta")]
    public async Task<ActionResult> Registrar(UsuarioRegistro usuario)
    {
        if (!ModelState.IsValid) return BadRequest();

        var user = new IdentityUser
        {
            UserName = usuario.Email,
            Email = usuario.Email,
            EmailConfirmed = true
        };

        var result = await usereManager.CreateAsync(user, usuario.Senha);

        if (result.Succeeded)
        {
            await signInManager.SignInAsync(user, false);
            return Ok();
        }

        return BadRequest();
    }

    [HttpPost("autenticar")]
    public async Task<ActionResult> Login(UsuarioLogin usuario)
    {
        if (!ModelState.IsValid) return BadRequest();

        var result = await signInManager.PasswordSignInAsync(usuario.Email, usuario.Senha, false, false);

        if (result.Succeeded) return Ok();

        return BadRequest();
    }
}