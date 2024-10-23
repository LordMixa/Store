namespace Store.Api.Middlwares
{
    public class ExceptionMiddlware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddlware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var result = $"I am message from middleware: {ex.Message}";

            return context.Response.WriteAsync(result);
        }
    }
}
