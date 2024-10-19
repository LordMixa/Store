using Store.ViewModels.ViewModels.AuthorViewModels;
using Store.ViewModels.ViewModels.CategoryViewModels;

namespace Store.ViewModels.ViewModels.BookViewModels
{
    public record BookViewModel
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public decimal Price { get; init; }
        public DateTime DateOfPublication { get; init; }
        public IEnumerable<AuthorViewModel> Authors { get; init; }
        public IEnumerable<CategoryViewModel> Categories { get; init; }

    }
}
