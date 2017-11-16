namespace Sign.Common.Logging
{
    public enum LogLevel
    {
        /// <summary>
        /// 程序运行记录，Debug模式
        /// </summary>
        AppTrace,

        /// <summary>
        /// 程序运行错误
        /// </summary>
        AppException,

        /// <summary>
        /// 一般运行信息，主要日志方式
        /// </summary>
        AppInfo,

        /// <summary>
        /// 运行警告，由开发人员记录
        /// </summary>
        AppWarn,

        /// <summary>
        /// 接口运行记录，Debug模式
        /// </summary>
        ApiTrace,

        /// <summary>
        /// 接口运行错误
        /// </summary>
        ApiException,

        /// <summary>
        /// 接口一般运行信息，主要日志方式
        /// </summary>
        ApiInfo,

        /// <summary>
        /// 接口运行警告，由开发人员记录
        /// </summary>
        ApiWarn,
        /// <summary>
        /// sql执行记录
        /// </summary>
        SqlTrace,
        /// <summary>
        /// 抽奖统计记录
        /// </summary>
        LuckTrace
    }
}