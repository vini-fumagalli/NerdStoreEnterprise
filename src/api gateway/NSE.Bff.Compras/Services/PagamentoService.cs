using Microsoft.Extensions.Options;
using NSE.Bff.Compras.Extensions;
using NSE.Bff.Compras.Services.Interfaces;

namespace NSE.Bff.Compras.Services;

public class PagamentoService : Service, IPagamentoService
{
    private readonly HttpClient _httpClient;

    public PagamentoService(HttpClient httpClient, IOptions<AppServicesSettings> options)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(options.Value.PagamentoUrl);
    }
}