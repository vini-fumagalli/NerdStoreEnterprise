using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NSE.WebApi.Core.Identidade;

public class CustomAuthorization
{
    public static bool ValidarClaimsUsuario(HttpContext context, string claimName, string claimValue)
        => context.User.Identity.IsAuthenticated &&
           context.User.Claims.Any(c => c.Type == claimName && c.Value.Contains(claimValue));
}

public class ClaimsAuthorizeAttribute : TypeFilterAttribute
{
    public ClaimsAuthorizeAttribute(Acesso acesso, Permissao[] permissao) : base(typeof(RequisitoClaimFilter))
    {
        Arguments = new object[] { new Claim(acesso.ToString(), string.Join(',', permissao)) };
    }
}

public class RequisitoClaimFilter(Claim claim) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result = new StatusCodeResult(401);
            return;
        }

        if (!CustomAuthorization.ValidarClaimsUsuario(context.HttpContext, claim.Type, claim.Value))
        {
            context.Result = new StatusCodeResult(403);
        }
    }
}