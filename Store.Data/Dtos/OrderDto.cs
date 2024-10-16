namespace Store.Data.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal Sum { get; set; }
        public DateOnly Date { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int OrderItemsId { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; }
    }
}
