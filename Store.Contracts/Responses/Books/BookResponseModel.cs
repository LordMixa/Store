using Store.Contracts.Responses.Authors;
using Store.Contracts.Responses.Categories;

namespace Store.Contracts.Responses.Books
{
    public record BookResponseModel
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public decimal Price { get; init; }
        public DateTime DateOfPublication { get; init; }
        public IEnumerable<AuthorResponseModel> Authors { get; init; } = new List<AuthorResponseModel>();
        public IEnumerable<CategoryResponseModel> Categories { get; init; } = new List<CategoryResponseModel>();

    }
}
