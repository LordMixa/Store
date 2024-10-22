namespace Store.Contracts.Responses.Books
{
    public record BookTitleResponseModel
    {
        public int Id { get; init; }
        public string Title { get; init; }
    }
}
