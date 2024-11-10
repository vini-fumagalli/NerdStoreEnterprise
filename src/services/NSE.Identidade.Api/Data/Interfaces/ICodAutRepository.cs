using NSE.Identidade.Api.Models;

namespace NSE.Identidade.Api.Data.Interfaces;

public interface ICodAutRepository
{
    Task GerarCodigoEnviarEmail(string email);
    Task<bool> Validar(string email, string codigoAutenticacao);
}