using Store.Business.Models.OrderItems;
using Store.Business.Models.Users;

namespace Store.Business.Models.Orders
{
    public class OrderModel
    {
        public int Id { get; set; }
        public decimal Sum { get; set; }
        public DateOnly Date { get; set; }
        public UserModel User { get; set; }
        public IEnumerable<OrderItemsModel> OrderItems { get; set; } = new List<OrderItemsModel>();

    }
}
