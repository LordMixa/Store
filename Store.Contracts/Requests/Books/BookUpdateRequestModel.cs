namespace Store.Contracts.Requests.Books
{
    public record BookUpdateRequestModel
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public decimal Price { get; init; }
        public DateTime DateOfPublication { get; init; }
        public List<int> AuthorIds { get; init; }
        public List<int> CategoryIds { get; init; }
    }
}
