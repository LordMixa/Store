using AutoMapper;
using Store.Business.Models.AuditLogs;
using Store.Business.Services.Interfaces;
using Store.Data.Dapper.Repositories.Interfaces;
using Store.Entities.Entities;

namespace Store.Business.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IMapper _mapper;
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogService(IMapper mapper, IAuditLogRepository auditLogRepository)
        {
            _mapper = mapper;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<int> CreateAsync(AuditLogCreateModel auditLogModel)
        {
            var auditLog = _mapper.Map<AuditLog>(auditLogModel);

            var id = await _auditLogRepository.CreateAsync(auditLog);

            return id;
        }
    }
}
