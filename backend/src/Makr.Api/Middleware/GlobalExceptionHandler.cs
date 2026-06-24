using Makr.Domain.Constants;
using Makr.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;

namespace Makr.Api.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

            var (statusCode, apiError) = MapException(exception);

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            await httpContext.Response.WriteAsJsonAsync(apiError, options);

            return true;
        }

        private static (int, ApiError) MapException(Exception exception)
        {
            return exception switch
            {
                ApiException ex => (ex.StatusCode, ex.Error),

                _ => (StatusCodes.Status500InternalServerError, new ApiError
                {
                    Code = ErrorCode.General.Unknown,
                    Message = exception.Message
                })
            };
        }
    }
}
