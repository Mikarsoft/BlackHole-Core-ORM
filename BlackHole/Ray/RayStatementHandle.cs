using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Ray
{
    internal struct SQLLEN
    {
        private IntPtr _value;

        internal SQLLEN(int value)
        {
            _value = new IntPtr(value);
        }

        internal SQLLEN(long value)
        {
#if WIN32
            _value = new IntPtr(checked((int)value));
#else
            _value = new IntPtr(value);
#endif
        }

        internal SQLLEN(IntPtr value)
        {
            _value = value;
        }

        public static implicit operator SQLLEN(int value)
        { // 
            return new SQLLEN(value);
        }

        public static explicit operator SQLLEN(long value)
        {
            return new SQLLEN(value);
        }

        public unsafe static implicit operator int(SQLLEN value)
        { // 
#if WIN32
            return (int)value._value.ToInt32();
#else
            long l = (long)value._value.ToInt64();
            return checked((int)l);
#endif
        }

        public unsafe static explicit operator long(SQLLEN value)
        {
            return value._value.ToInt64();
        }

        public unsafe long ToInt64()
        {
            return _value.ToInt64();
        }
    }

    sealed internal class RayStatementHandle : RayHandle
    {

        internal RayStatementHandle(RayConnectionHandle connectionHandle) : base(Ray32.SQL_HANDLE.STMT, connectionHandle)
        {
        }

        internal Ray32.RetCode BindColumn2(int columnNumber, Ray32.SQL_C targetType, HandleRef buffer, IntPtr length, IntPtr srLen_or_Ind)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLBindCol(this, checked((ushort)columnNumber), targetType, buffer, length, srLen_or_Ind);
            Ray.TraceRay(3, "SQLBindCol", retcode);
            return retcode;
        }

        internal Ray32.RetCode BindColumn3(int columnNumber, Ray32.SQL_C targetType, IntPtr srLen_or_Ind)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLBindCol(this, checked((ushort)columnNumber), targetType, ADP.PtrZero, ADP.PtrZero, srLen_or_Ind);
            Ray.TraceRay(3, "SQLBindCol", retcode);
            return retcode;
        }

        internal Ray32.RetCode BindParameter(short ordinal, short parameterDirection, Ray32.SQL_C sqlctype, Ray32.SQL_TYPE sqltype, IntPtr cchSize, IntPtr scale, HandleRef buffer, IntPtr bufferLength, HandleRef intbuffer)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLBindParameter(this,
                                    checked((ushort)ordinal),   // Parameter Number
                                    parameterDirection,         // InputOutputType
                                    sqlctype,                   // ValueType
                                    checked((short)sqltype),    // ParameterType
                                    cchSize,                    // ColumnSize
                                    scale,                      // DecimalDigits
                                    buffer,                     // ParameterValuePtr
                                    bufferLength,               // BufferLength
                                    intbuffer);                 // StrLen_or_IndPtr
            Ray.TraceRay(3, "SQLBindParameter", retcode);
            return retcode;
        }

        internal Ray32.RetCode Cancel()
        {
            // In Ray3.0 ... a call to SQLCancel when no processing is done has no effect at all
            // (Ray Programmer's Reference ...)
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLCancel(this);
            Ray.TraceRay(3, "SQLCancel", retcode);
            return retcode;
        }

        internal Ray32.RetCode CloseCursor()
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLCloseCursor(this);
            Ray.TraceRay(3, "SQLCloseCursor", retcode);
            return retcode;
        }

        internal Ray32.RetCode ColumnAttribute(int columnNumber, short fieldIdentifier, CNativeBuffer characterAttribute, out short stringLength, out SQLLEN numericAttribute)
        {
            IntPtr result;
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLColAttributeW(this, checked((short)columnNumber), fieldIdentifier, characterAttribute, characterAttribute.ShortLength, out stringLength, out result);
            numericAttribute = new SQLLEN(result);
            Ray.TraceRay(3, "SQLColAttributeW", retcode);
            return retcode;
        }

        internal Ray32.RetCode Columns(string tableCatalog,
                                        string tableSchema,
                                        string tableName,
                                        string columnName)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLColumnsW(this,
                                                                     tableCatalog,
                                                                     Ray.ShortStringLength(tableCatalog),
                                                                     tableSchema,
                                                                     Ray.ShortStringLength(tableSchema),
                                                                     tableName,
                                                                     Ray.ShortStringLength(tableName),
                                                                     columnName,
                                                                     Ray.ShortStringLength(columnName));

            Ray.TraceRay(3, "SQLColumnsW", retcode);
            return retcode;
        }

        internal Ray32.RetCode Execute()
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLExecute(this);
            Ray.TraceRay(3, "SQLExecute", retcode);
            return retcode;
        }

        internal Ray32.RetCode ExecuteDirect(string commandText)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLExecDirectW(this, commandText, Ray32.SQL_NTS);
            Ray.TraceRay(3, "SQLExecDirectW", retcode);
            return retcode;
        }

        internal Ray32.RetCode Fetch()
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLFetch(this);
            Ray.TraceRay(3, "SQLFetch", retcode);
            return retcode;
        }

        internal Ray32.RetCode FreeStatement(Ray32.STMT stmt)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLFreeStmt(this, stmt);
            Ray.TraceRay(3, "SQLFreeStmt", retcode);
            return retcode;
        }

        internal Ray32.RetCode GetData(int index, Ray32.SQL_C sqlctype, CNativeBuffer buffer, int cb, out IntPtr cbActual)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLGetData(this,
                            checked((ushort)index),
                            sqlctype,
                            buffer,
                            new IntPtr(cb),
                            out cbActual);
            Ray.TraceRay(3, "SQLGetData", retcode);
            return retcode;
        }

        internal Ray32.RetCode GetStatementAttribute(Ray32.SQL_ATTR attribute, out IntPtr value, out int stringLength)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLGetStmtAttrW(this, attribute, out value, ADP.PtrSize, out stringLength);
            Ray.TraceRay(3, "SQLGetStmtAttrW", retcode);
            return retcode;
        }

        internal Ray32.RetCode GetTypeInfo(Int16 fSqlType)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLGetTypeInfo(this, fSqlType);
            Ray.TraceRay(3, "SQLGetTypeInfo", retcode);
            return retcode;
        }

        internal Ray32.RetCode MoreResults()
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLMoreResults(this);
            Ray.TraceRay(3, "SQLMoreResults", retcode);
            return retcode;
        }

        internal Ray32.RetCode NumberOfResultColumns(out short columnsAffected)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLNumResultCols(this, out columnsAffected);
            Ray.TraceRay(3, "SQLNumResultCols", retcode);
            return retcode;
        }

        internal Ray32.RetCode Prepare(string commandText)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLPrepareW(this, commandText, Ray32.SQL_NTS);
            Ray.TraceRay(3, "SQLPrepareW", retcode);
            return retcode;
        }

        internal Ray32.RetCode PrimaryKeys(string catalogName, string schemaName, string tableName)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLPrimaryKeysW(this,
                            catalogName, Ray.ShortStringLength(catalogName),          // CatalogName
                            schemaName, Ray.ShortStringLength(schemaName),            // SchemaName
                            tableName, Ray.ShortStringLength(tableName)              // TableName
            );
            Ray.TraceRay(3, "SQLPrimaryKeysW", retcode);
            return retcode;
        }

        internal Ray32.RetCode Procedures(string procedureCatalog,
                                           string procedureSchema,
                                           string procedureName)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLProceduresW(this,
                                                                        procedureCatalog,
                                                                        Ray.ShortStringLength(procedureCatalog),
                                                                        procedureSchema,
                                                                        Ray.ShortStringLength(procedureSchema),
                                                                        procedureName,
                                                                        Ray.ShortStringLength(procedureName));

            Ray.TraceRay(3, "SQLProceduresW", retcode);
            return retcode;
        }

        internal Ray32.RetCode ProcedureColumns(string procedureCatalog,
                                                 string procedureSchema,
                                                 string procedureName,
                                                 string columnName)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLProcedureColumnsW(this,
                                                                              procedureCatalog,
                                                                              Ray.ShortStringLength(procedureCatalog),
                                                                              procedureSchema,
                                                                              Ray.ShortStringLength(procedureSchema),
                                                                              procedureName,
                                                                              Ray.ShortStringLength(procedureName),
                                                                              columnName,
                                                                              Ray.ShortStringLength(columnName));

            Ray.TraceRay(3, "SQLProcedureColumnsW", retcode);
            return retcode;
        }

        internal Ray32.RetCode RowCount(out SQLLEN rowCount)
        {
            IntPtr result;
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLRowCount(this, out result);
            rowCount = new SQLLEN(result);
            Ray.TraceRay(3, "SQLRowCount", retcode);
            return retcode;
        }

        internal Ray32.RetCode SetStatementAttribute(Ray32.SQL_ATTR attribute, IntPtr value, Ray32.SQL_IS stringLength)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLSetStmtAttrW(this, (int)attribute, value, (int)stringLength);
            Ray.TraceRay(3, "SQLSetStmtAttrW", retcode);
            return retcode;
        }

        internal Ray32.RetCode SpecialColumns(string quotedTable)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLSpecialColumnsW(this,
            Ray32.SQL_SPECIALCOLS.ROWVER, null, 0, null, 0,
            quotedTable, Ray.ShortStringLength(quotedTable),
            Ray32.SQL_SCOPE.SESSION, Ray32.SQL_NULLABILITY.NO_NULLS);
            Ray.TraceRay(3, "SQLSpecialColumnsW", retcode);
            return retcode;
        }

        internal Ray32.RetCode Statistics(string tableCatalog,
                                           string tableSchema,
                                           string tableName,
                                           Int16 unique,
                                           Int16 accuracy)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLStatisticsW(this,
                                                                        tableCatalog,
                                                                        Ray.ShortStringLength(tableCatalog),
                                                                        tableSchema,
                                                                        Ray.ShortStringLength(tableSchema),
                                                                        tableName,
                                                                        Ray.ShortStringLength(tableName),
                                                                        unique,
                                                                        accuracy);

            Ray.TraceRay(3, "SQLStatisticsW", retcode);
            return retcode;
        }

        internal Ray32.RetCode Statistics(string tableName)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLStatisticsW(this,
            null, 0, null, 0,
            tableName, Ray.ShortStringLength(tableName),
            (Int16)Ray32.SQL_INDEX.UNIQUE,
            (Int16)Ray32.SQL_STATISTICS_RESERVED.ENSURE);
            Ray.TraceRay(3, "SQLStatisticsW", retcode);
            return retcode;
        }

        internal Ray32.RetCode Tables(string tableCatalog,
                                       string tableSchema,
                                       string tableName,
                                       string tableType)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLTablesW(this,
                                                                    tableCatalog,
                                                                    Ray.ShortStringLength(tableCatalog),
                                                                    tableSchema,
                                                                    Ray.ShortStringLength(tableSchema),
                                                                    tableName,
                                                                    Ray.ShortStringLength(tableName),
                                                                    tableType,
                                                                    Ray.ShortStringLength(tableType));

            Ray.TraceRay(3, "SQLTablesW", retcode);
            return retcode;
        }
    }
}
