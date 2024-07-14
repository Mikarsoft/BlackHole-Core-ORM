namespace BlackHole.Provider.Abstractions
{
    public interface IBlackHoleLogger
    {
        void CreateErrorLogs(string commandText, string Area, string Message, string Details);

        string GenerateSHA1(string text);
    }
}
