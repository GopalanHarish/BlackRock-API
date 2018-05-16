using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace BlackRockAPI.Helpers
{
    public class ResponseMessage<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }

    public class DataTableResponseMessage<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public long draw { get; set; }
        public long recordsTotal { get; set; }
        public long recordsFiltered { get; set; }
        public T Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}