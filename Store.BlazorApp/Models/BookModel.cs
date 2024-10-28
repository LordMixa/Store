namespace Store.BlazorApp.Models
{
    public class BookModel
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public decimal Price { get; init; }
        public DateTime DateOfPublication { get; init; }
        public IEnumerable<AuthorModel> Authors { get; init; } = new List<AuthorModel>();
        public IEnumerable<CategoryModel> Categories { get; init; } = new List<CategoryModel>();
    }
}
