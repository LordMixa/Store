namespace Store.Data.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateOnly DateOfPublication { get; set; }
        public List<Category> Categories { get; set; }
        public List<Author> Authors { get; set; }
    }
}
