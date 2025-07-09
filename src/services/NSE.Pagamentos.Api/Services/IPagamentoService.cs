using NSE.Core.Messages.Integration;
using NSE.Pagamentos.Api.Models;

namespace NSE.Pagamentos.Api.Services;

public interface IPagamentoService
{
    Task<ResponseMessage> AutorizarPagamento(Pagamento pagamento);
}