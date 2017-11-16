using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebApiSignService.Common;
using WebApiSignService.Models;

namespace WebApiSignService.Controllers
{
    public class TokenController : ApiController
    {

        [HttpGet]
        public IHttpActionResult GetToken(string signKey)
        {
            if (string.IsNullOrEmpty(signKey))
                return Json<ResultMsg>(new ResultMsg((int)ExceptionStatus.ParameterError, EnumExtension.GetEnumText(ExceptionStatus.ParameterError), null));
            //根据签名ID获取缓存token
            string strKey = string.Format("{0}{1}", WebConfig.signKey, signKey);
            Token cacheData = HttpRuntime.Cache.Get(strKey) as Token;
            if (cacheData == null)
            {
                cacheData = new Token();
                cacheData.signId = signKey;
                cacheData.timespan = DateTime.Now.AddDays(1);
                cacheData.signToken = Guid.NewGuid().ToString("N");
                //插入缓存，缓存时间为1天
                HttpRuntime.Cache.Insert(strKey, cacheData, null, cacheData.timespan, TimeSpan.Zero);
            }
            //返回token信息
            return Json<ResultMsg>(new ResultMsg((int)ExceptionStatus.OK, EnumExtension.GetEnumText(ExceptionStatus.OK), cacheData));
        }

        [HttpGet]
        public IHttpActionResult GetTest()
        {
            return Json<ResultMsg>(new ResultMsg((int)ExceptionStatus.OK, EnumExtension.GetEnumText(ExceptionStatus.OK), "这是测试接口"));
        }
    }
}
