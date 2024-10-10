using Store.Business.Models.OrderModels;

namespace Store.Business.Services.Interfaces
{
    public interface IOrderService
    {
        OrderModel Get(int id);
    }
}
