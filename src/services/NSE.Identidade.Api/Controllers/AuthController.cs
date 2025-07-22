using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Core.Interfaces;
using NSE.Core.Messages.Integration;
using NSE.Identidade.Api.Data.Interfaces;
using NSE.Identidade.Api.Models;
using NSE.Identidade.Api.Utils;
using NSE.MessageBus;
using NSE.WebApi.Core.Controller;
using NSE.WebApi.Core.Identidade;
using NSE.WebApi.Core.Usuario;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace NSE.Identidade.Api.Controllers;

[Route("api/identidade")]
public class AuthController(
    SignInManager<IdentityUser<int>> signInManager,
    UserManager<IdentityUser<int>> userManager,
    ICodAutRepository codAutRepository,
    IRefreshTokensRepository refreshTokensRepository,
    IMessageBus bus,
    IJwtService jwtService,
    IOptions<TokenSettings> tokenSettings
    ) : MainController
{
    private readonly TokenSettings _tokenSettings = tokenSettings.Value;
    
    [HttpPost("nova-conta")]
    public async Task<ActionResult> Registrar([FromBody] UsuarioRegistro usuario)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);
        
        var user = await userManager.FindByEmailAsync(usuario.Email);
        if (user is not null) 
        {
            AdicionarErroProcessamento("Esse email já está sendo utilizado");
            return CustomResponse();
        }

        try
        {
            await codAutRepository.GerarCodigoEnviarEmail(usuario.Email);
            return CustomResponse();
        }
        catch (Exception e)
        {
            AdicionarErroProcessamento(e.Message);
            return CustomResponse();
        }
    }

    [HttpPost("cod-aut")]
    public async Task<ActionResult> CodAut([FromBody] CodAutEntry codAutEntry)
    {
        if (!await codAutRepository.Validar(codAutEntry.Email, codAutEntry.CodigoAutenticacao))
        {
            AdicionarErroProcessamento("Código inválido");
            return CustomResponse();
        }
        
        var user = new IdentityUser<int>
        {
            UserName = codAutEntry.Email,
            Email = codAutEntry.Email,
            EmailConfirmed = true
        };
        
        var result = await userManager.CreateAsync(user, codAutEntry.Senha);
        if (!result.Succeeded)
        {
            result.Errors.ToList().ForEach(e => AdicionarErroProcessamento(e.Description));
            return CustomResponse();
        }
        
        var clienteResult = await RegistrarCliente(codAutEntry);

        if (!clienteResult.ValidationResult.IsValid)
        {
            await userManager.DeleteAsync(user);
            return CustomResponse(clienteResult.ValidationResult);
        }
            
        await userManager.AddClaimAsync(user, new Claim(nameof(Acesso.Catalogo), nameof(Permissao.Ler)));
        await signInManager.SignInAsync(user, false);
        return CustomResponse(await GerarJwt(user.Email));
    }

    [HttpPost("autenticar")]
    public async Task<ActionResult> Login([FromBody] UsuarioLogin usuario)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var result = await signInManager.PasswordSignInAsync(usuario.Email, usuario.Senha, false, true);

        if (result.Succeeded) return CustomResponse(await GerarJwt(usuario.Email));

        if (result.IsLockedOut)
        {
            AdicionarErroProcessamento("Usuário bloqueado por tentativas inválidas");
            return CustomResponse();
        }

        AdicionarErroProcessamento("Usuário ou senha incorretos");
        return CustomResponse();
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult> RenovarToken([FromBody] TokenEntry tokenEntry)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);
        
        var principal = await ObterClaimsTokenExpirado(tokenEntry.Token);
        if (principal is null)
        {
            AdicionarErroProcessamento("Token ou refresh token inválido");
            return CustomResponse();
        }
        
        var usuarioId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!await refreshTokensRepository.Validar(int.Parse(usuarioId), tokenEntry.RefreshToken))
        {
            AdicionarErroProcessamento("Token ou refresh token inválido");
            return CustomResponse();
        }

        var user = await userManager.FindByIdAsync(usuarioId);
        return CustomResponse(await GerarJwt(user.Email));
    }

    private async Task<UsuarioRespostaLogin> GerarJwt(string email)
    {
        var usuario = await userManager.FindByEmailAsync(email);
        var claims = await userManager.GetClaimsAsync(usuario);
        AdicionarClaims(usuario, claims);
        var token = await ObterToken(new ClaimsIdentity(claims));

        return new UsuarioRespostaLogin
        {
            Token = token,
            Email = usuario.Email,
            ExpiraEm = TimeSpan.FromHours(1).TotalSeconds,
            RefreshToken = await GerarRefreshToken(usuario.Id)
        };
    }
    
    private async Task<ClaimsPrincipal?> ObterClaimsTokenExpirado(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = await jwtService.GetCurrentSecurityKey(),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        ClaimsPrincipal principal;
        try
        {
            principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        }   
        catch 
        {
            return null;
        }
        
        return principal.Identity.IsAuthenticated ? principal : null;
    }

    private async Task<string> GerarRefreshToken(int usuarioId)
    {
        var randomNumber = new byte[32];
        var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var refreshToken = Convert.ToBase64String(randomNumber);
        await refreshTokensRepository.CreateOrUpdate(new RefreshTokens(usuarioId, refreshToken));
        return refreshToken;
    }

    private async Task<string> ObterToken(ClaimsIdentity claimsIdentity)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _tokenSettings.Emissor,
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = await jwtService.GetCurrentSigningCredentials()
        });

        return tokenHandler.WriteToken(token);
    }

    private static void AdicionarClaims(IdentityUser<int> usuario, IList<Claim> claims)
    {
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, usuario.Email));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
    }

    private static long ToUnixEpochDate(DateTime date)
    {
        return (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
            .TotalSeconds);
    }
    
    private async Task<ResponseMessage> RegistrarCliente(CodAutEntry usuarioRegistro)
    {
        var usuario = await userManager.FindByEmailAsync(usuarioRegistro.Email);

        var usuarioRegistrado = new UsuarioRegistradoIntegrationEvent(usuario.Id, usuarioRegistro.Nome,
            usuarioRegistro.Email, usuarioRegistro.Cpf);

        try
        {
            return await bus.RequestAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(usuarioRegistrado);
        }
        catch
        {
            await userManager.DeleteAsync(usuario);
            throw;
        }
    }
}