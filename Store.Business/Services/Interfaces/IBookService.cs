using Store.Business.Models.BookModels;

namespace Store.Business.Services.Interfaces
{
    public interface IBookService
    {
        Task<BookModel> GetAsync(int id);
        Task<IEnumerable<BookModel>> GetAsync();
        Task<int> CreateAsync(BookCreateModel bookModel);
        Task<bool> UpdateAsync(BookCreateModel bookModel, int id);
        Task<bool> DeleteAsync(int id);
    }
}
