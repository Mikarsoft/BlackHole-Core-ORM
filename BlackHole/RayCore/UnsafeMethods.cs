using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Text;

namespace BlackHole.RayCore
{
    [SuppressUnmanagedCodeSecurity()]
    internal static class UnsafeNativeMethods
    {

        //
        // Ray64
        //
        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLAllocHandle(
            /*SQLSMALLINT*/Ray64.SQL_HANDLE HandleType,
            /*SQLHANDLE*/IntPtr InputHandle,
            /*SQLHANDLE* */out IntPtr OutputHandle);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLAllocHandle(
            /*SQLSMALLINT*/Ray64.SQL_HANDLE HandleType,
            /*SQLHANDLE*/RayHandle InputHandle,
            /*SQLHANDLE* */out IntPtr OutputHandle);


        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLBindCol(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLUSMALLINT*/ushort ColumnNumber,
            /*SQLSMALLINT*/Ray64.SQL_C TargetType,
            /*SQLPOINTER*/HandleRef TargetValue,
            /*SQLLEN*/IntPtr BufferLength,
            /*SQLLEN* */IntPtr StrLen_or_Ind);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLBindCol(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLUSMALLINT*/ushort ColumnNumber,
            /*SQLSMALLINT*/Ray64.SQL_C TargetType,
            /*SQLPOINTER*/IntPtr TargetValue,
            /*SQLLEN*/IntPtr BufferLength,
            /*SQLLEN* */IntPtr StrLen_or_Ind);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLBindParameter(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLUSMALLINT*/ushort ParameterNumber,
            /*SQLSMALLINT*/short ParamDirection,
            /*SQLSMALLINT*/Ray64.SQL_C SQLCType,
            /*SQLSMALLINT*/short SQLType,
            /*SQLULEN*/IntPtr cbColDef,
            /*SQLSMALLINT*/IntPtr ibScale,
            /*SQLPOINTER*/HandleRef rgbValue,
            /*SQLLEN*/IntPtr BufferLength,
            /*SQLLEN* */HandleRef StrLen_or_Ind);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLCancel(
            /*SQLHSTMT*/RayStatementHandle StatementHandle);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLCloseCursor(
            /*SQLHSTMT*/RayStatementHandle StatementHandle);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLColAttributeW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLUSMALLINT*/short ColumnNumber,
            /*SQLUSMALLINT*/short FieldIdentifier,
            /*SQLPOINTER*/RayNativeBuffer CharacterAttribute,
            /*SQLSMALLINT*/short BufferLength,
            /*SQLSMALLINT* */out short StringLength,
            /*SQLPOINTER*/out IntPtr NumericAttribute);

