using Store.Business.Models.BookModels;

namespace Store.Business.Models.BookDetails
{ 
    public record BookDetailsModel
    {
        public int Id { get; init; }
        public int NumberOfPages { get; init; }
        public string Language { get; init; }
        public BookModel Book { get; init; }
        public string PublishingHouse { get; init; }
    }
}
