using Bk.APG.Api.Filters;
using Bk.APG.CrossCutting.Exception;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Bk.APG.Api.Tests.Filters;

[TestFixture]
internal class EntityNotFoundExceptionFilterTests
{
    private EntityNotFoundExceptionFilter _filter = null!;

    [SetUp]
    public void SetUp()
    {
        _filter = new EntityNotFoundExceptionFilter();
    }

    [Test]
    public void OnException_WhenEntityNotFoundException_SetsStatusCodeTo404()
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = new EntityNotFoundException("Foo")
        };

        _filter.OnException(exceptionContext);

        Assert.Multiple(() =>
        {
            Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(exceptionContext.ExceptionHandled, Is.True);
        });
    }

    [Test]
    public void OnException_WhenDifferentException_DoesNotHandleOrSetResult()
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = new InvalidOperationException("Some other error")
        };

        _filter.OnException(exceptionContext);

        Assert.Multiple(() =>
        {
            Assert.That(exceptionContext.Result, Is.Null);
            Assert.That(exceptionContext.ExceptionHandled, Is.False);
        });
    }
}
