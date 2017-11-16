using Sign.Common.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Filters;
using WebApiSignService.Common;
using WebApiSignService.Models;

namespace WebApiSignService.Filter
{
    public class ApiSecurityFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext filterContext)
        {
            ResultMsg result = null;
            string signKey = string.Empty, timespan = string.Empty, nonce = string.Empty, signature = string.Empty;
            //判断请求的消息中是否包括判断参数
            var request = filterContext.Request;
            if (request.Headers.Contains("signKey"))
                signKey = request.Headers.GetValues("signKey").FirstOrDefault();
            if (request.Headers.Contains("timespan"))
                timespan = request.Headers.GetValues("timespan").FirstOrDefault();
            if (request.Headers.Contains("nonce"))
                nonce = request.Headers.GetValues("nonce").FirstOrDefault();
            if (request.Headers.Contains("signature"))
                signature = request.Headers.GetValues("signature").FirstOrDefault();

            //如果方法是GetToken,则不需要验证
            if (filterContext.ActionDescriptor.ActionName.ToLower() == "gettoken")
            {
                if (string.IsNullOrEmpty(signKey) || string.IsNullOrEmpty(timespan) || string.IsNullOrEmpty(nonce))
                {
                    result = new ResultMsg((int)ExceptionStatus.ParameterError, EnumExtension.GetEnumText(ExceptionStatus.ParameterError), null);
                    filterContext.Response = HttpResponseExtension.ToJson(result);
                    base.OnActionExecuting(filterContext);
                    return;
                }
                else
                {
                    base.OnActionExecuting(filterContext);
                    return;
                }
            }
            DbLogger.LogWriteMessage("测试参数");
            string signtoken = string.Empty;
            //判断是否包含以下参数
            if (string.IsNullOrEmpty(signKey) || string.IsNullOrEmpty(timespan) || string.IsNullOrEmpty(nonce) || string.IsNullOrEmpty(signature))
            {
                result = new ResultMsg((int)ExceptionStatus.ParameterError, EnumExtension.GetEnumText(ExceptionStatus.ParameterError), null);
                filterContext.Response = HttpResponseExtension.ToJson(result);
                base.OnActionExecuting(filterContext);
                return;
            }

            DbLogger.LogWriteMessage("测试是否在有效时间内");
            //判断是否在有效时间内
            double ts1 = 0;
            double ts2 = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
            bool timespanValidate = double.TryParse(timespan, out ts1);
            double ts = ts2 - ts1;
            bool falg = ts > int.Parse(WebConfig.UrlExpireTime) * 1000;
            if (!timespanValidate || falg)
            {
                result = new ResultMsg((int)ExceptionStatus.URLExpireError, EnumExtension.GetEnumText(ExceptionStatus.URLExpireError), null);
                filterContext.Response = HttpResponseExtension.ToJson(result);
                base.OnActionExecuting(filterContext);
                return;
            }

            DbLogger.LogWriteMessage("测试token是否有效");
            //判断token是否有效
            Token token = HttpRuntime.Cache.Get(string.Format("{0}{1}", WebConfig.signKey, signKey)) as Token;
            if (token == null)
            {
                result = new ResultMsg((int)ExceptionStatus.TokenInvalid, EnumExtension.GetEnumText(ExceptionStatus.TokenInvalid), null);
                filterContext.Response = HttpResponseExtension.ToJson(result);
                base.OnActionExecuting(filterContext);
                return;
            }
            else
                signtoken = token.signToken;

            DbLogger.LogWriteMessage("判断http调用方式");
            string data = string.Empty;
            //判断http调用方式
            string method = request.Method.Method.ToUpper();
            switch (method)
            {
                case "POST":
                    Stream stream = HttpContext.Current.Request.InputStream;
                    string responseJson = string.Empty;
                    StreamReader streamReader = new StreamReader(stream);
                    data = streamReader.ReadToEnd();
                    break;
                case "GET":
                    NameValueCollection form = HttpContext.Current.Request.QueryString;
                    //第一步：取出所有get参数
                    IDictionary<string, string> parameters = new Dictionary<string, string>();
                    for (int f = 0; f < form.Count; f++)
                    {
                        string key = form.Keys[f];
                        parameters.Add(key, form[key]);
                    }

                    // 第二步：把字典按Key的字母顺序排序
                    IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
                    IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

                    // 第三步：把所有参数名和参数值串在一起
                    StringBuilder query = new StringBuilder();
                    while (dem.MoveNext())
                    {
                        string key = dem.Current.Key;
                        string value = dem.Current.Value;
                        if (!string.IsNullOrEmpty(key))
                        {
                            query.Append(key).Append(value);
                        }
                    }
                    data = query.ToString();
                    break;
                default:
                    result = new ResultMsg((int)ExceptionStatus.HttpMehtodError, EnumExtension.GetEnumText(ExceptionStatus.HttpMehtodError), null);
                    filterContext.Response = HttpResponseExtension.ToJson(result);
                    base.OnActionExecuting(filterContext);
                    break;
            }

            DbLogger.LogWriteMessage("验证签名信息是否符合");
            //验证签名信息是否符合
            bool valida = ValidateSign.Validate(signKey, timespan, nonce, signtoken, data, signature);
            if (!valida)
            {
                result = new ResultMsg((int)ExceptionStatus.HttpRequestError, EnumExtension.GetEnumText(ExceptionStatus.HttpRequestError), null);
                filterContext.Response = HttpResponseExtension.ToJson(result);
                base.OnActionExecuting(filterContext);
                return;
            }
            else
                base.OnActionExecuting(filterContext);
        }
    }
}