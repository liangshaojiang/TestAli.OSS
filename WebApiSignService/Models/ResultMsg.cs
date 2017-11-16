using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiSignService.Models
{
    public class ResultMsg
    {
        public ResultMsg(int code, string message, dynamic result)
        {
            this.code = code;
            this.message = message;
            this.result = result;
        }

        public int code { get; set; }

        public string message { get; set; }

        public dynamic result { get; set; }
    }

    public class Token
    {
        public string signId { get; set; }

        public DateTime timespan { get; set; }

        public string signToken { get; set; }
    }
}