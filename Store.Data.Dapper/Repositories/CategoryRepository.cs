using Dapper;
using Store.Data.Dapper.Repositories.Interfaces;
using Store.Entities.Entities;
using System.Data;

namespace Store.Data.Dapper.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }
        public async Task<IEnumerable<Category>?> GetAsync()
        {
            var categories = await _sqlConnection.QueryAsync<Category>(
            "Procedure_GetAllCategoryNames",
            commandType: CommandType.StoredProcedure);

            return categories;
        }
    }
}
