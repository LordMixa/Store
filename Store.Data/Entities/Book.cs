namespace Store.Data.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateOnly DateOfPublication { get; set; }
        public IList<Category> Categories { get; set; } = new List<Category>();
        public IList<Author> Authors { get; set; } = new List<Author>();
    }
}
