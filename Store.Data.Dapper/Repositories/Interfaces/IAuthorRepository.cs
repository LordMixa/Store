using Store.Entities.Entities;

namespace Store.Data.Dapper.Repositories.Interfaces
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>?> GetAsync();
    }
}
