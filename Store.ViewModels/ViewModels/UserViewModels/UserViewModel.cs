namespace Store.ViewModels.ViewModels.UserViewModels
{
    public record UserViewModel
    {
        public int Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
    }
}
