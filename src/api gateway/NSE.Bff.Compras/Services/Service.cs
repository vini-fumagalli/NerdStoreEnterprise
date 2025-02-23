using System.Net;
using System.Text;
using System.Text.Json;
using NSE.Core.Communication;

namespace NSE.Bff.Compras.Services;

public abstract class Service
{
    protected StringContent ObterConteudo(object dado)
        => new(JsonSerializer.Serialize(dado), Encoding.UTF8, "application/json");

    protected async Task<T> DeserializarObjetoResponse<T>(HttpResponseMessage responseMessage)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<T>(await responseMessage.Content.ReadAsStringAsync(), options);
    }

    protected bool TratarErrosResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return false;

        response.EnsureSuccessStatusCode();
        return true;
    }

    protected ResponseResult Ok() => new();
}