using Store.Data.Entities;

namespace Store.Data.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<Book> GetAsync(int id);
        Task<IEnumerable<Book>> GetAsync();
        Task<int> CreateAsync(Book book);
        Task<bool> UpdateAsync(Book book, int id);
        Task<bool> DeleteAsync(int id);
    }
}
