using Dapper;
using Store.Data.Dapper.Repositories.Interfaces;
using Store.Entities.Entities;
using System.Data;

namespace Store.Data.Dapper.Repositories
{
    public class AuditLogRepository : BaseRepository, IAuditLogRepository
    {
        public AuditLogRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public async Task CreateAsync(AuditLog auditLog)
        {
            var parameters = new
            {
                auditLog.HttpMethod,
                auditLog.Request,
                auditLog.Url,
                auditLog.StatusCode,
                auditLog.Response
            };

            await _sqlConnection.ExecuteAsync(
            "Procedure_CreateAuditLog",
            parameters,
            commandType: CommandType.StoredProcedure
            );
        }
    }
}
