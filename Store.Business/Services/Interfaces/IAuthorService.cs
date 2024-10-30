using Store.Business.Models.Authors;

namespace Store.Business.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorModel>> GetAsync();
    }
}
