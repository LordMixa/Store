using Store.Business.Models.BookModels;
using Store.Business.Models.OrderModels;

namespace Store.Business.Models.OrderItemsModels
{
    public record OrderItemsModel
    {
        public int Id { get; init; }
        public int Amount { get; init; }
        public decimal Price { get; init; }
        public OrderModel Order { get; init; }
        public BookModel Book { get; init; }
    }
}
