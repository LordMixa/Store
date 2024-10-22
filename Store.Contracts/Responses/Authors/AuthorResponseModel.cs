namespace Store.Contracts.Responses.Authors
{
    public record AuthorResponseModel
    {
        public int Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Biography { get; init; }
    }
}