        // note: in sql.h this is defined differently for the 64Bit platform.
        // However, for us the code is not different for SQLPOINTER or SQLLEN ...
        // frome sql.h:
        // #ifdef _WIN64
        // SQLRETURN  SQL_API SQLColAttribute (SQLHSTMT StatementHandle,
        //            SQLUSMALLINT ColumnNumber, SQLUSMALLINT FieldIdentifier,
        //            SQLPOINTER CharacterAttribute, SQLSMALLINT BufferLength,
        //            SQLSMALLINT *StringLength, SQLLEN *NumericAttribute);
        // #else
        // SQLRETURN  SQL_API SQLColAttribute (SQLHSTMT StatementHandle,
        //            SQLUSMALLINT ColumnNumber, SQLUSMALLINT FieldIdentifier,
        //            SQLPOINTER CharacterAttribute, SQLSMALLINT BufferLength,
        //            SQLSMALLINT *StringLength, SQLPOINTER NumericAttribute);
        // #endif


        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLColumnsW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string CatalogName,
            /*SQLSMALLINT*/short NameLen1,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string SchemaName,
            /*SQLSMALLINT*/short NameLen2,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string TableName,
            /*SQLSMALLINT*/short NameLen3,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string ColumnName,
            /*SQLSMALLINT*/short NameLen4);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLDisconnect(
            /*SQLHDBC*/IntPtr ConnectionHandle);

        [DllImport(ExternDll.Ray64, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLDriverConnectW(
            /*SQLHDBC*/RayConnectionHandle hdbc,
            /*SQLHWND*/IntPtr hwnd,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string               connectionstring,
            /*SQLSMALLINT*/short cbConnectionstring,
            /*SQLCHAR* */IntPtr connectionstringout,
            /*SQLSMALLINT*/short cbConnectionstringoutMax,
            /*SQLSMALLINT* */out short cbConnectionstringout,
            /*SQLUSMALLINT*/short fDriverCompletion);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLEndTran(
            /*SQLSMALLINT*/Ray64.SQL_HANDLE HandleType,
            /*SQLHANDLE*/IntPtr Handle,
            /*SQLSMALLINT*/short CompletionType);

        [DllImport(ExternDll.Ray64, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLExecDirectW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string   StatementText,
            /*SQLINTEGER*/int TextLength);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLExecute(
            /*SQLHSTMT*/RayStatementHandle StatementHandle);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLFetch(
            /*SQLHSTMT*/RayStatementHandle StatementHandle);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLFreeHandle(
            /*SQLSMALLINT*/Ray64.SQL_HANDLE HandleType,
            /*SQLHSTMT*/IntPtr StatementHandle);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLFreeStmt(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLUSMALLINT*/Ray64.STMT Option);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLGetConnectAttrW(
            /*SQLHBDC*/RayConnectionHandle ConnectionHandle,
            /*SQLINTEGER*/Ray64.SQL_ATTR Attribute,
            /*SQLPOINTER*/byte[] Value,
            /*SQLINTEGER*/int BufferLength,
            /*SQLINTEGER* */out int StringLength);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLGetData(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLUSMALLINT*/ushort ColumnNumber,
            /*SQLSMALLINT*/Ray64.SQL_C TargetType,
            /*SQLPOINTER*/RayNativeBuffer TargetValue,
            /*SQLLEN*/IntPtr BufferLength, // sql.h differs from MSDN
            /*SQLLEN* */out IntPtr StrLen_or_Ind);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLGetDescFieldW(
            /*SQLHSTMT*/RayDescriptorHandle StatementHandle,
            /*SQLUSMALLINT*/short RecNumber,
            /*SQLUSMALLINT*/Ray64.SQL_DESC FieldIdentifier,
            /*SQLPOINTER*/RayNativeBuffer ValuePointer,
            /*SQLINTEGER*/int BufferLength,
            /*SQLINTEGER* */out int StringLength);

        [DllImport(ExternDll.Ray64, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLGetDiagRecW(
            /*SQLSMALLINT*/Ray64.SQL_HANDLE HandleType,
            /*SQLHANDLE*/RayHandle Handle,
            /*SQLSMALLINT*/short RecNumber,
            /*SQLCHAR* */  StringBuilder rchState,
            /*SQLINTEGER* */out int NativeError,
            /*SQLCHAR* */StringBuilder MessageText,
            /*SQLSMALLINT*/short BufferLength,
            /*SQLSMALLINT* */out short TextLength);

        [DllImport(ExternDll.Ray64, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLGetDiagFieldW(
           /*SQLSMALLINT*/ Ray64.SQL_HANDLE HandleType,
           /*SQLHANDLE*/   RayHandle Handle,
           /*SQLSMALLINT*/ short RecNumber,
           /*SQLSMALLINT*/ short DiagIdentifier,
           [MarshalAs(UnmanagedType.LPWStr)]
           /*SQLPOINTER*/  StringBuilder    rchState,
           /*SQLSMALLINT*/ short BufferLength,
           /*SQLSMALLINT* */ out short StringLength);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLGetFunctions(
            /*SQLHBDC*/RayConnectionHandle hdbc,
            /*SQLUSMALLINT*/Ray64.SQL_API fFunction,
            /*SQLUSMALLINT* */out short pfExists);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLGetInfoW(
            /*SQLHBDC*/RayConnectionHandle hdbc,
            /*SQLUSMALLINT*/Ray64.SQL_INFO fInfoType,
            /*SQLPOINTER*/byte[] rgbInfoValue,
            /*SQLSMALLINT*/short cbInfoValueMax,
            /*SQLSMALLINT* */out short pcbInfoValue);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLGetInfoW(
            /*SQLHBDC*/RayConnectionHandle hdbc,
            /*SQLUSMALLINT*/Ray64.SQL_INFO fInfoType,
            /*SQLPOINTER*/byte[] rgbInfoValue,
            /*SQLSMALLINT*/short cbInfoValueMax,
            /*SQLSMALLINT* */IntPtr pcbInfoValue);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLGetStmtAttrW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLINTEGER*/Ray64.SQL_ATTR Attribute,
            /*SQLPOINTER*/out IntPtr Value,
            /*SQLINTEGER*/int BufferLength,
            /*SQLINTEGER*/out int StringLength);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLGetTypeInfo(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLSMALLINT*/short fSqlType);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLMoreResults(
            /*SQLHSTMT*/RayStatementHandle StatementHandle);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLNumResultCols(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLSMALLINT* */out short ColumnCount);

        [DllImport(ExternDll.Ray64, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLPrepareW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string   StatementText,
            /*SQLINTEGER*/int TextLength);

        [DllImport(ExternDll.Ray64, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLPrimaryKeysW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string CatalogName,
            /*SQLSMALLINT*/short NameLen1,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */ string SchemaName,
            /*SQLSMALLINT*/short NameLen2,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string TableName,
            /*SQLSMALLINT*/short NameLen3);

        [DllImport(ExternDll.Ray64, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLProcedureColumnsW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)] /*SQLCHAR* */ string CatalogName,
            /*SQLSMALLINT*/short NameLen1,
            [In, MarshalAs(UnmanagedType.LPWStr)] /*SQLCHAR* */ string SchemaName,
            /*SQLSMALLINT*/short NameLen2,
            [In, MarshalAs(UnmanagedType.LPWStr)] /*SQLCHAR* */ string ProcName,
            /*SQLSMALLINT*/short NameLen3,
            [In, MarshalAs(UnmanagedType.LPWStr)] /*SQLCHAR* */ string ColumnName,
            /*SQLSMALLINT*/short NameLen4);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLProceduresW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)] /*SQLCHAR* */ string CatalogName,
            /*SQLSMALLINT*/short NameLen1,
            [In, MarshalAs(UnmanagedType.LPWStr)] /*SQLCHAR* */ string SchemaName,
            /*SQLSMALLINT*/short NameLen2,
            [In, MarshalAs(UnmanagedType.LPWStr)] /*SQLCHAR* */ string ProcName,
            /*SQLSMALLINT*/short NameLen3);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLRowCount(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLLEN* */out IntPtr RowCount);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLSetConnectAttrW(
            /*SQLHBDC*/RayConnectionHandle ConnectionHandle,
            /*SQLINTEGER*/Ray64.SQL_ATTR Attribute,
            /*SQLPOINTER*/System.Transactions.IDtcTransaction Value,
            /*SQLINTEGER*/int StringLength);

        [DllImport(ExternDll.Ray64, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLSetConnectAttrW(
            /*SQLHBDC*/RayConnectionHandle ConnectionHandle,
            /*SQLINTEGER*/Ray64.SQL_ATTR Attribute,
            /*SQLPOINTER*/string Value,
            /*SQLINTEGER*/int StringLength);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLSetConnectAttrW(
            /*SQLHBDC*/RayConnectionHandle ConnectionHandle,
            /*SQLINTEGER*/Ray64.SQL_ATTR Attribute,
            /*SQLPOINTER*/IntPtr Value,
            /*SQLINTEGER*/int StringLength);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLSetConnectAttrW( // used only for AutoCommitOn
            /*SQLHBDC*/IntPtr ConnectionHandle,
            /*SQLINTEGER*/Ray64.SQL_ATTR Attribute,
            /*SQLPOINTER*/IntPtr Value,
            /*SQLINTEGER*/int StringLength);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLSetDescFieldW(
            /*SQLHSTMT*/RayDescriptorHandle StatementHandle,
            /*SQLSMALLINT*/short ColumnNumber,
            /*SQLSMALLINT*/Ray64.SQL_DESC FieldIdentifier,
            /*SQLPOINTER*/HandleRef CharacterAttribute,
            /*SQLINTEGER*/int BufferLength);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLSetDescFieldW(
            /*SQLHSTMT*/RayDescriptorHandle StatementHandle,
            /*SQLSMALLINT*/short ColumnNumber,
            /*SQLSMALLINT*/Ray64.SQL_DESC FieldIdentifier,
            /*SQLPOINTER*/IntPtr CharacterAttribute,
            /*SQLINTEGER*/int BufferLength);

        [DllImport(ExternDll.Ray64)]
        // user can set SQL_ATTR_CONNECTION_POOLING attribute with envHandle = null, this attribute is process-level attribute
        [ResourceExposure(ResourceScope.Process)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLSetEnvAttr(
            /*SQLHENV*/RayEnvironmentHandle EnvironmentHandle,
            /*SQLINTEGER*/Ray64.SQL_ATTR Attribute,
            /*SQLPOINTER*/IntPtr Value,
            /*SQLINTEGER*/Ray64.SQL_IS StringLength);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLSetStmtAttrW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLINTEGER*/int Attribute,
            /*SQLPOINTER*/IntPtr Value,
            /*SQLINTEGER*/int StringLength);

        [DllImport(ExternDll.Ray64, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLSpecialColumnsW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLUSMALLINT*/Ray64.SQL_SPECIALCOLS IdentifierType,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string CatalogName,
            /*SQLSMALLINT*/short NameLen1,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string SchemaName,
            /*SQLSMALLINT*/short NameLen2,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string TableName,
            /*SQLSMALLINT*/short NameLen3,
            /*SQLUSMALLINT*/Ray64.SQL_SCOPE Scope,
            /*SQLUSMALLINT*/ Ray64.SQL_NULLABILITY Nullable);

        [DllImport(ExternDll.Ray64, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLStatisticsW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string CatalogName,
            /*SQLSMALLINT*/short NameLen1,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string SchemaName,
            /*SQLSMALLINT*/short NameLen2,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string TableName,
            /*SQLSMALLINT*/short NameLen3,
            /*SQLUSMALLINT*/short Unique,
            /*SQLUSMALLINT*/short Reserved);

        [DllImport(ExternDll.Ray64)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray64.RetCode SQLTablesW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string CatalogName,
            /*SQLSMALLINT*/short NameLen1,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string SchemaName,
            /*SQLSMALLINT*/short NameLen2,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string TableName,
            /*SQLSMALLINT*/short NameLen3,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string TableType,
            /*SQLSMALLINT*/short NameLen4);

        [DllImport(ExternDll.Kernel32, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        internal static extern int lstrlenW(IntPtr ptr);
    }
}
