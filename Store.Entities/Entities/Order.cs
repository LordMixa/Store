namespace Store.Entities.Entities
{
    public class Order : BaseEntity
    {
        public decimal Sum { get; set; }
        public DateOnly Date {  get; set; }
        public User User { get; set; }
        public IEnumerable<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
    }
}
