using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace InvestYes.API.Configuration
{
    public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", httpContext.TraceIdentifier);

            var problem = exception switch
            {
                ValidationException validation => CreateValidationProblem(httpContext, validation),
                KeyNotFoundException => CreateProblem(httpContext, StatusCodes.Status404NotFound, "Resource not found", exception.Message),
                UnauthorizedAccessException => CreateProblem(httpContext, StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message),
                DbUpdateException => CreateProblem(httpContext, StatusCodes.Status409Conflict, "Database error", exception.Message),
                ArgumentException => CreateProblem(httpContext, StatusCodes.Status400BadRequest, "Invalid request", exception.Message),
                _ => CreateProblem(httpContext, StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred.")
            };

            httpContext.Response.StatusCode = problem.Status!.Value;
            httpContext.Response.ContentType = "application/problem+json";
            
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            
            return true;
        }
        private static ProblemDetails CreateProblem(HttpContext context, int status, string title, string detail)
        {
            return new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path,
                Type = $"https://httpstatuses.com/{status}",
                Extensions = { ["traceId"] = context.TraceIdentifier, ["timestamp"] = DateTime.UtcNow }
            };
        }
        private static ValidationProblemDetails CreateValidationProblem(HttpContext context, ValidationException exception)
        {
            var errors = exception.Errors.GroupBy(x => x.PropertyName).ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());
            
            return new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed",
                Detail = "One or more validation errors occurred.",
                Instance = context.Request.Path,
                Type = "https://httpstatuses.com/400",
                Extensions = { ["traceId"] = context.TraceIdentifier, ["timestamp"] = DateTime.UtcNow }
            };
        }
    }
}
