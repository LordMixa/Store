using Store.ViewModels.ViewModels.BookViewModels;
using Store.ViewModels.ViewModels.OrderViewModels;

namespace Store.ViewModels.ViewModels.OrderItemsViewModels
{
    public record OrderItemsViewModel
    {
        public int Id { get; init; }
        public int Amount { get; init; }
        public decimal Price { get; init; }
        public OrderViewModel Order { get; init; }
        public BookViewModel Book { get; init; }
    }
}
