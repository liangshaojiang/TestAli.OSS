using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace Sign.Common.Logging
{
    public class Logger
    {
        /// <summary>
        /// 日志队列
        /// </summary>
        private static ConcurrentQueue<KeyValuePair<string, string>> _logQueue;

        /// <summary>
        /// 日志文件夹
        /// </summary>
        private string _logPath = System.Configuration.ConfigurationManager.AppSettings["LogPath"].ToString();

        private int _size;

        private Timer _watcher;

        #region Instance

        private static readonly object _lockObject = new object();
        private static volatile Logger _instance = null;

        /// <summary>
        /// 写日志，通过队列容量或定时触发写入操作
        /// </summary>
        /// <param name="capacity">记录数，默认为100</param>
        /// <param name="seconds">毫秒数，默认60秒</param>
        public Logger(int size = 100, int milliseconds = 60000)
        {
            //如果目录不存在则创建
            if (!Directory.Exists(this._logPath))
                Directory.CreateDirectory(this._logPath);

            _logQueue = new ConcurrentQueue<KeyValuePair<string, string>>();
            _size = size;

            _watcher = new Timer(milliseconds);
            _watcher.Elapsed += (o, e) =>
            {
                Submit();
            };
            _watcher.Start();
        }

        public static Logger Instance()
        {
            if (_instance == null)
            {
                lock (_lockObject)
                {
                    if (_instance == null)
                    {
#if DEBUG
                        _instance = new Logger(100, 3000);
#else
                        _instance = new Logger();
#endif
                    }
                }
            }
            return _instance;
        }

        #endregion

        #region 写日志

        /// <summary>
        /// 写日志 是一个入队操作
        /// </summary>
        /// <param name="str"></param>
        public void Write(string s, string prefix, LogRule rule = LogRule.Day)
        {
            if (string.IsNullOrWhiteSpace(s))
                return;

            DateTime dt = DateTime.Now;
            string val = string.Concat(dt, "\r\n", s);
            string key = string.Concat(prefix, GetKey(dt, rule));
            Write(key, val);
        }

        public void Write(string key, string val)
        {
            _logQueue.Enqueue(new KeyValuePair<string, string>(key, val));
            if (_logQueue.Count() >= _size)
                Submit();
        }

        /// <summary>
        /// 写日志 是一个入队操作
        /// </summary>
        /// <param name="str"></param>
        public void Write(string s, LogLevel level = LogLevel.AppInfo, LogRule rule = LogRule.Day, long ms = 0)
        {
            string prefix = level.ToString();
            if (ms > 0)
                ms = ms / 10 * 10;
            if (ms > 0)
                prefix = string.Concat("_", prefix, ms, "_");

            Write(s, prefix, rule);
        }

        #endregion

        #region 文本记录日志

        /// <summary>
        /// 写入文本记录
        /// </summary>
        public void Submit()
        {
            //独占方式，因为文件只能由一个进程写入.
            StreamWriter writer = null;
            var dict = GetLogText();
            string filename;
            FileInfo file;
            try
            {
                lock (_lockObject)
                {
                    foreach (var kv in dict)
                    {
                        filename = string.Concat(kv.Key, ".txt");

                        file = new FileInfo(this._logPath + "/" + filename);

                        //文件不存在就创建,true表示追加
                        writer = new StreamWriter(file.FullName, true, Encoding.UTF8);
                        writer.WriteLine(kv.Value);
                        writer.Close();
                    }
                }
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        private string GetKey(DateTime dt, LogRule rule)
        {
            string key;
            switch (rule)
            {
                case LogRule.Minute:
                    key = dt.ToString("yyyyMMddHHmm");
                    break;

                case LogRule.No:
                    key = "";
                    break;

                case LogRule.Day:
                    key = dt.ToString("yyyyMMdd");
                    break;

                case LogRule.Hour:
                default:
                    key = dt.ToString("yyyyMMddHH");
                    break;
            }
            return key;
        }

        /// <summary>
        /// 得到日志文本 一个出队操作
        /// 将主键相同的数据拼接到一起，减少写入的io操作
        /// </summary>
        /// <returns></returns>
        private ConcurrentDictionary<string, string> GetLogText()
        {
            ConcurrentDictionary<string, string> dict = new ConcurrentDictionary<string, string>();
            string key, val;
            do
            {
                KeyValuePair<string, string> kv;

                if (_logQueue.TryDequeue(out kv))
                {
                    key = kv.Key;

                    val = string.Concat(kv.Value, "\r\n----------------------------------------\r\n");

                    dict.AddOrUpdate(key, val, (k, v) => string.Concat(v + "\r\n" + val));
                }
            } while (_logQueue.Count > 0);
            return dict;
        }

        #endregion
    }
}