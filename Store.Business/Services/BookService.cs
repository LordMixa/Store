using AutoMapper;
using Store.Business.Models.BookModels;
using Store.Business.Services.Interfaces;
using Store.Data.Entities;
using Store.Data.Repositories.Interfaces;

namespace Store.Business.Services
{
    public class BookService : IBookService
    {
        private readonly IMapper _mapper;
        private readonly IBookRepository _bookRepository;

        public BookService(IMapper mapper, IBookRepository bookRepository)
        {
            _mapper = mapper;
            _bookRepository = bookRepository;
        }
        public async Task<BookModel> GetAsync(int id)
        {
            var book = await _bookRepository.GetAsync(id);
            var bookModel = _mapper.Map<BookModel>(book);

            return bookModel;
        }
        public async Task<IEnumerable<BookModel>> GetAsync()
        {
            var books = await _bookRepository.GetAsync();
            var bookModels = _mapper.Map<List<BookModel>>(books);

            return bookModels;
        }
        public async Task<int> CreateAsync(BookCreateModel bookModel)
        {
            var book = _mapper.Map<Book>(bookModel);
            var id = await _bookRepository.CreateAsync(book);

            return id;
        }
        public async Task<bool> UpdateAsync(BookCreateModel bookModel)
        {
            var book = _mapper.Map<Book>(bookModel);
            var isSuccess = await _bookRepository.UpdateAsync(book);

            return isSuccess;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var isSuccess = await _bookRepository.DeleteAsync(id);

            return isSuccess;
        }
    }
}
