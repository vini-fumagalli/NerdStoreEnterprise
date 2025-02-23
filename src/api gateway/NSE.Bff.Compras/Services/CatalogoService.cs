using Microsoft.Extensions.Options;
using NSE.Bff.Compras.Extensions;
using NSE.Bff.Compras.Models;
using NSE.Bff.Compras.Services.Interfaces;

namespace NSE.Bff.Compras.Services;

public class CatalogoService : Service, ICatalogoService
{
    private readonly HttpClient _httpClient;
    
    public CatalogoService(HttpClient client, IOptions<AppServicesSettings> options)
    {
        _httpClient = client;
        _httpClient.BaseAddress = new Uri(options.Value.CatalogoUrl);
    }
    
    public async Task<ItemProdutoDTO> ObterPorId(int id)
    {
        var response = await _httpClient.GetAsync($"/catalogos/produtos/{id}");

        TratarErrosResponse(response);

        return await DeserializarObjetoResponse<ItemProdutoDTO>(response);
    }

    public async Task<IEnumerable<ItemProdutoDTO>> ObterItens(IEnumerable<int> ids)
    {
        var idsRequest = string.Join(",", ids);
        var response = await _httpClient.GetAsync($"/catalogos/produtos/lista/{idsRequest}");

        TratarErrosResponse(response);

        return await DeserializarObjetoResponse<IEnumerable<ItemProdutoDTO>>(response);
    }
}