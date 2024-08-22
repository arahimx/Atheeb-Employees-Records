using System.Diagnostics;
using System.Security.Claims;

namespace RestApi.Middleware
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;

        public ExceptionLoggingMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await LogExceptionToTextFileAsync(ex);
                throw;
            }
        }

        private async Task LogExceptionToTextFileAsync(Exception exception, [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            try
            {
                var customRequestId = Guid.NewGuid().ToString();
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var requestPath = _httpContextAccessor.HttpContext?.Request.Path;

                string innerExceptionMessage = exception.InnerException?.Message;
                var details = exception.Message;
                if (innerExceptionMessage != null)
                {
                    details += $" - Inner Exception: {innerExceptionMessage}";
                }

                var logEntry = $"Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}\n" +
                               $"Request ID: {Activity.Current?.Id ?? customRequestId}\n" +
                               $"User ID: {userId}\n" +
                               $"Request Path: {requestPath}\n" +
                               $"Message: {exception.Message}\n" +
                               $"Inner Exception: {innerExceptionMessage}\n" +
                               $"StackTrace: {exception.StackTrace}\n" +
                               $"Line Number: {lineNumber}\n" +
                               $"--------------------------------------------------\n";

                var logsPath = Path.Combine(_env.WebRootPath, "logs");
                if (!Directory.Exists(logsPath))
                {
                    Directory.CreateDirectory(logsPath);
                }

                var logFilePath = Path.Combine(logsPath, $"exceptions_{DateTime.UtcNow:yyyyMMdd}.txt");

                await File.AppendAllTextAsync(logFilePath, logEntry);
            }
            catch (Exception fileEx)
            {
                // Handle any errors that occur while writing to the log file
                Console.WriteLine($"Error while writing exception to text file: {fileEx.Message}");
            }
        }
    }
}
