using Store.Business.Models.ProductModels;

namespace Store.Business.Services.Interfaces
{
    public interface IProductService
    {
        ProductModel Get(int id);
    }
}
