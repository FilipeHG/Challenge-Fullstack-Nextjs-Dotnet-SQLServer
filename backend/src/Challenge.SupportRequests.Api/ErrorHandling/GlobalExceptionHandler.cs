using System.Text.Json;
using Challenge.SupportRequests.Application.Common.Exceptions;
using Challenge.SupportRequests.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.SupportRequests.Api.ErrorHandling;

public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetails, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var status = exception switch
        {
            ValidationException => 400,
            ResourceNotFoundException => 404,
            InvalidStatusTransitionException or SupportRequestDeletionNotAllowedException => 409,
            _ => 500
        };
        if (status == 500) logger.LogError(exception, "Request failed with correlation {CorrelationId}", context.TraceIdentifier);
        var problem = new ProblemDetails
        {
            Status = status,
            Type = $"https://httpstatuses.com/{status}",
            Title = status switch { 400 => "Dados inválidos", 404 => "Recurso não encontrado", 409 => "Conflito de regra de negócio", _ => "Erro interno" },
            Detail = status == 500 ? "Ocorreu um erro inesperado." : exception is ValidationException ? "Verifique os campos informados." : exception.Message,
            Instance = context.Request.Path
        };
        problem.Extensions["correlationId"] = context.TraceIdentifier;
        if (exception is ValidationException validation)
            problem.Extensions["errors"] = validation.Errors.GroupBy(x => JsonNamingPolicy.CamelCase.ConvertName(x.PropertyName))
                .ToDictionary(x => x.Key, x => x.Select(y => y.ErrorMessage).Distinct().ToArray());
        context.Response.StatusCode = status;
        await problemDetails.WriteAsync(new ProblemDetailsContext { HttpContext = context, ProblemDetails = problem });
        return true;
    }
}
