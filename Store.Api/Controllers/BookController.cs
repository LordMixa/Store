using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Business.Models.BookModels;
using Store.Business.Services.Interfaces;
using Store.Contracts.Requests.Books;
using Store.Contracts.Responses.Books;

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
        public async Task<IActionResult> Get(int id)
        {
            var book = await _bookService.GetAsync(id);
            var bookViewModel = _mapper.Map<BookResponseModel>(book);

            return Ok(bookViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var books = await _bookService.GetAsync();
            var bookViewModels = _mapper.Map<List<BookResponseModel>>(books);

            return Ok(bookViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookCreateRequestModel request)
        {
            var bookModel = _mapper.Map<BookCreateModel>(request);
            int id = await _bookService.CreateAsync(bookModel);

            if (id == 0)
                return BadRequest(id);

            return Ok(id);
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, [FromBody] BookRequestModel request)
        //{
        //    var bookModel = _mapper.Map<BookCreateModel>(request);
        //    bookModel.Id = id;
        //    var isSuccess = await _bookService.UpdateAsync(bookModel);

        //    return Ok(isSuccess);
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var isSuccess = await _bookService.DeleteAsync(id);

        //    return Ok(isSuccess);
        //}
    }
}
