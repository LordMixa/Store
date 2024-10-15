using Store.Business.Models.BookModels;

namespace Store.Business.Models.BookDetailsModels
{
    public class BookDetailsModel
    {
        public int Id { get; set; }
        public int NumberOfPages { get; set; }
        public string Language { get; set; }
        public BookModel Book { get; set; }
        public string PublishingHouse { get; set; }
    }
}
