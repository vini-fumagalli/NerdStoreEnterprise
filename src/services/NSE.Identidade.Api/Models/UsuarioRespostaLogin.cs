namespace NSE.Identidade.Api.Models;

public class UsuarioRespostaLogin
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public double ExpiraEm { get; set; }
    public string Email { get; set; }
}