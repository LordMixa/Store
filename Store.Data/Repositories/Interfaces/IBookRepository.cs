using Store.Data.Dtos;
using Store.Data.Entities;

namespace Store.Data.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<Book?> GetAsync(int id);
        Task<IEnumerable<BookDto>?> GetAsync();
        Task<int> CreateAsync(Book book);
        Task<bool> UpdateAsync(Book book);
        Task<bool> DeleteAsync(int id);
    }
}
