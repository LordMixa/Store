namespace Store.Business.Models.UserModels
{
    public record UserModel
    {
        public int Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
    }
}
