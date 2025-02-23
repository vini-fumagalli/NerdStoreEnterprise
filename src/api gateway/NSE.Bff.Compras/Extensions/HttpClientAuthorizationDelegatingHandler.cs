using System.Net.Http.Headers;
using NSE.WebApi.Core.Usuario;

namespace NSE.Bff.Compras.Extensions;

public class HttpClientAuthorizationDelegatingHandler(IAspNetUser user) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authorizationHeader = user.ObterHttpContext().Request.Headers.Authorization;
        if (!string.IsNullOrEmpty(authorizationHeader))
        {
            request.Headers.Add("Authorization", new List<string> { authorizationHeader });
        }

        var token = user.ObterUserToken();
        if (token != null) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}