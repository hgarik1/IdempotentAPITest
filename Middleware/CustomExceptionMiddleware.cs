using System.Net;

namespace IdempotentAPITest.Middleware
{
    public class CustomExceptionMiddleware
    {
        private const string JsonContentType = "application/json";
        private readonly RequestDelegate _request;
        private readonly ILogger<CustomExceptionMiddleware> _logger;

        public CustomExceptionMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            _request = next;
            _logger = loggerFactory.CreateLogger<CustomExceptionMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context, IHostEnvironment host)
        {
            Exception exception = null;
            try
            {
                await _request(context);
            }
            catch (Exception ex)
            {
                exception = ex;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                throw;
            }
            finally
            {

                Console.WriteLine("FINALLY");
            }

        }

    }
}
