using Microsoft.Extensions.DependencyInjection;
using Store.Business.MapperConfigurations;
using Store.Business.Services;
using Store.Business.Services.Interfaces;

namespace Store.Business
{
    public static class Configuration
    {
        public static void Configure(IServiceCollection serviceCollection, string connectionString)
        {
            Data.Dapper.Configuration.Configure(serviceCollection, connectionString);

            serviceCollection.AddAutoMapper(typeof(ModelsMappingProfile));

            //serviceCollection.AddTransient<IOrderService, OrderService>();
            serviceCollection.AddTransient<IBookService, BookService>();
            serviceCollection.AddTransient<IRetryPolicyService, RetryPolicyService>();
            serviceCollection.AddTransient<IAuditLogService, AuditLogService>();
            serviceCollection.AddTransient<IAuthorService, AuthorService>();
            serviceCollection.AddTransient<ICategoryService, CategoryService>();
        }
    }
}
