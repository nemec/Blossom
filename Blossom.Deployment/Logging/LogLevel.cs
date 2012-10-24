namespace Blossom.Deployment.Logging
{
    public enum LogLevel
    {
        /// <summary>
        /// Log incredibly detailed information.
        /// Logged information should not be critical
        /// to normal use.
        /// </summary>
        Verbose = -2,

        /// <summary>
        /// Log information suitable to aid in debugging.
        /// </summary>
        Debug = -1,

        /// <summary>
        /// Log unexceptional information. Information
        /// logged at this level should not contain
        /// issues (or possible issues).
        /// 
        /// The default log level.
        /// </summary>
        Info = 0,

        /// <summary>
        /// Log information about data that may produce
        /// undesirable consequences but will not affect
        /// performance.
        /// </summary>
        Warn = 1,

        /// <summary>
        /// Log information about errors that have occurred
        /// but are still recoverable.
        /// </summary>
        Error = 2,

        /// <summary>
        /// Log information about unrecoverable errors.
        /// </summary>
        Fatal = 3,

        /// <summary>
        /// Log no information at all.
        /// </summary>
        None = 4
    }
}
