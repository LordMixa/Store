using Store.Business.Models.OrderItemsModels;
using Store.Business.Models.UserModels;

namespace Store.Business.Models.OrderModels
{
    public class OrderModel
    {
        public int Id { get; set; }
        public decimal Sum { get; set; }
        public DateOnly Date { get; set; }
        //public UserModel User { get; set; }
        //public List<OrderItemsModel> OrderItems { get; set; }

    }
}
