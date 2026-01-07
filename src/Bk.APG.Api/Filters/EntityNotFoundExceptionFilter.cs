using Bk.APG.CrossCutting.Exception;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Bk.APG.Api.Filters;

public class EntityNotFoundExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is EntityNotFoundException)
        {
            context.HttpContext.Response.StatusCode = 404;
            context.ExceptionHandled = true;
        }
    }
}
