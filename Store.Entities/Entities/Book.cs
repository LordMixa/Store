namespace Store.Entities.Entities
{
    public class Book : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime DateOfPublication { get; set; }
        public IList<Category> Categories { get; set; } = new List<Category>();
        public IList<Author> Authors { get; set; } = new List<Author>();
    }
}
