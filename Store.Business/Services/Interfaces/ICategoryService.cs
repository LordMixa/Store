using Store.Business.Models.Categories;

namespace Store.Business.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryModel>> GetAsync();
    }
}
