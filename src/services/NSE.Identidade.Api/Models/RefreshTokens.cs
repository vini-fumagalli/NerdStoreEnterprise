using System.ComponentModel.DataAnnotations;

namespace NSE.Identidade.Api.Models;

public class RefreshTokens
{
    [Key] public int UsuarioId { get; private set; } 
    public string RefreshToken { get; private set; }
    public DateTime ValidoAte { get; set; }
    
    private RefreshTokens() { }

    public RefreshTokens(int usuarioId, string refreshToken)
    {
        UsuarioId = usuarioId;
        RefreshToken = refreshToken;
        SetValidoAte();
    }

    public void Update(RefreshTokens refreshTokens)
    {
        RefreshToken = refreshTokens.RefreshToken;
        SetValidoAte();
    }

    private void SetValidoAte()
    {
        ValidoAte = DateTime.Now.AddDays(7);
    }
}