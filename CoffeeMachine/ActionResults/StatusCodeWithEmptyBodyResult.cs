using Microsoft.AspNetCore.Mvc;

namespace CoffeeMachine.ActionResults;

public class StatusCodeWithEmptyBodyResult : JsonResult
{
    public StatusCodeWithEmptyBodyResult(object? value) : base(value)
    {
    }

    public StatusCodeWithEmptyBodyResult(object? value, object? serializerSettings) : base(value, serializerSettings)
    {
    }

    public StatusCodeWithEmptyBodyResult(int? statusCode) : base(new {})
    {
        StatusCode = statusCode;
    }
}