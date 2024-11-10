namespace NSE.Identidade.Api.Services.Interfaces;

public interface IEmailService
{
    Task EnviarEmail(string userEmail, string codAut);
}