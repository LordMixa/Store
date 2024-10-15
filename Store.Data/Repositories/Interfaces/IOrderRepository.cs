using Store.Data.Entities;

namespace Store.Data.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetAsync(int id);
        Task<IEnumerable<Order>> GetAsync();
        Task<int> CreateAsync(Order order);
        Task<bool> UpdateAsync(Order order, int id);
        Task<bool> DeleteAsync(int id);
    }
}
