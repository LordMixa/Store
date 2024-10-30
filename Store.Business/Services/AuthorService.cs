using AutoMapper;
using Store.Business.Models.Authors;
using Store.Business.Services.Interfaces;
using Store.Data.Dapper.Repositories.Interfaces;

namespace Store.Business.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IMapper _mapper;
        private readonly IAuthorRepository _authorRepository;
        private readonly IRetryPolicyService _retryPolicyService;

        public AuthorService(IMapper mapper, IAuthorRepository authorRepositpry, IRetryPolicyService retryPolicyService)
        {
            _mapper = mapper;
            _authorRepository = authorRepositpry;
            _retryPolicyService = retryPolicyService;
        }

        public async Task<IEnumerable<AuthorModel>> GetAsync()
        {
            var authors = await _retryPolicyService.ExecuteAsync(_authorRepository.GetAsync);

            var authorModels = _mapper.Map<IEnumerable<AuthorModel>>(authors);

            return authorModels;
        }
    }
}
