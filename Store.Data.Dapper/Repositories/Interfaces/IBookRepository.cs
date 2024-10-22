using Store.Entities.Dtos;
using Store.Entities.Entities;

namespace Store.Data.Dapper.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<Book?> GetAsync(int id);
        Task<IEnumerable<BookDto>?> GetAsync();
        Task<int> CreateAsync(Book book);
    }
}
