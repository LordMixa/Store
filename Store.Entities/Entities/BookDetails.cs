namespace Store.Entities.Entities
{
    public class BookDetails
    {
        public int Id { get; set; }
        public int NumberOfPages { get; set; }
        public string Language { get; set; }
        public Book Book { get; set; }
        public string PublishingHouse { get; set; }
    }
}
