using Microsoft.Data.SqlClient;
using System.Data;

namespace Store.Data.Dapper.Repositories
{
    public abstract class BaseRepository : IDisposable
    {
        protected SqlConnection _sqlConnection;
        private bool _disposedValue;

        protected BaseRepository(IDbConnection dbConnection)
        {
            if (dbConnection is not SqlConnection sqlConnection)
            {
                throw new ArgumentException("Invalid database connection type. SqlConnection is required.", nameof(dbConnection));
            }

            _sqlConnection = sqlConnection;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _sqlConnection?.Dispose();
                    _sqlConnection = null;
                }

                _disposedValue = true;
            }
        }
    }
}
