using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Api.Models.Requests;
using Store.Business.Models.BookModels;
using Store.Business.Services.Interfaces;
using Store.ViewModels.ViewModels.BookViewModels;

namespace Store.Api.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IBookService _bookService;

        public BookController(IBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<BookViewModel> Get(int id)
        {
            var book = await _bookService.GetAsync(id);
            var bookViewModel = _mapper.Map<BookViewModel>(book);

            return bookViewModel;

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookViewModel>>> Get()
        {
            var books = await _bookService.GetAsync();
            var bookViewModels = _mapper.Map<List<BookViewModel>>(books);

            return bookViewModels;

        }
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] BookRequestModel request)
        {
            var bookModel = _mapper.Map<BookCreateModel>(request);
            int id = await _bookService.CreateAsync(bookModel);

            return id;

        }
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(int id, [FromBody] BookRequestModel request)
        {
            var bookModel = _mapper.Map<BookCreateModel>(request);
            var isSuccess = await _bookService.UpdateAsync(bookModel, id);

            return isSuccess;

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var isSuccess = await _bookService.DeleteAsync(id);

            return isSuccess;
        }
    }
}
