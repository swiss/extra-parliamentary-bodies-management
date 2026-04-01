using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Api.Filters;

public class DbUpdateConcurrencyExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Exception is DbUpdateConcurrencyException)
        {
            context.Result = new ConflictObjectResult(new
            {
                error = "Entity has been modified since last read."
            });
            context.ExceptionHandled = true;
        }
    }
}
