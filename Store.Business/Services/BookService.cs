using AutoMapper;
using Store.Business.Models.Authors;
using Store.Business.Models.BookModels;
using Store.Business.Models.Categories;
using Store.Business.Services.Interfaces;
using Store.Data.Dapper.Repositories.Interfaces;
using Store.Entities.Dtos;
using Store.Entities.Entities;

namespace Store.Business.Services
{
    public class BookService : IBookService
    {
        private readonly IMapper _mapper;
        private readonly IBookRepository _bookRepository;
        private readonly IRetryPolicyService _retryPolicyService;

        public BookService(IMapper mapper, IBookRepository bookRepository, IRetryPolicyService retryPolicyService)
        {
            _mapper = mapper;
            _bookRepository = bookRepository;
            _retryPolicyService = retryPolicyService;
        }

        public async Task<BookModel> GetAsync(int id)
        {
            var book = await _retryPolicyService.ExecuteAsync(() => _bookRepository.GetAsync(id));

            var bookModel = _mapper.Map<BookModel>(book);

            return bookModel;
        }

        public async Task<IEnumerable<BookModel>> GetAsync()
        {
            var bookDtos = await _retryPolicyService.ExecuteAsync(_bookRepository.GetAsync);

            var bookModels = DtoMapToModel(bookDtos);

            return bookModels;
        }

        public async Task<int> CreateAsync(BookCreateModel bookModel)
        {
            ValidateBook(bookModel.Authors, bookModel.Categories);

            var book = _mapper.Map<Book>(bookModel);

            var id = await _retryPolicyService.ExecuteAsync(() => _bookRepository.CreateAsync(book));

            return id;
        }

        public async Task<bool> UpdateAsync(BookUpdateModel bookModel)
        {
            ValidateBook(bookModel.Authors, bookModel.Categories);

            var book = _mapper.Map<Book>(bookModel);

            var isSucceeded = await _retryPolicyService.ExecuteAsync(() => _bookRepository.UpdateAsync(book));

            return isSucceeded;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var isSucceeded = await _retryPolicyService.ExecuteAsync(() => _bookRepository.DeleteAsync(id));

            return isSucceeded;
        }

        private void ValidateBook(IEnumerable<AuthorModel> authorModels, IEnumerable<CategoryModel> categoryModels)
        {
            var authorInts = authorModels.Select(a => a.Id);

            bool isAuthorsValid = ValidateIds(authorInts);
            if (!isAuthorsValid)
                throw new Exception($"Parameter {typeof(Author)} {nameof(Author.Id)} is not valid");

            var categoryInts = categoryModels.Select(c => c.Id);

            bool isCategoriesValid = ValidateIds(categoryInts);
            if (!isCategoriesValid)
                throw new Exception($"Parameter {typeof(Category)} {nameof(Category.Id)} is not valid");
        }

        private bool ValidateIds(IEnumerable<int> ids)
        {
            if (ids.Any(id => id <= 0))
                return false;

            return true;
        }

        private IEnumerable<BookModel> DtoMapToModel(IEnumerable<BookDto> bookDtos)
        {
            var books = bookDtos
                .GroupBy(dto => dto.Id)
                .Select(group =>
                {
                    var firstDto = group.FirstOrDefault();

                    var bookModel = new BookModel
                    {
                        Id = group.Key,
                        Title = firstDto.Title,
                        DateOfPublication = firstDto.DateOfPublication,
                        Description = firstDto.Description,
                        Price = firstDto.Price,
                        Authors = group
                            .Select(dto => new AuthorModel
                            {
                                Id = dto.AuthorId,
                                FirstName = dto.FirstName,
                                LastName = dto.LastName,
                                Biography = dto.Biography
                            })
                            .DistinctBy(author => author.Id)
                            .ToList(),
                        Categories = group
                            .Select(dto => new CategoryModel
                            {
                                Id = dto.CategoryId,
                                Name = dto.Name
                            })
                            .DistinctBy(category => category.Id)
                            .ToList()
                    };

                    return bookModel;
                });

            return books;
        }
    }
}
