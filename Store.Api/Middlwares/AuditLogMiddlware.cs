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
            await _next(context);

            await AuditAsync(context);
        }

        private async Task AuditAsync(HttpContext context)
        {
            var auditLogModel = new AuditLogCreateModel()
            {
                HttpMethod = "Test method4",
                Request = "TestRequest3",
                Url = "htt3p",
                StatusCode = 203,
                Response = "TestResponse3"
            };

            int id = await _auditLogService.CreateAsync(auditLogModel);
        }
    }
}
