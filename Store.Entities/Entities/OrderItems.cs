namespace Store.Entities.Entities
{
    public class OrderItems
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public Order Order { get; set; }
        public Book Book { get; set; }
    }
}
