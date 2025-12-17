using System.Net;

namespace Voyage.Models
{
    public class Response
    {
        public Response() 
        { 
            ErrorMessages = new List<string>();
            Data = new object();
        }


        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;
        public List<string> ErrorMessages { get; set; }
        public object Data { get; set; } 
        public string? RedirectURL { get; set; }
    }
}
