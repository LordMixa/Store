using Store.ViewModels.ViewModels.OrderItemsViewModels;
using Store.ViewModels.ViewModels.UserViewModels;

namespace Store.ViewModels.ViewModels.OrderViewModels
{
    public record OrderViewModel
    {
        public int Id { get; init; }
        public decimal Sum { get; init; }
        public DateOnly Date { get; init; }
        public UserViewModel User { get; init; }
        public IEnumerable<OrderItemsViewModel> OrderItems { get; init; }

    }
}
