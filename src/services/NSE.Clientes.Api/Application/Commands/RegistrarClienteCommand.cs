using FluentValidation;
using FluentValidation.Results;
using NSE.Core.Messages;

namespace NSE.Clientes.Api.Application.Commands;

public class RegistrarClienteCommand(int id, string nome, string email, string cpf)
    : Command
{
    public int Id { get; private set; } = id;
    public string Nome { get; private set; } = nome;
    public string Email { get; private set; } = email;
    public string Cpf { get; private set; } = cpf;

    public override bool EhValido()
    {
        ValidationResult = new RegistrarClienteValidation().Validate(this);
        return ValidationResult.IsValid;
    }

    public class RegistrarClienteValidation : AbstractValidator<RegistrarClienteCommand>
    {
        public RegistrarClienteValidation()
        {
            RuleFor(c => c.Nome)
                .NotEmpty()
                .WithMessage("O do cliente não foi informado.");

            RuleFor(c => c.Cpf)
                .Must(TerCpfValido)
                .WithMessage("O CPF informado não é válido.");
            
            RuleFor(c => c.Email)
                .Must(TerEmailValido)
                .WithMessage("O e-mail informado não é válido.");
        }

        protected static bool TerCpfValido(string cpf) => Core.DomainObjects.Cpf.Validar(cpf);

        protected static bool TerEmailValido(string email) => Core.DomainObjects.Email.Validar(email);
    }
}