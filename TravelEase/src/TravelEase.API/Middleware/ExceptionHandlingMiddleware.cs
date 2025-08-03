using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace TravelEase.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var errorMessage = ex.Message;

            switch (ex)
            {
                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    break;

                case ArgumentException:
                case InvalidOperationException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;

                case ApplicationException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;

                case NullReferenceException:
                    statusCode = HttpStatusCode.BadRequest;
                    errorMessage = "A required value was null or missing.";
                    break;

                case DbUpdateException dbEx:
                    statusCode = HttpStatusCode.BadRequest;
                    errorMessage = ExtractDatabaseErrorMessage(dbEx);
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    errorMessage = "An unexpected error occurred. Please try again later.";
                    break;
            }

            var response = new
            {
                success = false,
                statusCode = (int)statusCode,
                error = errorMessage,
                details = ex.InnerException?.Message,
                timestamp = DateTime.UtcNow
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return context.Response.WriteAsync(json);
        }

        private static string ExtractDatabaseErrorMessage(DbUpdateException dbEx)
        {
            if (dbEx.InnerException != null)
            {
                var message = dbEx.InnerException.Message;

                if (message.Contains("FOREIGN KEY"))
                    return "Database constraint violation: related record not found.";
                if (message.Contains("UNIQUE"))
                    return "A record with the same value already exists.";

                return message;
            }

            return "A database update error occurred.";
        }
    }
}
