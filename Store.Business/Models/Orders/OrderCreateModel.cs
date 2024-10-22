namespace Store.Business.Models.Orders
{
    public class OrderCreateModel
    {
        public int Id { get; set; }
        public decimal Sum { get; set; }
        public DateOnly Date { get; set; }
        public int UserId { get; set; }
        public IEnumerable<int> OrderItemsIds { get; set; }
    }
}
