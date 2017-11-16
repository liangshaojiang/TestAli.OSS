using Sign.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebApiSignService.Common
{
    public class ValidateSign
    {
        public static bool Validate(string signId, string timespan, string nonce, string token, string data, string signature)
        {
            string strsign = signId + timespan + nonce + token + data;
            //创建MD5加密实例
            var md5 = MD5.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(string.Concat(strsign.OrderBy(c => c)));
            //使用MD5加密
            var md5val = md5.ComputeHash(bytes);
            //把二进制转化为大写的十六进制
            StringBuilder result = new StringBuilder();
            foreach (var c in md5val)
            {
                result.Append(c.ToString("X2"));
            }
            try
            {
                DbLogger.LogWriteMessage("new:" + result.ToString().ToUpper() + " compare:" + signature);
            }
            catch
            {

            }
            return result.ToString().ToUpper() == signature;
        }
    }
}