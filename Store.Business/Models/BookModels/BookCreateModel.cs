using Store.Business.Models.AuthorModels;
using Store.Business.Models.CategoryModels;

namespace Store.Business.Models.BookModels
{
    public class BookCreateModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime DateOfPublication { get; set; }
        public IEnumerable<AuthorModel> Authors { get; set; }
        public IEnumerable<CategoryModel> Categories { get; set; }

    }
}
