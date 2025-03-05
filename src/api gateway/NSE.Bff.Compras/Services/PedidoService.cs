using System.Net;
using Microsoft.Extensions.Options;
using NSE.Bff.Compras.Extensions;
using NSE.Bff.Compras.Models;
using NSE.Bff.Compras.Services.Interfaces;
using NSE.Core.Communication;

namespace NSE.Bff.Compras.Services;

public class PedidoService : Service, IPedidoService
{
    private readonly HttpClient _httpClient;

    public PedidoService(HttpClient httpClient, IOptions<AppServicesSettings> options)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(options.Value.PedidoUrl);
    }
    
    public async Task<ResponseResult> FinalizarPedido(PedidoDTO pedido)
    {
        var response = await _httpClient.PostAsync("pedidos", ObterConteudo(pedido));

        if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

        return Ok();
    }

    public async Task<PedidoDTO> ObterUltimoPedido()
    {
        var response = await _httpClient.GetAsync("pedidos/ultimo");

        if (response.StatusCode == HttpStatusCode.NotFound) return null;

        TratarErrosResponse(response);

        return await DeserializarObjetoResponse<PedidoDTO>(response);
    }

    public async Task<IEnumerable<PedidoDTO>> ObterListaPorClienteId()
    {
        var response = await _httpClient.GetAsync("pedidos/lista-cliente");

        if (response.StatusCode == HttpStatusCode.NotFound) return null;

        TratarErrosResponse(response);

        return await DeserializarObjetoResponse<IEnumerable<PedidoDTO>>(response);
    }

    public async Task<VoucherDTO> ObterVoucherPorCodigo(string codigo)
    {
        var response = await _httpClient.GetAsync($"vouchers/{codigo}");

        if (response.StatusCode == HttpStatusCode.NotFound) return null;

        TratarErrosResponse(response);

        return await DeserializarObjetoResponse<VoucherDTO>(response);
    }
}