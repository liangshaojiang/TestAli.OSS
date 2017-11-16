using System;
using System.Data;
using System.Text;

namespace Sign.Common.Logging
{
    #region Logger

    public static class DbLogger
    {
        //初始化金财神接口
        public static void LogWrite(long userId,int teacherId,string techName, string createTime, string valiTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Concat("执行状态: 成功！"));
            sb.AppendLine(string.Concat("模板标题:预售结束通知"));
            sb.AppendLine(string.Concat("用户ID:" + userId));
            sb.AppendLine(string.Concat("分析师ID:" + teacherId));
            sb.AppendLine(string.Concat("模板内容:您" + createTime + "拜师的" + techName + "老师将于明日结束"));
            sb.AppendLine(string.Concat("拜师时间:" + createTime));
            sb.AppendLine(string.Concat("结束时间:" + valiTime));

            Logger.Instance().Write(sb.ToString(), LogLevel.AppInfo);
        }

        public static void LogWrite(string message,string userId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Concat("消息内容:" + message + "  用户ID：" + userId));

            Logger.Instance().Write(sb.ToString(), LogLevel.AppInfo);
        }

        public static void LogWriteMessage(string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Concat("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  消息内容：") + message);
            Logger.Instance().Write(sb.ToString(), LogLevel.AppInfo);
        }

        public static void LogException(Exception ex, string lastCommand = null)
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(lastCommand))
                sb.AppendLine(string.Concat("LastCommand:", lastCommand));

            if (ex.InnerException != null)
                ex = ex.InnerException;

            sb.AppendLine(string.Concat("异常信息: " + ex.Message));
            sb.AppendLine(string.Concat("错误源:" + ex.Source));
            sb.AppendLine(string.Concat("堆栈信息:\r\n" + ex.StackTrace));

            Logger.Instance().Write(sb.ToString(), LogLevel.AppInfo);
        }

        public static void LogTrace(IDbCommand cmd, long elapsedMilliseconds = 0)
        {
            StringBuilder sb = new StringBuilder();

            if (elapsedMilliseconds > 0)
                sb.AppendLine(string.Format("执行时长: {0} ms", elapsedMilliseconds));

            sb.AppendLine(string.Format("CommandText: {0}", cmd.CommandText));
            sb.AppendLine(string.Format("CommandType: {0}", cmd.CommandType));
            sb.AppendLine(string.Format("Parameters: {0}", cmd.Parameters.Count));
            foreach (IDbDataParameter m in cmd.Parameters)
            {
                sb.Append(string.Format("\tDirection: {0}", m.Direction));
                sb.Append(string.Format("\tParameterName: {0}", m.ParameterName));
                sb.Append(string.Format("\tDbType: {0}", m.DbType));
                sb.AppendLine(string.Format("\tDbValue: {0}", m.Value));
            }

            Logger.Instance().Write(sb.ToString(), LogLevel.SqlTrace, LogRule.Hour, elapsedMilliseconds);
        }

        public static void LogLuckTrace(string message,long elapsedMilliseconds = 0)
        {
            Logger.Instance().Write(message.ToString(), LogLevel.LuckTrace, LogRule.Day, elapsedMilliseconds);
        }
    }

    #endregion
}