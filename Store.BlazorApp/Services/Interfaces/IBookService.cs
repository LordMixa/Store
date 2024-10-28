using Store.BlazorApp.Models;

namespace Store.BlazorApp.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookModel>> GetBooksAsync();
    }
}
