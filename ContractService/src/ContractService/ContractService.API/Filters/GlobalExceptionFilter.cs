using ContractService.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace ContractService.API.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var problem = new ProblemDetails
            {
                Title = "Erro inesperado",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = exception.Message
            };

            switch (exception)
            {
                case ValidationException validationEx:
                    problem.Title = "Erro de validação";
                    problem.Status = (int)HttpStatusCode.BadRequest;
                    problem.Detail = "Ocorreram erros de validação nos dados enviados.";
                    problem.Extensions["errors"] = validationEx.Errors.Select(e => e.ErrorMessage);
                    break;

                case DomainValidationException domainEx:
                    problem.Title = "Erro de domínio";
                    problem.Status = (int)HttpStatusCode.BadRequest;
                    problem.Detail = domainEx.Message;
                    break;
            }

            context.Result = new ObjectResult(problem)
            {
                StatusCode = problem.Status
            };

            context.ExceptionHandled = true;
        }
    }
}
