using NSE.Identidade.Api.Models;

namespace NSE.Identidade.Api.Data.Interfaces;

public interface IRefreshTokensRepository
{
    Task CreateOrUpdate(RefreshTokens refreshTokens);
    Task<bool> Validar(int usuarioId, string refreshToken);
}