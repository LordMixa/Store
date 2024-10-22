namespace Store.Entities.Entities
{
    public class Author : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Biography { get; set; }
        public IEnumerable<Book> Books { get; set; } = new List<Book>();
    }
}
