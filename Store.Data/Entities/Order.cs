namespace Store.Data.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public decimal Sum { get; set; }
        public DateOnly Date {  get; set; }
        public User User { get; set; }
        public IEnumerable<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
    }
}
