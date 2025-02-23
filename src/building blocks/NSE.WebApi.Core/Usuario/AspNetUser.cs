using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace NSE.WebApi.Core.Usuario;

public class AspNetUser(IHttpContextAccessor http) : IAspNetUser
{
    public string Name => http.HttpContext.User.Identity.Name;

    public int ObterUserId() => EstaAutenticado() ? int.Parse(http.HttpContext.User.GetUserId()) : 0;

    public string ObterUserEmail() => EstaAutenticado() ? http.HttpContext.User.GetUserEmail() : "";

    public string ObterUserToken() => EstaAutenticado() ? http.HttpContext.User.GetUserToken() : "";
    
    public bool EstaAutenticado() => http.HttpContext.User.Identity.IsAuthenticated;

    public bool PossuiRole(string role) => http.HttpContext.User.IsInRole(role);

    public IEnumerable<Claim> ObterClaims() => http.HttpContext.User.Claims;

    public HttpContext ObterHttpContext() => http.HttpContext;
}