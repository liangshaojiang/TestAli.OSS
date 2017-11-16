using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApiSignService.Common;

namespace WebApiSignService.Models
{
    public enum ExceptionStatus
    {
        [Text("请求或处理成功！")]
        OK = 200,

        [Text("出现内部错误！")]
        Error = 500,

        [Text("未授权标识！")]
        Unauthorized = 401,

        [Text("请求的参数不合法！")]
        ParameterError = 400,

        [Text("Token验证失败！")]
        TokenInvalid = 403,

        [Text("HTTP请求类型不合法！")]
        HttpMehtodError = 405,

        [Text("HTTP请求不合法,请求参数可能被篡改")]
        HttpRequestError = 406,

        [Text("该URL已经失效")]
        URLExpireError = 407
    }
}