using Microsoft.Extensions.Options;
using NSE.Bff.Compras.Extensions;
using NSE.Bff.Compras.Models;
using NSE.Bff.Compras.Services.Interfaces;
using NSE.Core.Communication;

namespace NSE.Bff.Compras.Services;

public class CarrinhoService : Service, ICarrinhoService
{
    private readonly HttpClient _httpClient;

    public CarrinhoService(HttpClient httpClient, IOptions<AppServicesSettings> options)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(options.Value.CarrinhoUrl);
    }

    public async Task<CarrinhoDTO> ObterCarrinho()
    {
        var response = await _httpClient.GetAsync("carrinhos");

        TratarErrosResponse(response);

        return await DeserializarObjetoResponse<CarrinhoDTO>(response);
    }

    public async Task<ResponseResult> AdicionarItemCarrinho(ItemCarrinhoDTO produto)
    {
        var response = await _httpClient.PostAsync("carrinhos", ObterConteudo(produto));

        if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

        return Ok();
    }

    public async Task<ResponseResult> AtualizarItemCarrinho(int produtoId, ItemCarrinhoDTO carrinho)
    {
        var response = await _httpClient.PutAsync($"carrinhos/{produtoId}", ObterConteudo(carrinho));
        
        if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

        return Ok();
    }

    public async Task<ResponseResult> RemoverItemCarrinho(int produtoId)
    {
        var response = await _httpClient.DeleteAsync($"carrinhos/{produtoId}");
        
        if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

        return Ok();
    }

    public async Task<ResponseResult> AplicarVoucherCarrinho(VoucherDTO voucher)
    {
        var response = await _httpClient.PostAsync("carrinhos/aplicar-voucher", ObterConteudo(voucher));

        if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

        return Ok();
    }
}