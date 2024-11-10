using System.ComponentModel.DataAnnotations;

namespace NSE.Identidade.Api.Models;

public class CodAut
{
    [Key] public string Email { get; private set; }
    public string CodigoAutenticacao { get; private set; }

    private CodAut() { }

    public CodAut(string email, string codigoAutenticacao)
    {
        Email = email;
        CodigoAutenticacao = codigoAutenticacao;
    }
}