using Microsoft.Extensions.DependencyInjection;
using Store.Data.Repositories;
using Store.Data.Repositories.Interfaces;

namespace Store.Data
{
    public static class Configuration
    {
        public static void Configure(IServiceCollection serviceCollection, string connectionString)
        {
            serviceCollection.AddTransient<IOrderRepository, OrderRepository>();
            serviceCollection.AddTransient<IProductRepository, ProductRepository>();
        }
    }
}
