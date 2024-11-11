using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSE.Identidade.Api.Data.Interfaces;
using NSE.Identidade.Api.Extensions;
using NSE.Identidade.Api.Models;
using NSE.WebApi.Core.Identidade;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace NSE.Identidade.Api.Controllers;

[ApiController]
[Route("api/identidade")]
public class AuthController(
    SignInManager<IdentityUser<int>> signInManager,
    UserManager<IdentityUser<int>> userManager, 
    IOptions<AppSettings> appSettings, 
    ICodAutRepository codAutRepository,
    IRefreshTokensRepository refreshTokensRepository) : MainController
{

    private readonly AppSettings _appSettings = appSettings.Value;
    
    [HttpPost("nova-conta")]
    public async Task<ActionResult> Registrar(UsuarioRegistro usuario)
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
    public async Task<ActionResult> CodAut(CodAutEntry codAutEntry)
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
        if (result.Succeeded)
        {
            await signInManager.SignInAsync(user, false);
            return CustomResponse(await GerarJwt(user.Email));
        }
        
        AdicionarErroProcessamento("Erro ao realizar cadastro de usuário");
        return CustomResponse();
    }

    [HttpPost("autenticar")]
    public async Task<ActionResult> Login(UsuarioLogin usuario)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var result = await signInManager.PasswordSignInAsync(usuario.Email, usuario.Senha, false, false);

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
    public async Task<ActionResult> RenovarToken(TokenEntry tokenEntry)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);
        
        var principal = ObterClaimsTokenExpirado(tokenEntry.Token);
        if (principal is null)
        {
            AdicionarErroProcessamento("Token/refresh token inválido");
            return CustomResponse();
        }
        
        var usuarioId = principal.Claims.First(c => c.Type.EndsWith("nameidentifier")).Value;
        if (!await refreshTokensRepository.Validar(int.Parse(usuarioId), tokenEntry.RefreshToken))
        {
            AdicionarErroProcessamento("Token/refresh token inválido");
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
        var token = ObterToken(new ClaimsIdentity(claims));

        return new UsuarioRespostaLogin
        {
            Token = token,
            Email = usuario.Email,
            ExpiraEm = TimeSpan.FromHours(_appSettings.ExpiracaoHoras).TotalSeconds,
            RefreshToken = await GerarRefreshToken(usuario.Id)
        };
    }
    
    private ClaimsPrincipal? ObterClaimsTokenExpirado(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Segredo)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
            out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Token inválido");
        }

        return principal;
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

    private string ObterToken(ClaimsIdentity claimsIdentity)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Segredo);
        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _appSettings.Emissor,
            Audience = _appSettings.ValidoEm,
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        });

        return tokenHandler.WriteToken(token);
    }

    private void AdicionarClaims(IdentityUser<int> usuario, IList<Claim> claims)
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
}