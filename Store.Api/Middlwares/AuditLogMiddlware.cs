using Store.Business.Extensions;
using Store.Business.Models.AuditLogs;
using Store.Business.Services.Interfaces;

namespace Store.Api.Middlwares
{
    public class AuditLogMiddlware
    {
        private readonly RequestDelegate _next;
        private readonly IAuditLogService _auditLogService;

        public AuditLogMiddlware(RequestDelegate next, IAuditLogService auditLogService)
        {
            _next = next;
            _auditLogService = auditLogService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var auditLogModel = new AuditLogCreateModel();

            auditLogModel.HttpMethod = context.Request.Method;
            auditLogModel.Request = await context.GetBody();
            auditLogModel.Url = context.Request.Path;

            var originalBodyStream = context.Response.Body;

            using (var responseBodyStream = new MemoryStream())
            {
                context.Response.Body = responseBodyStream;

                await _next(context);

                auditLogModel.StatusCode = context.Response.StatusCode;

                auditLogModel.Response = await context.GetResponse();

                await responseBodyStream.CopyToAsync(originalBodyStream);

                await AuditAsync(auditLogModel);
            }
        }

        private async Task AuditAsync(AuditLogCreateModel auditLogModel)
        {
            await _auditLogService.CreateAsync(auditLogModel);
        }
    }
}
