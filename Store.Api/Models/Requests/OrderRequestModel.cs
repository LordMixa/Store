namespace Store.Api.Models.Requests
{
    public record OrderRequestModel
    {
        public decimal Sum { get; init; }
        public DateOnly Date { get; init; }
        public int UserId { get; init; }
        public List<int> OrderItemsIds { get; init; }

    }
}
