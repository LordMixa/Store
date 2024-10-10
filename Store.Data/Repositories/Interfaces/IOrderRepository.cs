using Store.Data.Entities;

namespace Store.Data.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Order Get(int id);
    }
}
