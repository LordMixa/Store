using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;
using Store.Business.Models.AuditLogs;
using Store.Business.Services.Interfaces;

namespace Store.Business.Services
{
    public class RetryPolicyService : IRetryPolicyService
    {
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly IAuditLogService _auditLogService;

        public RetryPolicyService(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;

            _retryPolicy = Policy
                .Handle<SqlException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetryAsync: OnRetryAsync);
        }

        private async Task OnRetryAsync(Exception ex, TimeSpan timeSpan, int retryCount, Context context)
        {
            await _auditLogService.CreateAsync(new AuditLogCreateModel
            {
                Response = "I am from retry policy: exception is {ex.Exception}, retryCount is {retryCount}",
                StatusCode = 1,
                Request = "Test",
                HttpMethod = "TestMethod",
                Url = "books"
            });
        }

        public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation) => _retryPolicy.ExecuteAsync(operation);

        public Task ExecuteAsync<TResult>(Func<Task> operation) => _retryPolicy.ExecuteAsync(operation);
    }
}
