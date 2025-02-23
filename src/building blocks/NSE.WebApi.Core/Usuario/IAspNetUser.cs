using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace NSE.WebApi.Core.Usuario;

public interface IAspNetUser
{
    string Name { get; }
    int ObterUserId();
    string ObterUserEmail();
    string ObterUserToken();
    bool EstaAutenticado();
    bool PossuiRole(string role);
    IEnumerable<Claim> ObterClaims();
    HttpContext ObterHttpContext();
}