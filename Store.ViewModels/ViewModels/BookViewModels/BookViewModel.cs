namespace Store.ViewModels.ViewModels.BookViewModels
{
    public record BookViewModel
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public decimal Price { get; init; }
        public DateOnly DateOfPublication { get; init; }
    }
}
