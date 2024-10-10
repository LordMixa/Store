using Store.Data.Entities;
using Store.Data.Repositories.Interfaces;

namespace Store.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public Product Get(int id)
        {
            return new Product();
        }
    }
}
