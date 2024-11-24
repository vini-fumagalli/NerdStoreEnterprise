using FluentValidation.Results;
using NSE.Core.Data;

namespace NSE.Core.Messages;

public class CommandHandler
{
    protected ValidationResult ValidationResult;

    public CommandHandler()
    {
        ValidationResult = new ValidationResult();
    }

    protected void AdicionarErro(string mensagem)
    {
        ValidationResult.Errors.Add(new ValidationFailure(string.Empty, mensagem));
    }

    protected async Task<ValidationResult> PersitirDados(IUnitOfWork unitOfWork)
    {
        if(!await unitOfWork.Commit()) AdicionarErro("Houve um erro ao persistir os dados!");

        return ValidationResult;
    }
}