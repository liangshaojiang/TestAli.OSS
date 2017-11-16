using Aliyun.OSS;
using Aliyun.OSS.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TestAli.OSS.Models;
using TestAli.OSS.Utils;

namespace TestAli.OSS
{
    class Program
    {
        static void Main(string[] args)
        {
            var result1 = HttpRequestHelper.Get<JObject>("http://192.168.1.3:8123/Token/GetTest", "", AppConfig.SignKey, true);
            Console.WriteLine(JsonConvert.SerializeObject(result1));
            Console.ReadLine();
        }

        public void UploadToAliyun(byte[] uploadFileBytes)
        {
            string uploadFileName = string.Empty;
            //上传到阿里云  
            using (Stream fileStream = new MemoryStream(uploadFileBytes))//转成Stream流  
            {
                string md5 = OssUtils.ComputeContentMd5(fileStream, uploadFileBytes.Length);
                string today = DateTime.Now.ToString("yyyyMMdd");
                string FileName = uploadFileName + today + Path.GetExtension(uploadFileName);//文件名=文件名+当前上传时间  
                string FilePath = "Upload/" + today + "/" + FileName;//云文件保存路径  
                try
                {
                    //初始化阿里云配置--外网Endpoint、访问ID、访问password  
                    OssClient aliyun = new OssClient("https://oss-cn-【外网Endpoint区域】.aliyuncs.com", "your Access Key ID", "your Access Key Secret");

                    //将文件md5值赋值给meat头信息，服务器验证文件MD5  
                    var objectMeta = new ObjectMetadata
                    {
                        ContentMd5 = md5,
                    };
                    //文件上传--空间名、文件保存路径、文件流、meta头信息(文件md5) //返回meta头信息(文件md5)  
                    aliyun.PutObject("bucketName", FilePath, fileStream, objectMeta);

                    new Model<IList<string>>("", "", null);
                    UploadConfig.Config.Result(() =>
                    {
                        return new Model("","");
                    });
                }
                catch (Exception e)
                {
                    
                }
                finally
                {
                    
                }
            }
        }
    }

    public class UploadConfig
    {
        private static UploadConfig config;
        public static UploadConfig Config
        {
            get
            {
                if (config == null)
                {
                    config = new UploadConfig();
                }
                return config;
            }
        }

        private string uploadFieldName;

        public string UploadFieldName
        {
            get
            {
                if (string.IsNullOrEmpty(uploadFieldName))
                    uploadFieldName = "";
                return uploadFieldName;
            }
        }

        public Model Result(Func<Model> func)
        {
            try
            {
               return func();
            }
            catch(Exception ex)
            {
                return new Model("", "");
            }
        }
    }

   
    public partial class BaseInfo
    {
        private readonly object objlock = new object();
        public BaseInfo(string conn)
        {
            lock (objlock)
            {
                if (!string.IsNullOrEmpty(conn))
                {
                    conn = "";
                }
                //执行操作方法
            }
        }
    }

    public partial class NewInfo : BaseInfo
    {
        public NewInfo()
            : base("conname")
        {

        }
    }

    public partial class Base1Info
    {
        private readonly string strcon;
        public Base1Info()
        {
            if(string.IsNullOrEmpty(strcon))
            {
                strcon = "";
            }
        }
    }

    public partial class New1Info : Base1Info
    {

    }

    public class Model
    {
        public Model(string code,string message)
        {
            this.code = code;
            this.message = message;
        }
        public string code { get; set; }

        public string message { get; set; }

    }

    public class Model<T> : Model
    {
        public Model(string code, string message, T result)
            : base(code, message)
        {
            this.result = result;
        }
        private T result;
        public T Result
        {
            get;
            set;
        }
    }
}
