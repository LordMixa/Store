namespace Store.Contracts.Requests.Orders
{
    public record OrderCreateRequestModel
    {
        public decimal Sum { get; init; }
        public DateOnly Date { get; init; }
        public int UserId { get; init; }
        public IEnumerable<int> OrderItemsIds { get; init; }
    }
}
