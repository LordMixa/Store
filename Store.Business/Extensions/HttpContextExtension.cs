using Microsoft.AspNetCore.Http;
using Store.Business.Models.AuditLogs;

namespace Store.Business.Extensions
{
    public static class HttpContextExtension
    {
        public static async Task<string> GetBody(this HttpContext context)
        {
            string request = await FormatRequest(context.Request);

            return request;
        }

        public static async Task<string> GetResponse(this HttpContext context)
        {
            string response = await FormatResponse(context.Response);

            return response;
        }

        private static async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();

            string text = await new StreamReader(request.Body).ReadToEndAsync();

            request.Body.Position = 0;

            return text;
        }

        private static async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);

            var text = await new StreamReader(response.Body).ReadToEndAsync();

            response.Body.Seek(0, SeekOrigin.Begin);

            return text;
        }
    }
}
