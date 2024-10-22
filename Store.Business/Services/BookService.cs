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
            var bookDtos = await _bookRepository.GetAsync();
            var bookModels = DtoMapToModel(bookDtos);

            return bookModels;
        }

        public async Task<int> CreateAsync(BookCreateModel bookModel)
        {

            bool isAuthorsValid = ValidateAuthorIds(bookModel.Authors);
            if (!isAuthorsValid)
                throw new();

            bool isCategoriesValid = ValidateCategoryIds(bookModel.Categories);
            if (!isCategoriesValid)
                throw new();

            var book = _mapper.Map<Book>(bookModel);

            var id = await _bookRepository.CreateAsync(book);

            return id;
        }

        private bool ValidateAuthorIds(IEnumerable<AuthorModel> authors)
        {
            if(authors.Any(a => a.Id <=0 ))
                return false;

            return true;
        }

        private bool ValidateCategoryIds(IEnumerable<CategoryModel> categories)
        {
            if (categories.Any(a => a.Id <= 0))
                return false;

            return true;
        }

        //public async Task<bool> UpdateAsync(BookCreateModel bookModel)
        //{
        //    var book = _mapper.Map<Book>(bookModel);
        //    var isSuccess = await _bookRepository.UpdateAsync(book);

        //    return isSuccess;
        //}

        //public async Task<bool> DeleteAsync(int id)
        //{
        //    var isSuccess = await _bookRepository.DeleteAsync(id);

        //    return isSuccess;
        //}

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
