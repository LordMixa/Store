using Store.Data.Dtos;
using Store.Data.Entities;

namespace Store.Data.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetAsync(int id);
        Task<IEnumerable<OrderDto>> GetAsync();
        Task<int> CreateAsync(Order order);
        Task<bool> UpdateAsync(Order order);
        Task<bool> DeleteAsync(int id);
    }
}
