using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Store.Data.Dapper.Repositories.Interfaces;
using Store.Data.Dapper.Repositories;
using System.Data;
using Store.Data.Dapper.FluentMapConfigurations;

namespace Store.Data.Dapper
{
    public static class Configuration
    {
        public static void Configure(IServiceCollection serviceCollection, string connectionString)
        {
            serviceCollection.AddTransient<IDbConnection>(sp => new SqlConnection(connectionString));

            serviceCollection.AddTransient<IBookRepository, BookRepository>();

            FluentMapConfig.Initialize();
        }
    }
}
