using Store.Entities.Entities;

namespace Store.Data.Dapper.Repositories.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<int> CreateAsync(AuditLog auditLog);
    }
}
