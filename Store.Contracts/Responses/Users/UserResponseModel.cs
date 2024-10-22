namespace Store.Contracts.Responses.Users
{
    public record UserResponseModel
    {
        public int Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
    }
}
