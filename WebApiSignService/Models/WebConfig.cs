using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebApiSignService.Models
{
    public class WebConfig
    {
        public static string signKey = "sign_key_";

        private static string urlExpireTime;

        public static string UrlExpireTime
        {
            get
            {
                if (string.IsNullOrEmpty(urlExpireTime))
                    urlExpireTime = ConfigurationManager.AppSettings["UrlExpireTime"];
                return urlExpireTime;
            }
        }
    }
}