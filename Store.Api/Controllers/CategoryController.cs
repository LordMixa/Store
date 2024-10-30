using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Business.Services.Interfaces;
using Store.Contracts.Responses.Categories;

namespace Store.Api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var books = await _categoryService.GetAsync();
            var bookResponseModels = _mapper.Map<List<CategoryResponseModel>>(books);

            return Ok(bookResponseModels);
        }
    }
}
