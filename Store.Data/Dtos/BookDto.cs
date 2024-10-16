namespace Store.Data.Dtos
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateOnly DateOfPublication { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Biography { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
