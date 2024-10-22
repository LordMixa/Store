using Store.Business.Models.Orders;

namespace Store.Business.Services.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateAsync(OrderCreateModel orderModel);
        Task<bool> DeleteAsync(int id);
        Task<OrderModel> GetAsync(int id);
        Task<IEnumerable<OrderModel>> GetAsync();
        Task<bool> UpdateAsync(OrderCreateModel orderModel);
    }
}
