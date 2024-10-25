using System.Reflection;

namespace Store.Business.Models.AuditLogs
{
    public class AuditLogCreateModel
    {
        public string HttpMethod { get; set; }
        public string Request { get; set; }
        public string Url { get; set; }
        public int StatusCode { get; set; }
        public string Response { get; set; }
    }
}
