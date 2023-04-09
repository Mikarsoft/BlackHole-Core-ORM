using BlackHole.CoreSupport;
using BlackHole.Enums;
using System.Data;

namespace BlackHole.Internal
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
