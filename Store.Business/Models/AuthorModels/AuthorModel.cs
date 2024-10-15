using Store.Business.Models.BookModels;

namespace Store.Business.Models.AuthorModels
{
    public class AuthorModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Biography { get; set; }
        public IEnumerable<BookModel> Books { get; set; }
    }
}
