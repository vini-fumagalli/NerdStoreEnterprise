using System.ComponentModel.DataAnnotations;

namespace NSE.Identidade.Api.Models;

public class TokenEntry
{
    [Required(ErrorMessage = "Não foi passado o {0}")]
    public string Token { get; init; }
    
    [Required(ErrorMessage = "Não foi passado o {0}")]
    public string RefreshToken { get; init; }
}