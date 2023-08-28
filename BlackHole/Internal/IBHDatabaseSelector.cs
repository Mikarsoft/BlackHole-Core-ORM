using BlackHole.CoreSupport;

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
        bool GetOpenPKConstraint();

        string TableSchemaCheck();
        string GetDatabaseSchema();
        string GetDatabaseSchemaFk();
        string GetOwnerName();

        IExecutionProvider GetExecutionProvider(string connectionString);
    }
}
