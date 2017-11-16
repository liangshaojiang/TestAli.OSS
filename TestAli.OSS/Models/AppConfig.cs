using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TestAli.OSS.Models
{
    public class AppConfig
    {
        private static string tokenUrl;

        public static string TokenUrl
        {
            get
            {
                if (string.IsNullOrEmpty(tokenUrl))
                    tokenUrl = ConfigurationManager.AppSettings["TokenUrl"];
                return tokenUrl;
            }
        }

        private static string signKey;

        public static string SignKey
        {
            get
            {
                if (string.IsNullOrEmpty(signKey))
                    signKey = ConfigurationManager.AppSettings["SignKey"];
                return signKey;
            }
        }
    }
}
