using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Business.Services.Interfaces;
using Store.Contracts.Responses.Authors;

namespace Store.Api.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService, IMapper mapper)
        {
            _authorService = authorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var books = await _authorService.GetAsync();
            var bookResponseModels = _mapper.Map<List<AuthorNameResponseModel>>(books);

            return Ok(bookResponseModels);
        }
    }
}
