using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NSE.Core.Communication;

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
    
    protected ActionResult CustomResponse(ResponseResult resposta)
    {
        ResponsePossuiErros(resposta);
        return CustomResponse();
    }
    
    protected bool ResponsePossuiErros(ResponseResult resposta)
    {
        if (resposta == null || !resposta.Errors.Mensagens.Any()) return false;

        foreach (var mensagem in resposta.Errors.Mensagens)
        {
            AdicionarErroProcessamento(mensagem);
        }

        return true;
    }
    
    protected bool OperacaoValida() => !Erros.Any();

    protected void AdicionarErroProcessamento(string erro) => Erros.Add(erro);

    protected void LimparErrosProcessamento() => Erros.Clear();
}