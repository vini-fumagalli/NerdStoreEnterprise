using NSE.Core.DomainObjects;

namespace NSE.Clientes.Api.Models;

public class Cliente : Entity, IAggregateRoot
{
    public string Nome { get; private set; }
    public Email Email { get; private set; }
    public Cpf Cpf { get; private set; }
    public bool Excluido { get; private set; }
    public Endereco Endereco { get; private set; }

    
    protected Cliente() { }

    public Cliente(int id, string nome, string email, string cpf)
    {
        Id = id;
        Nome = nome;
        Email = new Email(email);
        Cpf = new Cpf(cpf);
        Excluido = false;
    }

    public void TrocarEmail(string email)
    {
        Email = new Email(email);
    }

    public void AtribuirEndereco(Endereco endereco)
    {
        Endereco = endereco;
    }
}