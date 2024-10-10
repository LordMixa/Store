using Store.Data.Entities;

namespace Store.Data.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Product Get(int id);
    }
}
