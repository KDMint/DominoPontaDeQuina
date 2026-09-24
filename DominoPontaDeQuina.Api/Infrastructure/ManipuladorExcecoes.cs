using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DominoPontaDeQuina.Api.Infrastructure;

/// <summary>Converte as exceções de regra lançadas pelos services em respostas ProblemDetails.</summary>
public class ManipuladorExcecoes(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext contexto,
        Exception excecao,
        CancellationToken cancelamento)
    {
        var (status, titulo) = excecao switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            InvalidOperationException => (StatusCodes.Status409Conflict, "Operação inválida para o estado atual"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
            _ => (0, string.Empty)
        };

        if (status == 0)
            return false;

        contexto.Response.StatusCode = status;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = contexto,
            Exception = excecao,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = titulo,
                Detail = excecao.Message
            }
        });
    }
}
