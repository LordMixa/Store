using Microsoft.Data.SqlClient;
using System.Data;

namespace Store.Data.Dapper.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly SqlConnection _sqlConnection;

        protected BaseRepository(IDbConnection dbConnection)
        {
            if (dbConnection is not SqlConnection sqlConnection)
            {
                throw new ArgumentException("Invalid database connection type. SqlConnection is required.", nameof(dbConnection));
            }

            _sqlConnection = sqlConnection;
        }
    }
}
