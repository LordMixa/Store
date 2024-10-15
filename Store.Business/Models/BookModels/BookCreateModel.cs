namespace Store.Business.Models.BookModels
{
    public class BookCreateModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateOnly DateOfPublication { get; set; }
    }
}
