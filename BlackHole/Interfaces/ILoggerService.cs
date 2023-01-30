
namespace BlackHole.Interfaces
{
    internal interface ILoggerService
    {
        void CreateErrorLogs(string Area, string Message, string Details);
    }
}
