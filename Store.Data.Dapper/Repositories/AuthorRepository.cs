using Dapper;
using Store.Data.Dapper.Repositories.Interfaces;
using Store.Entities.Entities;
using System.Data;

namespace Store.Data.Dapper.Repositories
{
    public class AuthorRepository : BaseRepository, IAuthorRepository
    {
        public AuthorRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }
        public async Task<IEnumerable<Author>?> GetAsync()
        {
            var authors = await _sqlConnection.QueryAsync<Author>(
            "Procedure_GetAllAuthorNames",
            commandType: CommandType.StoredProcedure);

            return authors;
        }
    }
}
