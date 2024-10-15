using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Store.Data.Repositories;
using Store.Data.Repositories.Interfaces;
using System.Data;

namespace Store.Data
{
    public static class Configuration
    {
        public static void Configure(IServiceCollection serviceCollection, string connectionString)
        {
            serviceCollection.AddTransient<IOrderRepository, OrderRepository>();
            serviceCollection.AddTransient<IBookRepository, BookRepository>();

            serviceCollection.AddTransient<IDbConnection>(sp => new SqlConnection(connectionString));
        }
    }
}
