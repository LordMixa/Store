using Store.Entities.Entities;

namespace Store.Data.Dapper.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>?> GetAsync();
    }
}
