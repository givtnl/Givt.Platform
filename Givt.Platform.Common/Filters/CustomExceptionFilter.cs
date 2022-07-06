using Givt.Platform.Common.Enums;
using Givt.Platform.Common.Exceptions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Sinks.Http.Logger;
using System.Security.Claims;

namespace Givt.Platform.Common.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CustomExceptionFilter : ExceptionFilterAttribute
{
    private readonly ILog _logger;

    public CustomExceptionFilter(ILog logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Returns an uniform exception to the client
    /// </summary>
    /// <param name="context"></param>
    public override void OnException(ExceptionContext context)
    {
        var logLevel = GivtExceptionLevel.Error;

        switch (context.Exception)
        {
            case GivtException givtException:
                logLevel = givtException.Level;
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = givtException.ErrorCode;
                context.Result = new JsonResult(new
                {
                    givtException.AdditionalInformation,
                    givtException.ErrorCode,
                    givtException.Message
                });
                break;
            case TaskCanceledException _:
            case OperationCanceledException _:
                context.ExceptionHandled = true;
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = 499; //Client Closed Request;
                context.Result = new JsonResult(new
                {
                    ErrorCode = 499
                });
                break;
        }

        if (context.ExceptionHandled)
            return;

        var exceptionObject = new
        {
            Exception = logLevel == GivtExceptionLevel.Error ? context.Exception.ToString() : context.Exception.Message,
            Authenticated = context.HttpContext.User != null,
            UserId = context.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
            RequestUrl = context.HttpContext.Request.GetDisplayUrl(),
            QueryParameters = context.HttpContext.Request.QueryString.ToString()
        };

        switch (logLevel)
        {
            case GivtExceptionLevel.Information:
                _logger.Information($"Exception occured {exceptionObject}");
                break;
            case GivtExceptionLevel.Warning:
                _logger.Warning($"Exception occured {exceptionObject}");
                break;
            case GivtExceptionLevel.Error:
                _logger.Error($"Exception occured {exceptionObject}");
                break;
        }
    }
}

