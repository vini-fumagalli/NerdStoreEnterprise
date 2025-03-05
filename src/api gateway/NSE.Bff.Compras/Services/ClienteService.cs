using System.Net;
using Microsoft.Extensions.Options;
using NSE.Bff.Compras.Extensions;
using NSE.Bff.Compras.Models;
using NSE.Bff.Compras.Services.Interfaces;

namespace NSE.Bff.Compras.Services;
 
public class ClienteService : Service, IClienteService
{
    private readonly HttpClient _httpClient;

    public ClienteService(HttpClient httpClient, IOptions<AppServicesSettings> options)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(options.Value.ClienteUrl);
    }

    public async Task<EnderecoDTO> ObterEndereco()
    {
        var response = await _httpClient.GetAsync("clientes/endereco");
        
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        TratarErrosResponse(response);
        
        return await DeserializarObjetoResponse<EnderecoDTO>(response);
    }
}