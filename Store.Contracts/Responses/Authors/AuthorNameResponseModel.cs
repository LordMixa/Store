namespace Store.Contracts.Responses.Authors
{
    public record AuthorNameResponseModel
    {
        public int Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }

    }
}
