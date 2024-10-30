using AutoMapper;
using Store.Business.Models.Categories;
using Store.Business.Services.Interfaces;
using Store.Data.Dapper.Repositories.Interfaces;

namespace Store.Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IRetryPolicyService _retryPolicyService;

        public CategoryService(IMapper mapper, ICategoryRepository categoryRepository, IRetryPolicyService retryPolicyService)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _retryPolicyService = retryPolicyService;
        }

        public async Task<IEnumerable<CategoryModel>> GetAsync()
        {
            var categories = await _retryPolicyService.ExecuteAsync(_categoryRepository.GetAsync);

            var categoryModels = _mapper.Map<IEnumerable<CategoryModel>>(categories);

            return categoryModels;
        }
    }
}
