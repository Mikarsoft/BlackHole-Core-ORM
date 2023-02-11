using BlackHole.Enums;
using System.Data;

namespace BlackHole.Interfaces
{
    internal interface IBHDatabaseSelector
    {
        /// <summary>
        /// This Method Finds the Type of the Database You are using in your 
        /// Application and returns the IDbConnection, ready for Use.
        /// !!Don't forget to dispose the Connection after your Job is done.
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConnection();

        bool TestServerConnection();

        IDbConnection CreateConnection(string connectionString);

        string[] SqlDatatypesTranslation();

        bool IsLite();

        bool IsTransact();

        string GetDatabaseName();

        BHSqlTypes GetSqlType();

        string GetServerConnection();

        string GetPrimaryKeyCommand();

        string GetGuidPrimaryKeyCommand();

        int GetSqlTypeId();

        bool GetMyShit();

        string[] IdOutput(string tableName, string columnName, bool g);

        bool IsMyShittyDb();

        BHIdTypes GetIdType(Type type);

        string GetDatePrimaryKeyCommand();

        string GetStringPrimaryKeyCommand();

        bool RequiredIdGeneration(BHIdTypes dataType);
    }
}
