using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace NSE.WebApi.Core.Extensions;

public static class PollyExtensions
{
    public static AsyncRetryPolicy<HttpResponseMessage> EsperarTentar()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync([
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            ]);
    }
}