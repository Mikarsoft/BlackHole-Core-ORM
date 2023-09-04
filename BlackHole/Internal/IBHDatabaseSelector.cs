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
        string GetCompositePrimaryKeyCommand(Type propType, string columName);

        int GetSqlTypeId();
        bool GetMyShit();
        bool GetOpenPKConstraint();
        void SetDbDateFormat(IExecutionProvider _executionProvider);

        string TableSchemaCheck();
        string GetDatabaseSchema();
        string GetDatabaseSchemaFk();
        string GetOwnerName();

        IExecutionProvider GetExecutionProvider(string connectionString);
    }
}
