using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Store.Data.Dapper
{
    public static class Configuration
    {
        public static void Configure(IServiceCollection serviceCollection, string connectionString)
        {
            serviceCollection.AddTransient<IDbConnection>(sp => new SqlConnection(connectionString));
        }
    }
}
