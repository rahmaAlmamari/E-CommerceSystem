using System.Text.Json;
using Microsoft.Extensions.Logging;
using E_CommerceSystem.Models; 

namespace E_CommerceSystem.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IWebHostEnvironment env)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                _logger.LogWarning(ex, "Handled AppException: {Message}", ex.Message);
                await WriteProblem(context, ex.StatusCode, ex.Message, ex.ErrorCode, env, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized");
                await WriteProblem(context, StatusCodes.Status401Unauthorized, "Unauthorized", "UNAUTHORIZED", env, ex);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found");
                await WriteProblem(context, StatusCodes.Status404NotFound, ex.Message, "NOT_FOUND", env, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await WriteProblem(context, StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.", "INTERNAL_ERROR", env, ex);
            }
        }

        private static Task WriteProblem(
            HttpContext context,
            int statusCode,
            string message,
            string? errorCode,
            IWebHostEnvironment env,
            Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var payload = new
            {
                status = statusCode,
                title = message,
                errorCode,
                traceId = context.TraceIdentifier,
                details = env.IsDevelopment() ? ex.ToString() : null
            };

            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            return context.Response.WriteAsync(json);
        }
    }
}
