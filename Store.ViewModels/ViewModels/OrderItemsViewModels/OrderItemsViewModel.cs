using Store.ViewModels.ViewModels.BookViewModels;

namespace Store.ViewModels.ViewModels.OrderItemsViewModels
{
    public record OrderItemsViewModel
    {
        public int Id { get; init; }
        public int Amount { get; init; }
        public decimal Price { get; init; }
        public BookTitleViewModel Book { get; init; }
    }
}
