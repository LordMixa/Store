namespace Store.Business.Models.OrderModels
{
    public class OrderCreateModel
    {
        public int Id { get; set; }
        public decimal Sum { get; set; }
        public DateOnly Date { get; set; }
        public int UserId { get; set; }
        public List<int> OrderItemsIds { get; set; }
    }
}
