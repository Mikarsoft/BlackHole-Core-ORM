namespace BlackHole.Logger
{
    internal interface ILoggerService
    {
        /// <summary>
        /// Creates Error Logs When needed
        /// </summary>
        /// <param name="Area">Method</param>
        /// <param name="Message">Error Title</param>
        /// <param name="Details">Error Details</param>
        void CreateErrorLogs(string Area, string Message, string Details);
    }
}
