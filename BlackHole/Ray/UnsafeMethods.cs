using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Ray
{
    [SuppressUnmanagedCodeSecurity()]
    internal static class UnsafeNativeMethods
    {

        //
        // Ray32
        //
        [DllImport(ExternDll.Ray32)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLAllocHandle(
            /*SQLSMALLINT*/Ray32.SQL_HANDLE HandleType,
            /*SQLHANDLE*/IntPtr InputHandle,
            /*SQLHANDLE* */out IntPtr OutputHandle);

        [DllImport(ExternDll.Ray32)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLAllocHandle(
            /*SQLSMALLINT*/Ray32.SQL_HANDLE HandleType,
            /*SQLHANDLE*/RayHandle InputHandle,
            /*SQLHANDLE* */out IntPtr OutputHandle);


        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLBindCol(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLUSMALLINT*/UInt16 ColumnNumber,
            /*SQLSMALLINT*/Ray32.SQL_C TargetType,
            /*SQLPOINTER*/HandleRef TargetValue,
            /*SQLLEN*/IntPtr BufferLength,
            /*SQLLEN* */IntPtr StrLen_or_Ind);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLBindCol(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLUSMALLINT*/UInt16 ColumnNumber,
            /*SQLSMALLINT*/Ray32.SQL_C TargetType,
            /*SQLPOINTER*/IntPtr TargetValue,
            /*SQLLEN*/IntPtr BufferLength,
            /*SQLLEN* */IntPtr StrLen_or_Ind);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLBindParameter(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLUSMALLINT*/UInt16 ParameterNumber,
            /*SQLSMALLINT*/Int16 ParamDirection,
            /*SQLSMALLINT*/Ray32.SQL_C SQLCType,
            /*SQLSMALLINT*/Int16 SQLType,
            /*SQLULEN*/IntPtr cbColDef,
            /*SQLSMALLINT*/IntPtr ibScale,
            /*SQLPOINTER*/HandleRef rgbValue,
            /*SQLLEN*/IntPtr BufferLength,
            /*SQLLEN* */HandleRef StrLen_or_Ind);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLCancel(
            /*SQLHSTMT*/RayStatementHandle StatementHandle);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLCloseCursor(
            /*SQLHSTMT*/RayStatementHandle StatementHandle);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLColAttributeW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLUSMALLINT*/Int16 ColumnNumber,
            /*SQLUSMALLINT*/Int16 FieldIdentifier,
            /*SQLPOINTER*/CNativeBuffer CharacterAttribute,
            /*SQLSMALLINT*/Int16 BufferLength,
            /*SQLSMALLINT* */out Int16 StringLength,
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


        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLColumnsW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string CatalogName,
            /*SQLSMALLINT*/Int16 NameLen1,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string SchemaName,
            /*SQLSMALLINT*/Int16 NameLen2,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string TableName,
            /*SQLSMALLINT*/Int16 NameLen3,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string ColumnName,
            /*SQLSMALLINT*/Int16 NameLen4);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLDisconnect(
            /*SQLHDBC*/IntPtr ConnectionHandle);

        [DllImport(ExternDll.Ray32, CharSet = CharSet.Unicode)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLDriverConnectW(
            /*SQLHDBC*/RayConnectionHandle hdbc,
            /*SQLHWND*/IntPtr hwnd,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string               connectionstring,
            /*SQLSMALLINT*/Int16 cbConnectionstring,
            /*SQLCHAR* */IntPtr connectionstringout,
            /*SQLSMALLINT*/Int16 cbConnectionstringoutMax,
            /*SQLSMALLINT* */out Int16 cbConnectionstringout,
            /*SQLUSMALLINT*/Int16 fDriverCompletion);

        [DllImport(ExternDll.Ray32)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLEndTran(
            /*SQLSMALLINT*/Ray32.SQL_HANDLE HandleType,
            /*SQLHANDLE*/IntPtr Handle,
            /*SQLSMALLINT*/Int16 CompletionType);

        [DllImport(ExternDll.Ray32, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLExecDirectW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string   StatementText,
            /*SQLINTEGER*/Int32 TextLength);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLExecute(
            /*SQLHSTMT*/RayStatementHandle StatementHandle);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLFetch(
            /*SQLHSTMT*/RayStatementHandle StatementHandle);

        [DllImport(ExternDll.Ray32)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLFreeHandle(
            /*SQLSMALLINT*/Ray32.SQL_HANDLE HandleType,
            /*SQLHSTMT*/IntPtr StatementHandle);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLFreeStmt(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLUSMALLINT*/Ray32.STMT Option);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLGetConnectAttrW(
            /*SQLHBDC*/RayConnectionHandle ConnectionHandle,
            /*SQLINTEGER*/Ray32.SQL_ATTR Attribute,
            /*SQLPOINTER*/byte[] Value,
            /*SQLINTEGER*/Int32 BufferLength,
            /*SQLINTEGER* */out Int32 StringLength);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLGetData(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLUSMALLINT*/UInt16 ColumnNumber,
            /*SQLSMALLINT*/Ray32.SQL_C TargetType,
            /*SQLPOINTER*/CNativeBuffer TargetValue,
            /*SQLLEN*/IntPtr BufferLength, // sql.h differs from MSDN
            /*SQLLEN* */out IntPtr StrLen_or_Ind);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLGetDescFieldW(
            /*SQLHSTMT*/RayDescriptorHandle StatementHandle,
            /*SQLUSMALLINT*/Int16 RecNumber,
            /*SQLUSMALLINT*/Ray32.SQL_DESC FieldIdentifier,
            /*SQLPOINTER*/CNativeBuffer ValuePointer,
            /*SQLINTEGER*/Int32 BufferLength,
            /*SQLINTEGER* */out Int32 StringLength);

        [DllImport(ExternDll.Ray32, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLGetDiagRecW(
            /*SQLSMALLINT*/Ray32.SQL_HANDLE HandleType,
            /*SQLHANDLE*/RayHandle Handle,
            /*SQLSMALLINT*/Int16 RecNumber,
            /*SQLCHAR* */  StringBuilder rchState,
            /*SQLINTEGER* */out Int32 NativeError,
            /*SQLCHAR* */StringBuilder MessageText,
            /*SQLSMALLINT*/Int16 BufferLength,
            /*SQLSMALLINT* */out Int16 TextLength);

        [DllImport(ExternDll.Ray32, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLGetDiagFieldW(
           /*SQLSMALLINT*/ Ray32.SQL_HANDLE HandleType,
           /*SQLHANDLE*/   RayHandle Handle,
           /*SQLSMALLINT*/ Int16 RecNumber,
           /*SQLSMALLINT*/ Int16 DiagIdentifier,
           [MarshalAs(UnmanagedType.LPWStr)]
           /*SQLPOINTER*/  StringBuilder    rchState,
           /*SQLSMALLINT*/ Int16 BufferLength,
           /*SQLSMALLINT* */ out Int16 StringLength);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLGetFunctions(
            /*SQLHBDC*/RayConnectionHandle hdbc,
            /*SQLUSMALLINT*/Ray32.SQL_API fFunction,
            /*SQLUSMALLINT* */out Int16 pfExists);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLGetInfoW(
            /*SQLHBDC*/RayConnectionHandle hdbc,
            /*SQLUSMALLINT*/Ray32.SQL_INFO fInfoType,
            /*SQLPOINTER*/byte[] rgbInfoValue,
            /*SQLSMALLINT*/Int16 cbInfoValueMax,
            /*SQLSMALLINT* */out Int16 pcbInfoValue);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLGetInfoW(
            /*SQLHBDC*/RayConnectionHandle hdbc,
            /*SQLUSMALLINT*/Ray32.SQL_INFO fInfoType,
            /*SQLPOINTER*/byte[] rgbInfoValue,
            /*SQLSMALLINT*/Int16 cbInfoValueMax,
            /*SQLSMALLINT* */IntPtr pcbInfoValue);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLGetStmtAttrW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLINTEGER*/Ray32.SQL_ATTR Attribute,
            /*SQLPOINTER*/out IntPtr Value,
            /*SQLINTEGER*/Int32 BufferLength,
            /*SQLINTEGER*/out Int32 StringLength);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLGetTypeInfo(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLSMALLINT*/Int16 fSqlType);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLMoreResults(
            /*SQLHSTMT*/RayStatementHandle StatementHandle);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLNumResultCols(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLSMALLINT* */out Int16 ColumnCount);

        [DllImport(ExternDll.Ray32, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLPrepareW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string   StatementText,
            /*SQLINTEGER*/Int32 TextLength);

        [DllImport(ExternDll.Ray32, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLPrimaryKeysW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string CatalogName,
            /*SQLSMALLINT*/Int16 NameLen1,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */ string SchemaName,
            /*SQLSMALLINT*/Int16 NameLen2,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string TableName,
            /*SQLSMALLINT*/Int16 NameLen3);

        [DllImport(ExternDll.Ray32, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLProcedureColumnsW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)] /*SQLCHAR* */ string CatalogName,
            /*SQLSMALLINT*/Int16 NameLen1,
            [In, MarshalAs(UnmanagedType.LPWStr)] /*SQLCHAR* */ string SchemaName,
            /*SQLSMALLINT*/Int16 NameLen2,
            [In, MarshalAs(UnmanagedType.LPWStr)] /*SQLCHAR* */ string ProcName,
            /*SQLSMALLINT*/Int16 NameLen3,
            [In, MarshalAs(UnmanagedType.LPWStr)] /*SQLCHAR* */ string ColumnName,
            /*SQLSMALLINT*/Int16 NameLen4);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLProceduresW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)] /*SQLCHAR* */ string CatalogName,
            /*SQLSMALLINT*/Int16 NameLen1,
            [In, MarshalAs(UnmanagedType.LPWStr)] /*SQLCHAR* */ string SchemaName,
            /*SQLSMALLINT*/Int16 NameLen2,
            [In, MarshalAs(UnmanagedType.LPWStr)] /*SQLCHAR* */ string ProcName,
            /*SQLSMALLINT*/Int16 NameLen3);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLRowCount(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLLEN* */out IntPtr RowCount);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLSetConnectAttrW(
            /*SQLHBDC*/RayConnectionHandle ConnectionHandle,
            /*SQLINTEGER*/Ray32.SQL_ATTR Attribute,
            /*SQLPOINTER*/System.Transactions.IDtcTransaction Value,
            /*SQLINTEGER*/Int32 StringLength);

        [DllImport(ExternDll.Ray32, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLSetConnectAttrW(
            /*SQLHBDC*/RayConnectionHandle ConnectionHandle,
            /*SQLINTEGER*/Ray32.SQL_ATTR Attribute,
            /*SQLPOINTER*/string Value,
            /*SQLINTEGER*/Int32 StringLength);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLSetConnectAttrW(
            /*SQLHBDC*/RayConnectionHandle ConnectionHandle,
            /*SQLINTEGER*/Ray32.SQL_ATTR Attribute,
            /*SQLPOINTER*/IntPtr Value,
            /*SQLINTEGER*/Int32 StringLength);

        [DllImport(ExternDll.Ray32)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLSetConnectAttrW( // used only for AutoCommitOn
            /*SQLHBDC*/IntPtr ConnectionHandle,
            /*SQLINTEGER*/Ray32.SQL_ATTR Attribute,
            /*SQLPOINTER*/IntPtr Value,
            /*SQLINTEGER*/Int32 StringLength);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLSetDescFieldW(
            /*SQLHSTMT*/RayDescriptorHandle StatementHandle,
            /*SQLSMALLINT*/Int16 ColumnNumber,
            /*SQLSMALLINT*/Ray32.SQL_DESC FieldIdentifier,
            /*SQLPOINTER*/HandleRef CharacterAttribute,
            /*SQLINTEGER*/Int32 BufferLength);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLSetDescFieldW(
            /*SQLHSTMT*/RayDescriptorHandle StatementHandle,
            /*SQLSMALLINT*/Int16 ColumnNumber,
            /*SQLSMALLINT*/Ray32.SQL_DESC FieldIdentifier,
            /*SQLPOINTER*/IntPtr CharacterAttribute,
            /*SQLINTEGER*/Int32 BufferLength);

        [DllImport(ExternDll.Ray32)]
        // user can set SQL_ATTR_CONNECTION_POOLING attribute with envHandle = null, this attribute is process-level attribute
        [ResourceExposure(ResourceScope.Process)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLSetEnvAttr(
            /*SQLHENV*/RayEnvironmentHandle EnvironmentHandle,
            /*SQLINTEGER*/Ray32.SQL_ATTR Attribute,
            /*SQLPOINTER*/IntPtr Value,
            /*SQLINTEGER*/Ray32.SQL_IS StringLength);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLSetStmtAttrW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLINTEGER*/Int32 Attribute,
            /*SQLPOINTER*/IntPtr Value,
            /*SQLINTEGER*/Int32 StringLength);

        [DllImport(ExternDll.Ray32, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLSpecialColumnsW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            /*SQLUSMALLINT*/Ray32.SQL_SPECIALCOLS IdentifierType,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string CatalogName,
            /*SQLSMALLINT*/Int16 NameLen1,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string SchemaName,
            /*SQLSMALLINT*/Int16 NameLen2,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string TableName,
            /*SQLSMALLINT*/Int16 NameLen3,
            /*SQLUSMALLINT*/Ray32.SQL_SCOPE Scope,
            /*SQLUSMALLINT*/ Ray32.SQL_NULLABILITY Nullable);

        [DllImport(ExternDll.Ray32, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLStatisticsW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string CatalogName,
            /*SQLSMALLINT*/Int16 NameLen1,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string SchemaName,
            /*SQLSMALLINT*/Int16 NameLen2,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string TableName,
            /*SQLSMALLINT*/Int16 NameLen3,
            /*SQLUSMALLINT*/Int16 Unique,
            /*SQLUSMALLINT*/Int16 Reserved);

        [DllImport(ExternDll.Ray32)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern /*SQLRETURN*/Ray32.RetCode SQLTablesW(
            /*SQLHSTMT*/RayStatementHandle StatementHandle,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string CatalogName,
            /*SQLSMALLINT*/Int16 NameLen1,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string SchemaName,
            /*SQLSMALLINT*/Int16 NameLen2,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string TableName,
            /*SQLSMALLINT*/Int16 NameLen3,
            [In, MarshalAs(UnmanagedType.LPWStr)]
            /*SQLCHAR* */string TableType,
            /*SQLSMALLINT*/Int16 NameLen4);
    }
}
