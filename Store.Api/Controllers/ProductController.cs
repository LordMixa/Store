using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Business.Services.Interfaces;
using Store.ViewModels.ViewModels.ProductViewModels;

namespace Store.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public ProductViewModel Get(int id)
        {
            var product = _productService.Get(id);

            return _mapper.Map<ProductViewModel>(product);

        }
    }
}
