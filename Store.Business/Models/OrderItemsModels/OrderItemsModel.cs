using Store.Business.Models.BookModels;
using Store.Business.Models.OrderModels;

namespace Store.Business.Models.OrderItemsModels
{
    public class OrderItemsModel
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public OrderModel Order { get; set; }
        public BookModel Book { get; set; }
    }
}
