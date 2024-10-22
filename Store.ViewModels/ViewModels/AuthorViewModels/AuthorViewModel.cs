namespace Store.ViewModels.ViewModels.AuthorViewModels
{
    public record AuthorViewModel
    {
        public int Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Biography { get; init; }
    }
}
