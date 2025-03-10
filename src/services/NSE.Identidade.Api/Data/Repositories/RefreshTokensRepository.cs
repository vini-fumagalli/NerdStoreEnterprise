using Microsoft.EntityFrameworkCore;
using NSE.Identidade.Api.Data.Interfaces;
using NSE.Identidade.Api.Models;

namespace NSE.Identidade.Api.Data.Repositories;

public class RefreshTokensRepository(IdentidadeDbContext context) : IRefreshTokensRepository
{
    public async Task CreateOrUpdate(RefreshTokens refreshTokens)
    {
        var exists = await Encontrar(refreshTokens.UsuarioId);
        if (exists is null)
        {
            await context.RefreshTokens.AddAsync(refreshTokens);
            await context.SaveChangesAsync();
            return;
        }

        exists.Update(refreshTokens);
        await context.SaveChangesAsync();
    }

    public async Task<bool> Validar(int usuarioId, string refreshToken)
    {
        var entity = await Encontrar(usuarioId);

        return RefreshTokenValido(entity, refreshToken);
    }
    
    private static bool RefreshTokenValido(RefreshTokens entity, string refreshToken)
        => entity is not null &&
           entity.RefreshToken == refreshToken &&
           entity.ValidoAte >= DateTime.Now;

    private async Task<RefreshTokens> Encontrar(int usuarioId)
    {
        return await context.RefreshTokens.FindAsync(usuarioId);
    }
}