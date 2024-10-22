using Store.Contracts.Responses.OrderItems;
using Store.Contracts.Responses.Users;

namespace Store.Contracts.Responses.Orders
{
    public record OrderResponseModel
    {
        public int Id { get; init; }
        public decimal Sum { get; init; }
        public DateOnly Date { get; init; }
        public UserResponseModel User { get; init; }
        public IEnumerable<OrderItemsResponseModel> OrderItems { get; init; } = new List<OrderItemsResponseModel>();

    }
}
