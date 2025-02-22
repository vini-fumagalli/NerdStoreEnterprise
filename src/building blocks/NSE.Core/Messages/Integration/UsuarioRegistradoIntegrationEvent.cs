namespace NSE.Core.Messages.Integration;

public class UsuarioRegistradoIntegrationEvent(int id, string nome, string email, string cpf)
    : IntegrationEvent
{
    public int Id { get; private set; } = id;
    public string Nome { get; private set; } = nome;
    public string Email { get; private set; } = email;
    public string Cpf { get; private set; } = cpf;
}