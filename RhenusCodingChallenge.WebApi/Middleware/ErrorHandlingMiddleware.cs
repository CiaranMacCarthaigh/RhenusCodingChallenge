using Microsoft.AspNetCore.Mvc;
using RhenusCodingChallenge.Application.Exceptions;

namespace RhenusCodingChallenge.WebApi.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidCommandException exception)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "ValidationFailure",
                    Title = "Validation error",
                    Detail = exception.Reason
                };
                problemDetails.Extensions["propertyName"] = exception.PropertyName;

                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
            catch (AggregateNotFoundException exception)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Type = "AggregateNotFound",
                    Title = "Aggregate not found error",
                    Detail = "The aggregate with the specified ID could not be found"
                };
                problemDetails.Extensions["aggregateId"] = exception.AggregateId;

                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    }
}
