
using System.Net.Mail;
using MailKit.Security;
using MimeKit;
using NSE.Identidade.Api.Services.Interfaces;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace NSE.Identidade.Api.Services;

public class EmailService(IConfiguration configuration) : IEmailService
{
    public async Task EnviarEmail(string userEmail, string codAut)
    {
        var address = configuration["EmailSettings:Address"];
        var password = configuration["EmailSettings:Password"];
        var toAdress = userEmail;

        var emailMessage = GerarMensagem(address, toAdress, codAut);
        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(address, password);
            await client.SendAsync(emailMessage);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Falha ao enviar email", ex);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }

    private static MimeMessage GerarMensagem(string address, string toAdress, string codAut)
    {
        var emailMessage = new MimeMessage
        {
            Sender = MailboxAddress.Parse(address),
            Subject = "Complete seu cadastro!!"
        };
        
        emailMessage.To.Add(MailboxAddress.Parse(toAdress));

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $"Seu código de autenticação é {codAut}"
        };

        emailMessage.Body = bodyBuilder.ToMessageBody();
        emailMessage.From.Add(new MailboxAddress("Nerd Store Enterprise", address));

        return emailMessage;
    }
}