using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sign.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using TestAli.OSS.Models;

namespace TestAli.OSS.Utils
{
    public class HttpRequestHelper
    {
        public static void Post(string url,string paras,string signId)
        {
            byte[] bytes=Encoding.UTF8.GetBytes(paras);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentLength = bytes.Length;
            webRequest.ContentType = "application/json";
            //加入头信息
            webRequest.Headers.Add("signId", signId);
            webRequest.Headers.Add("timespan", GetTimespan());
            webRequest.Headers.Add("nonce", GetRandom(10));
            webRequest.Headers.Add("signature", "");
        }

        public static T Get<T>(string url, string paras, string signId,bool isSign=true)
        {
            HttpWebRequest webrequest = null;
            HttpWebResponse webresponse = null;
            string strResult = string.Empty;
            try
            {
                webrequest = (HttpWebRequest)WebRequest.Create(url + "?" + paras);
                webrequest.Method = "GET";
                webrequest.ContentType = "application/json";
                webrequest.Timeout = 90000;
                //加入头信息
                string timespan = GetTimespan();
                string ran = GetRandom(10);
                webrequest.Headers.Add("signKey", signId);
                DbLogger.LogWriteMessage("signKey:" + signId);
                webrequest.Headers.Add("timespan", timespan);
                DbLogger.LogWriteMessage("timespan:" + timespan);
                webrequest.Headers.Add("nonce", ran);
                DbLogger.LogWriteMessage("nonce:" + ran);
                if (isSign)
                {
                    string strSign = GetSignature(signId, timespan, ran, paras);
                    webrequest.Headers.Add("signature", strSign);
                    DbLogger.LogWriteMessage("signature:" + strSign);
                }
                webresponse = (HttpWebResponse)webrequest.GetResponse();
                Stream stream = webresponse.GetResponseStream();
                StreamReader sr = new StreamReader(stream, Encoding.UTF8);
                strResult = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                return JsonConvert.DeserializeObject<T>(ex.Message);
            }
            finally
            {
                if (webresponse != null)
                    webresponse.Close();
                if (webrequest != null)
                    webrequest.Abort();
            }
            return JsonConvert.DeserializeObject<T>(strResult);
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimespan()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            return ts.TotalMilliseconds.ToString();
        }

        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandom(int length)
        {
            StringBuilder str = new StringBuilder();
            for(int i=0;i<length;i++)
            {
                var ran = new Random(Guid.NewGuid().GetHashCode());
                str.Append(ran.Next(0, 10));
            }
            return str.ToString();
        }

        public static T GetToken<T>()
        {
            string signKey = AppConfig.SignKey;
            var result = Get<T>(AppConfig.TokenUrl, "signKey=" + signKey, signKey, false);
            return result;
        }

        public static string GetSignature(string signKey, string timespan, string nonce, string data)
        {
            string signToken = string.Empty;
            var result = GetToken<JObject>();
            if (result != null)
            {
                if (result["code"].ToString() == "200")
                {
                    var tokena = JsonConvert.DeserializeObject<JObject>(result["result"].ToString());
                    if (tokena != null)
                        signToken = tokena["signToken"].ToString();
                }
            }

            var hash = MD5.Create();
            string str = signKey + timespan + nonce + signToken + data;
            byte[] bytes = Encoding.UTF8.GetBytes(string.Concat(str.OrderBy(c => c)));
            DbLogger.LogWriteMessage("str内容:" + string.Concat(str.OrderBy(c => c)));
            //使用MD5加密
            var md5Val = hash.ComputeHash(bytes);
            //把二进制转化为大写的十六进制
            StringBuilder strSign = new StringBuilder();
            foreach (var val in md5Val)
            {
                strSign.Append(val.ToString("X2"));
            }
            return strSign.ToString();
        }
    }
}
