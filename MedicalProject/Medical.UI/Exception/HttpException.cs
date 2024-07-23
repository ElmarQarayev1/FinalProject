using System;
using System.Net;

namespace Medical.UI.Exception
{
    public class HttpException : System.Exception
    {
        public HttpStatusCode Status { get; set; }

        public HttpException(HttpStatusCode status)
        {
            this.Status = status;
        }

        public HttpException(HttpStatusCode status, string message) : base(message)
        {
            this.Status = status;
        }
    }
}

