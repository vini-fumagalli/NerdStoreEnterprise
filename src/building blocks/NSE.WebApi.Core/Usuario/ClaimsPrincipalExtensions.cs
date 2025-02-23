using System.Security.Claims;

namespace NSE.WebApi.Core.Usuario;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentException(nameof(principal));

        return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public static string GetUserEmail(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentException(nameof(principal));

        return principal.FindFirst(ClaimTypes.Email)?.Value;
    }

    public static string GetUserToken(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentException(nameof(principal));

        return principal.FindFirst("JWT")?.Value;
    }
}