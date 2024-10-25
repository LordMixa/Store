using Store.Business.Models.AuditLogs;

namespace Store.Business.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task CreateAsync(AuditLogCreateModel auditLogModel);
    }
}
