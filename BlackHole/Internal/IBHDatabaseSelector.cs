using BlackHole.CoreSupport;
using BlackHole.Enums;
using System.Data;

namespace BlackHole.Internal
{
    internal interface IBHDatabaseSelector
    {
        string[] SqlDatatypesTranslation();

        bool IsLite();

        string GetDatabaseName();

        string GetServerConnection();

        string GetPrimaryKeyCommand();
        string GetGuidPrimaryKeyCommand();
        string GetStringPrimaryKeyCommand();

        int GetSqlTypeId();

        bool GetMyShit();

        IExecutionProvider GetExecutionProvider(string connectionString);
    }
}
