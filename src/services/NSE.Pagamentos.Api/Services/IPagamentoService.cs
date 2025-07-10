using NSE.Core.Messages.Integration;
using NSE.Pagamentos.Api.Models;

namespace NSE.Pagamentos.Api.Services;

public interface IPagamentoService
{
    Task<ResponseMessage> AutorizarPagamento(Pagamento pagamento);
    Task<ResponseMessage> CapturarPagamento(int pedidoId);
    Task<ResponseMessage> CancelarPagamento(int pedidoId);
}