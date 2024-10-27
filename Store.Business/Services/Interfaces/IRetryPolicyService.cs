namespace Store.Business.Services.Interfaces
{
    public interface IRetryPolicyService
    {
        Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation);
    }
}
