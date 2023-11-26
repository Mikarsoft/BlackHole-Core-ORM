
namespace BlackHole.Ray
{
    internal static class HResults
    {

        internal const int Configuration = unchecked((int)0x80131902);

        // Xml
        internal const int Xml = unchecked((int)0x80131940);
        internal const int XmlSchema = unchecked((int)0x80131941);
        internal const int XmlXslt = unchecked((int)0x80131942);
        internal const int XmlXPath = unchecked((int)0x80131943);

        // DataSet
        internal const int Data = unchecked((int)0x80131920);
        internal const int DataDeletedRowInaccessible = unchecked((int)0x80131921);
        internal const int DataDuplicateName = unchecked((int)0x80131922);
        internal const int DataInRowChangingEvent = unchecked((int)0x80131923);
        internal const int DataInvalidConstraint = unchecked((int)0x80131924);
        internal const int DataMissingPrimaryKey = unchecked((int)0x80131925);
        internal const int DataNoNullAllowed = unchecked((int)0x80131926);
        internal const int DataReadOnly = unchecked((int)0x80131927);
        internal const int DataRowNotInTable = unchecked((int)0x80131928);
        internal const int DataVersionNotFound = unchecked((int)0x80131929);
        internal const int DataConstraint = unchecked((int)0x8013192A);
        internal const int StrongTyping = unchecked((int)0x8013192B);

        // Managed Providers
        internal const int SqlType = unchecked((int)0x80131930);
        internal const int SqlNullValue = unchecked((int)0x80131931);
        internal const int SqlTruncate = unchecked((int)0x80131932);
        internal const int AdapterMapping = unchecked((int)0x80131933);
        internal const int DataAdapter = unchecked((int)0x80131934);
        internal const int DBConcurrency = unchecked((int)0x80131935);
        internal const int OperationAborted = unchecked((int)0x80131936);
        internal const int InvalidUdt = unchecked((int)0x80131937);
        internal const int Metadata = unchecked((int)0x80131939);
        internal const int InvalidQuery = unchecked((int)0x8013193A);
        internal const int CommandCompilation = unchecked((int)0x8013193B);
        internal const int CommandExecution = unchecked((int)0x8013193C);


        internal const int SqlException = unchecked((int)0x80131904); // System.Data.SqlClient.SqlClientException
        internal const int RayException = unchecked((int)0x80131937);   // System.Data.Odbc.OdbcException
        internal const int OracleException = unchecked((int)0x80131938); // System.Data.OracleClient.OracleException
        internal const int ConnectionPlanException = unchecked((int)0x8013193d); // System.Data.SqlClient.ConnectionPlanException

        // Configuration encryption
        internal const int NteBadKeySet = unchecked((int)0x80090016);

        // Win32
        internal const int Win32AccessDenied = unchecked((int)0x80070005);
        internal const int Win32InvalidHandle = unchecked((int)0x80070006);


#if !FEATURE_PAL
        internal const int License = unchecked((int)0x80131901);
        internal const int InternalBufferOverflow = unchecked((int)0x80131905);
        internal const int ServiceControllerTimeout = unchecked((int)0x80131906);
        internal const int Install = unchecked((int)0x80131907);

        // Win32
        internal const int EFail = unchecked((int)0x80004005);
#endif
    }
}
