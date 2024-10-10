using AutoMapper;
using Store.Business.Models.ProductModels;
using Store.Business.Services.Interfaces;
using Store.Data.Repositories.Interfaces;

namespace Store.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public ProductService(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }
        public ProductModel Get(int id)
        {
            var product = _productRepository.Get(id);

            return _mapper.Map<ProductModel>(product);
        }
    }
}
