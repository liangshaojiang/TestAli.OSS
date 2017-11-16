using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace WebApiSignService.Common
{
    public class HttpResponseExtension
    {
        public static HttpResponseMessage ToJson(object obj)
        {
            string str=string.Empty;
            if (obj is string || obj is char)
                str = obj.ToString();
            else
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                str = serializer.Serialize(obj);
            }
            return new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
        }
    }
}