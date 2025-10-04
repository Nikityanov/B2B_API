using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace B2B_API.API.Middleware
{
    /// <summary>
    /// Глобальный обработчик исключений с поддержкой FluentResults
    /// </summary>
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // Обрабатываем неудачные результаты FluentResults
                if (context.Response.StatusCode >= 400 && context.Response.HasStarted == false)
                {
                    HandleFluentResultErrors(context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Необработанное исключение в запросе {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var errorResponse = new
            {
                Success = false,
                Message = "Внутренняя ошибка сервера",
                ErrorCode = "INTERNAL_ERROR",
                Timestamp = DateTime.UtcNow
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
        }

        private void HandleFluentResultErrors(HttpContext context)
        {
            // FluentResults ошибки уже обработаны в контроллерах
            // Этот метод можно расширить для дополнительной обработки
        }
    }

    /// <summary>
    /// Расширение для регистрации middleware
    /// </summary>
    public static class GlobalExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandler>();
        }
    }
}
