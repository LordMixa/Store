namespace Store.Entities.Entities
{
    public class AuditLog : BaseEntity
    {
        public string HttpMethod { get; set; }
        public string Request { get; set; }
        public string Url { get; set; }
        public int StatusCode { get; set; }
        public string Response { get; set; }
    }
}
