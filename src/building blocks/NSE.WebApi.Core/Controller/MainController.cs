using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NSE.WebApi.Core.Controller;

public class MainController : ControllerBase
{
    protected ICollection<string> Erros = new List<string>();

    protected ActionResult CustomResponse(object result = null)
    {
        if (OperacaoValida()) return Ok(result);

        return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Mensagens", Erros.ToArray() }
        }));
    }

    protected ActionResult CustomResponse(ModelStateDictionary modelStateDictionary)
    {
        var erros = modelStateDictionary.Values.SelectMany(e => e.Errors).ToList();
        erros.ForEach(e => AdicionarErroProcessamento(e.ErrorMessage));
        return CustomResponse();
    }
    
    protected ActionResult CustomResponse(ValidationResult validationResult)
    {
        var erros = validationResult.Errors.ToList();
        erros.ForEach(e => AdicionarErroProcessamento(e.ErrorMessage));
        return CustomResponse();
    }
    
    protected bool OperacaoValida() => !Erros.Any();

    protected void AdicionarErroProcessamento(string erro) => Erros.Add(erro);

    protected void LimparErrosProcessamento() => Erros.Clear();
}