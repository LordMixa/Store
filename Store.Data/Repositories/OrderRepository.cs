using Store.Data.Entities;
using Store.Data.Repositories.Interfaces;

namespace Store.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        public Order Get(int id)
        {
            return new Order();
        }
    }
}
