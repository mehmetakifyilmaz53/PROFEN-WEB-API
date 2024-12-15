namespace Pro_Web_API.Core.Entities
{
    public class ApiLog
    {
        public int Id { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string HttpMethod { get; set; }
        public string RequestPath { get; set; }
        public DateTime LogTime { get; set; } = DateTime.Now;
        public string Message { get; set; }
        public string IpAdresss { get; set; }
        public string Response { get; set; } 
    }


}
