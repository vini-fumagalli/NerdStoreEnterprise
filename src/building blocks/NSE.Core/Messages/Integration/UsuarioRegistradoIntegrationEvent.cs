namespace NSE.Core.Messages.Integration;

public class UsuarioRegistradoIntegrationEvent : IntegrationEvent
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string Cpf { get; private set; }

    public UsuarioRegistradoIntegrationEvent(int id, string nome, string email, string cpf)
    {
        Id = id;
        Nome = nome;
        Email = email;
        Cpf = cpf;
    }
}