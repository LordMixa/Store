using Store.Contracts.Responses.Books;

namespace Store.Contracts.Responses.OrderItems
{
    public record OrderItemsResponseModel
    {
        public int Id { get; init; }
        public int Amount { get; init; }
        public decimal Price { get; init; }
        public BookTitleResponseModel Book { get; init; }
    }
}
