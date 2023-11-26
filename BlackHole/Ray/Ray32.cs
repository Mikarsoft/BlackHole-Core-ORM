
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;

namespace BlackHole.Ray
{

    internal static class Ray
    {

        static internal Exception ConnectionClosed()
        {
            return ADP.InvalidOperation(Res.GetString(Res.Ray_ConnectionClosed));
        }

        static internal Exception OpenConnectionNoOwner()
        {
            return ADP.InvalidOperation(Res.GetString(Res.Ray_OpenConnectionNoOwner));
        }

        static internal Exception UnknownSQLType(Ray32.SQL_TYPE sqltype)
        {
            return ADP.Argument(Res.GetString(Res.Ray_UnknownSQLType, sqltype.ToString()));
        }
        static internal Exception ConnectionStringTooLong()
        {
            return ADP.Argument(Res.GetString(Res.RayConnection_ConnectionStringTooLong, Ray32.MAX_CONNECTION_STRING_LENGTH));
        }
        static internal ArgumentException GetSchemaRestrictionRequired()
        {
            return ADP.Argument(Res.GetString(Res.Ray_GetSchemaRestrictionRequired));
        }
        static internal ArgumentOutOfRangeException NotSupportedEnumerationValue(Type type, int value)
        {
            return ADP.ArgumentOutOfRange(Res.GetString(Res.Ray_NotSupportedEnumerationValue, type.Name, value.ToString(System.Globalization.CultureInfo.InvariantCulture)), type.Name);
        }
        static internal ArgumentOutOfRangeException NotSupportedCommandType(CommandType value)
        {
#if DEBUG
            switch (value)
            {
                case CommandType.Text:
                case CommandType.StoredProcedure:
                    Debug.Assert(false, "valid CommandType " + value.ToString());
                    break;
                case CommandType.TableDirect:
                    break;
                default:
                    Debug.Assert(false, "invalid CommandType " + value.ToString());
                    break;
            }
#endif
            return Ray.NotSupportedEnumerationValue(typeof(CommandType), (int)value);
        }
        static internal ArgumentOutOfRangeException NotSupportedIsolationLevel(IsolationLevel value)
        {
#if DEBUG
            switch (value)
            {
                case IsolationLevel.Unspecified:
                case IsolationLevel.ReadUncommitted:
                case IsolationLevel.ReadCommitted:
                case IsolationLevel.RepeatableRead:
                case IsolationLevel.Serializable:
                case IsolationLevel.Snapshot:
                    Debug.Assert(false, "valid IsolationLevel " + value.ToString());
                    break;
                case IsolationLevel.Chaos:
                    break;
                default:
                    Debug.Assert(false, "invalid IsolationLevel " + value.ToString());
                    break;
            }
#endif
            return Ray.NotSupportedEnumerationValue(typeof(IsolationLevel), (int)value);
        }

        static internal InvalidOperationException NoMappingForSqlTransactionLevel(int value)
        {
            return ADP.DataAdapter(Res.GetString(Res.Ray_NoMappingForSqlTransactionLevel, value.ToString(CultureInfo.InvariantCulture)));
        }

        static internal Exception NegativeArgument()
        {
            return ADP.Argument(Res.GetString(Res.Ray_NegativeArgument));
        }
        static internal Exception CantSetPropertyOnOpenConnection()
        {
            return ADP.InvalidOperation(Res.GetString(Res.Ray_CantSetPropertyOnOpenConnection));
        }
        static internal Exception CantEnableConnectionpooling(Ray32.RetCode retcode)
        {
            return ADP.DataAdapter(Res.GetString(Res.Ray_CantEnableConnectionpooling, Ray32.RetcodeToString(retcode)));
        }
        static internal Exception CantAllocateEnvironmentHandle(Ray32.RetCode retcode)
        {
            return ADP.DataAdapter(Res.GetString(Res.Ray_CantAllocateEnvironmentHandle, Ray32.RetcodeToString(retcode)));
        }
        static internal Exception FailedToGetDescriptorHandle(Ray32.RetCode retcode)
        {
            return ADP.DataAdapter(Res.GetString(Res.Ray_FailedToGetDescriptorHandle, Ray32.RetcodeToString(retcode)));
        }
        static internal Exception NotInTransaction()
        {
            return ADP.InvalidOperation(Res.GetString(Res.Ray_NotInTransaction));
        }
        static internal Exception UnknownRayType(RayType Raytype)
        {
            return ADP.InvalidEnumerationValue(typeof(RayType), (int)Raytype);
        }
        internal const string Pwd = "pwd";

        static internal void TraceRay(int level, string method, Ray32.RetCode retcode)
        {
            Bid.TraceSqlReturn("<Ray|API|Ray|RET> %08X{SQLRETURN}, method=%ls\n", retcode, method);
        }

        internal static short ShortStringLength(string inputString)
        {
            return checked((short)ADP.StringLength(inputString));
        }
    }


    internal static class Ray32
    {
        internal enum SQL_HANDLE : short
        {
            ENV = 1,
            DBC = 2,
            STMT = 3,
            DESC = 4,
        }

        // from .\public\sdk\inc\sqlext.h: and .\public\sdk\inc\sql.h
        // must be public because it is serialized by RayException
        [Serializable]
        public enum RETCODE : int
        { // must be int instead of short for Everett RayException Serializablity.
            SUCCESS = 0,
            SUCCESS_WITH_INFO = 1,
            ERROR = -1,
            INVALID_HANDLE = -2,
            NO_DATA = 100,
        }

        // must be public because it is serialized by RayException
        internal enum RetCode : short
        {
            SUCCESS = 0,
            SUCCESS_WITH_INFO = 1,
            ERROR = -1,
            INVALID_HANDLE = -2,
            NO_DATA = 100,
        }

        internal static string RetcodeToString(RetCode retcode)
        {
            switch (retcode)
            {
                case RetCode.SUCCESS: return "SUCCESS";
                case RetCode.SUCCESS_WITH_INFO: return "SUCCESS_WITH_INFO";
                case RetCode.ERROR: return "ERROR";
                case RetCode.INVALID_HANDLE: return "INVALID_HANDLE";
                case RetCode.NO_DATA: return "NO_DATA";
                default:
                    Debug.Assert(false, "Unknown enumerator passed to RetcodeToString method");
                    goto case RetCode.ERROR;
            }
        }



        internal enum SQL_CONVERT : ushort
        {
            BIGINT = 53,
            BINARY = 54,
            BIT = 55,
            CHAR = 56,
            DATE = 57,
            DECIMAL = 58,
            DOUBLE = 59,
            FLOAT = 60,
            INTEGER = 61,
            LONGVARCHAR = 62,
            NUMERIC = 63,
            REAL = 64,
            SMALLINT = 65,
            TIME = 66,
            TIMESTAMP = 67,
            TINYINT = 68,
            VARBINARY = 69,
            VARCHAR = 70,
            LONGVARBINARY = 71,
        }

        [Flags]
        internal enum SQL_CVT
        {
            CHAR = 0x00000001,
            NUMERIC = 0x00000002,
            DECIMAL = 0x00000004,
            INTEGER = 0x00000008,
            SMALLINT = 0x00000010,
            FLOAT = 0x00000020,
            REAL = 0x00000040,
            DOUBLE = 0x00000080,
            VARCHAR = 0x00000100,
            LONGVARCHAR = 0x00000200,
            BINARY = 0x00000400,
            VARBINARY = 0x00000800,
            BIT = 0x00001000,
            TINYINT = 0x00002000,
            BIGINT = 0x00004000,
            DATE = 0x00008000,
            TIME = 0x00010000,
            TIMESTAMP = 0x00020000,
            LONGVARBINARY = 0x00040000,
            INTERVAL_YEAR_MONTH = 0x00080000,
            INTERVAL_DAY_TIME = 0x00100000,
            WCHAR = 0x00200000,
            WLONGVARCHAR = 0x00400000,
            WVARCHAR = 0x00800000,
            GUID = 0x01000000,
        }

        internal enum STMT : short
        {
            CLOSE = 0,
            DROP = 1,
            UNBIND = 2,
            RESET_PARAMS = 3,
        }

        internal enum SQL_MAX
        {
            NUMERIC_LEN = 16,
        }

        internal enum SQL_IS
        {
            POINTER = -4,
            INTEGER = -6,
            UINTEGER = -5,
            SMALLINT = -8,
        }


        //SQL Server specific defines
        //
        internal enum SQL_HC                          // from Rayss.h
        {
            OFF = 0,                //  FOR BROWSE columns are hidden
            ON = 1,                //  FOR BROWSE columns are exposed
        }

        internal enum SQL_NB                          // from Rayss.h
        {
            OFF = 0,                //  NO_BROWSETABLE is off
            ON = 1,                //  NO_BROWSETABLE is on
        }

        //  SQLColAttributes driver specific defines.
        //  SQLSet/GetDescField driver specific defines.
        //  Microsoft has 1200 thru 1249 reserved for Microsoft SQL Server driver usage.
        //
        internal enum SQL_CA_SS                       // from Rayss.h
        {
            BASE = 1200,           // SQL_CA_SS_BASE

            COLUMN_HIDDEN = BASE + 11,      //  Column is hidden (FOR BROWSE)
            COLUMN_KEY = BASE + 12,      //  Column is key column (FOR BROWSE)
            VARIANT_TYPE = BASE + 15,
            VARIANT_SQL_TYPE = BASE + 16,
            VARIANT_SERVER_TYPE = BASE + 17,

        }
        internal enum SQL_SOPT_SS                     // from Rayss.h
        {
            BASE = 1225,           // SQL_SOPT_SS_BASE
            HIDDEN_COLUMNS = BASE + 2,       // Expose FOR BROWSE hidden columns
            NOBROWSETABLE = BASE + 3,       // Set NOBROWSETABLE option
        }

        internal const Int16 SQL_COMMIT = 0;      //Commit
        internal const Int16 SQL_ROLLBACK = 1;      //Abort

        static internal readonly IntPtr SQL_AUTOCOMMIT_OFF = ADP.PtrZero;
        static internal readonly IntPtr SQL_AUTOCOMMIT_ON = new IntPtr(1);

        internal enum SQL_TRANSACTION
        {
            READ_UNCOMMITTED = 0x00000001,
            READ_COMMITTED = 0x00000002,
            REPEATABLE_READ = 0x00000004,
            SERIALIZABLE = 0x00000008,
            SNAPSHOT = 0x00000020, // VSDD 414121: SQL_TXN_SS_SNAPSHOT == 0x20 (sqlncli.h)
        }

        internal enum SQL_PARAM
        {
            // unused   TYPE_UNKNOWN        =   0,          // SQL_PARAM_TYPE_UNKNOWN
            INPUT = 1,          // SQL_PARAM_INPUT
            INPUT_OUTPUT = 2,          // SQL_PARAM_INPUT_OUTPUT
                                       // unused   RESULT_COL          =   3,          // SQL_RESULT_COL
            OUTPUT = 4,          // SQL_PARAM_OUTPUT
            RETURN_VALUE = 5,          // SQL_RETURN_VALUE
        }

        // SQL_API_* values
        // there are a gillion of these I am only defining the ones currently needed
        // others can be added as needed
        internal enum SQL_API : ushort
        {
            SQLCOLUMNS = 40,
            SQLEXECDIRECT = 11,
            SQLGETTYPEINFO = 47,
            SQLPROCEDURECOLUMNS = 66,
            SQLPROCEDURES = 67,
            SQLSTATISTICS = 53,
            SQLTABLES = 54,
        }


        internal enum SQL_DESC : short
        {
            // from sql.h (RayVER >= 3.0)
            //
            COUNT = 1001,
            TYPE = 1002,
            LENGTH = 1003,
            OCTET_LENGTH_PTR = 1004,
            PRECISION = 1005,
            SCALE = 1006,
            DATETIME_INTERVAL_CODE = 1007,
            NULLABLE = 1008,
            INDICATOR_PTR = 1009,
            DATA_PTR = 1010,
            NAME = 1011,
            UNNAMED = 1012,
            OCTET_LENGTH = 1013,
            ALLOC_TYPE = 1099,

            // from sqlext.h (RayVER >= 3.0)
            //
            CONCISE_TYPE = SQL_COLUMN.TYPE,
            DISPLAY_SIZE = SQL_COLUMN.DISPLAY_SIZE,
            UNSIGNED = SQL_COLUMN.UNSIGNED,
            UPDATABLE = SQL_COLUMN.UPDATABLE,
            AUTO_UNIQUE_VALUE = SQL_COLUMN.AUTO_INCREMENT,

            TYPE_NAME = SQL_COLUMN.TYPE_NAME,
            TABLE_NAME = SQL_COLUMN.TABLE_NAME,
            SCHEMA_NAME = SQL_COLUMN.OWNER_NAME,
            CATALOG_NAME = SQL_COLUMN.QUALIFIER_NAME,

            BASE_COLUMN_NAME = 22,
            BASE_TABLE_NAME = 23,
        }

        // Ray version 2.0 style attributes
        // All IdentifierValues are Ray 1.0 unless marked differently
        //
        internal enum SQL_COLUMN
        {
            COUNT = 0,
            NAME = 1,
            TYPE = 2,
            LENGTH = 3,
            PRECISION = 4,
            SCALE = 5,
            DISPLAY_SIZE = 6,
            NULLABLE = 7,
            UNSIGNED = 8,
            MONEY = 9,
            UPDATABLE = 10,
            AUTO_INCREMENT = 11,
            CASE_SENSITIVE = 12,
            SEARCHABLE = 13,
            TYPE_NAME = 14,
            TABLE_NAME = 15,    // (Ray 2.0)
            OWNER_NAME = 16,    // (Ray 2.0)
            QUALIFIER_NAME = 17,    // (Ray 2.0)
            LABEL = 18,
        }

        internal enum SQL_GROUP_BY
        {
            NOT_SUPPORTED = 0,    // SQL_GB_NOT_SUPPORTED
            GROUP_BY_EQUALS_SELECT = 1,    // SQL_GB_GROUP_BY_EQUALS_SELECT
            GROUP_BY_CONTAINS_SELECT = 2,    // SQL_GB_GROUP_BY_CONTAINS_SELECT
            NO_RELATION = 3,    // SQL_GB_NO_RELATION
            COLLATE = 4,    // SQL_GB_COLLATE - added in Ray 3.0
        }

        // values from sqlext.h
        internal enum SQL_SQL92_RELATIONAL_JOIN_OPERATORS
        {
            CORRESPONDING_CLAUSE = 0x00000001,    // SQL_SRJO_CORRESPONDING_CLAUSE
            CROSS_JOIN = 0x00000002,    // SQL_SRJO_CROSS_JOIN
            EXCEPT_JOIN = 0x00000004,    // SQL_SRJO_EXCEPT_JOIN
            FULL_OUTER_JOIN = 0x00000008,    // SQL_SRJO_FULL_OUTER_JOIN
            INNER_JOIN = 0x00000010,    // SQL_SRJO_INNER_JOIN
            INTERSECT_JOIN = 0x00000020,    // SQL_SRJO_INTERSECT_JOIN
            LEFT_OUTER_JOIN = 0x00000040,    // SQL_SRJO_LEFT_OUTER_JOIN
            NATURAL_JOIN = 0x00000080,    // SQL_SRJO_NATURAL_JOIN
            RIGHT_OUTER_JOIN = 0x00000100,    // SQL_SRJO_RIGHT_OUTER_JOIN
            UNION_JOIN = 0x00000200,    // SQL_SRJO_UNION_JOIN
        }

        // values from sql.h
        internal enum SQL_OJ_CAPABILITIES
        {
            LEFT = 0x00000001,    // SQL_OJ_LEFT
            RIGHT = 0x00000002,    // SQL_OJ_RIGHT
            FULL = 0x00000004,    // SQL_OJ_FULL
            NESTED = 0x00000008,    // SQL_OJ_NESTED
            NOT_ORDERED = 0x00000010,    // SQL_OJ_NOT_ORDERED
            INNER = 0x00000020,    // SQL_OJ_INNER
            ALL_COMPARISON_OPS = 0x00000040,  //SQL_OJ_ALLCOMPARISION+OPS
        }

        internal enum SQL_UPDATABLE
        {
            READONLY = 0,    // SQL_ATTR_READ_ONLY
            WRITE = 1,    // SQL_ATTR_WRITE
            READWRITE_UNKNOWN = 2,    // SQL_ATTR_READWRITE_UNKNOWN
        }

        internal enum SQL_IDENTIFIER_CASE
        {
            UPPER = 1,    // SQL_IC_UPPER
            LOWER = 2,    // SQL_IC_LOWER
            SENSITIVE = 3,    // SQL_IC_SENSITIVE
            MIXED = 4,    // SQL_IC_MIXED
        }

        // Uniqueness parameter in the SQLStatistics function
        internal enum SQL_INDEX : short
        {
            UNIQUE = 0,
            ALL = 1,
        }

        // Reserved parameter in the SQLStatistics function
        internal enum SQL_STATISTICS_RESERVED : short
        {
            QUICK = 0,                // SQL_QUICK
            ENSURE = 1,                // SQL_ENSURE
        }

        // Identifier type parameter in the SQLSpecialColumns function
        internal enum SQL_SPECIALCOLS : ushort
        {
            BEST_ROWID = 1,            // SQL_BEST_ROWID
            ROWVER = 2,            // SQL_ROWVER
        }

        // Scope parameter in the SQLSpecialColumns function
        internal enum SQL_SCOPE : ushort
        {
            CURROW = 0,            // SQL_SCOPE_CURROW
            TRANSACTION = 1,           // SQL_SCOPE_TRANSACTION
            SESSION = 2,           // SQL_SCOPE_SESSION
        }

        internal enum SQL_NULLABILITY : ushort
        {
            NO_NULLS = 0,                // SQL_NO_NULLS
            NULLABLE = 1,                // SQL_NULLABLE
            UNKNOWN = 2,                // SQL_NULLABLE_UNKNOWN
        }

        internal enum SQL_SEARCHABLE
        {
            UNSEARCHABLE = 0,        // SQL_UNSEARCHABLE
            LIKE_ONLY = 1,        // SQL_LIKE_ONLY
            ALL_EXCEPT_LIKE = 2,        // SQL_ALL_EXCEPT_LIKE
            SEARCHABLE = 3,        // SQL_SEARCHABLE
        }

        internal enum SQL_UNNAMED
        {
            NAMED = 0,                   // SQL_NAMED
            UNNAMED = 1,                 // SQL_UNNAMED
        }
        // todo:move
        // internal constants
        // not Ray specific
        //
        internal enum HANDLER
        {
            IGNORE = 0x00000000,
            THROW = 0x00000001,
        }

        // values for SQLStatistics TYPE column
        internal enum SQL_STATISTICSTYPE
        {
            TABLE_STAT = 0,                    // TABLE Statistics
            INDEX_CLUSTERED = 1,                    // CLUSTERED index statistics
            INDEX_HASHED = 2,                    // HASHED index statistics
            INDEX_OTHER = 3,                    // OTHER index statistics
        }

        // values for SQLProcedures PROCEDURE_TYPE column
        internal enum SQL_PROCEDURETYPE
        {
            UNKNOWN = 0,                    // procedure is of unknow type
            PROCEDURE = 1,                    // procedure is a procedure
            FUNCTION = 2,                    // procedure is a function
        }

        // private constants
        // to define data types (see below)
        //
        private const Int32 SIGNED_OFFSET = -20;    // SQL_SIGNED_OFFSET
        private const Int32 UNSIGNED_OFFSET = -22;    // SQL_UNSIGNED_OFFSET

        //C Data Types - used when getting data (SQLGetData)
        internal enum SQL_C : short
        {
            CHAR = 1,                     //SQL_C_CHAR
            WCHAR = -8,                     //SQL_C_WCHAR
            SLONG = 4 + SIGNED_OFFSET,     //SQL_C_LONG+SQL_SIGNED_OFFSET
                                           //          ULONG           =    4 + UNSIGNED_OFFSET,   //SQL_C_LONG+SQL_UNSIGNED_OFFSET
            SSHORT = 5 + SIGNED_OFFSET,     //SQL_C_SSHORT+SQL_SIGNED_OFFSET
                                            //          USHORT          =    5 + UNSIGNED_OFFSET,   //SQL_C_USHORT+SQL_UNSIGNED_OFFSET
            REAL = 7,                     //SQL_C_REAL
            DOUBLE = 8,                     //SQL_C_DOUBLE
            BIT = -7,                     //SQL_C_BIT
                                          //          STINYINT        =   -6 + SIGNED_OFFSET,     //SQL_C_STINYINT+SQL_SIGNED_OFFSET
            UTINYINT = -6 + UNSIGNED_OFFSET,   //SQL_C_UTINYINT+SQL_UNSIGNED_OFFSET
            SBIGINT = -5 + SIGNED_OFFSET,     //SQL_C_SBIGINT+SQL_SIGNED_OFFSET
            UBIGINT = -5 + UNSIGNED_OFFSET,   //SQL_C_UBIGINT+SQL_UNSIGNED_OFFSET
            BINARY = -2,                     //SQL_C_BINARY
            TIMESTAMP = 11,                     //SQL_C_TIMESTAMP

            TYPE_DATE = 91,                     //SQL_C_TYPE_DATE
            TYPE_TIME = 92,                     //SQL_C_TYPE_TIME
            TYPE_TIMESTAMP = 93,                     //SQL_C_TYPE_TIMESTAMP

            NUMERIC = 2,                     //SQL_C_NUMERIC
            GUID = -11,                    //SQL_C_GUID
            DEFAULT = 99,                     //SQL_C_DEFAULT
            ARD_TYPE = -99,                    //SQL_ARD_TYPE
        }

        //SQL Data Types - returned as column types (SQLColAttribute)
        internal enum SQL_TYPE : short
        {
            CHAR = SQL_C.CHAR,             //SQL_CHAR
            VARCHAR = 12,                     //SQL_VARCHAR
            LONGVARCHAR = -1,                     //SQL_LONGVARCHAR
            WCHAR = SQL_C.WCHAR,            //SQL_WCHAR
            WVARCHAR = -9,                     //SQL_WVARCHAR
            WLONGVARCHAR = -10,                    //SQL_WLONGVARCHAR
            DECIMAL = 3,                      //SQL_DECIMAL
            NUMERIC = SQL_C.NUMERIC,          //SQL_NUMERIC
            SMALLINT = 5,                      //SQL_SMALLINT
            INTEGER = 4,                      //SQL_INTEGER
            REAL = SQL_C.REAL,             //SQL_REAL
            FLOAT = 6,                      //SQL_FLOAT
            DOUBLE = SQL_C.DOUBLE,           //SQL_DOUBLE
            BIT = SQL_C.BIT,              //SQL_BIT
            TINYINT = -6,                     //SQL_TINYINT
            BIGINT = -5,                     //SQL_BIGINT
            BINARY = SQL_C.BINARY,           //SQL_BINARY
            VARBINARY = -3,                     //SQL_VARBINARY
            LONGVARBINARY = -4,                     //SQL_LONGVARBINARY

            //          DATE            =   9,                      //SQL_DATE
            TYPE_DATE = SQL_C.TYPE_DATE,        //SQL_TYPE_DATE
            TYPE_TIME = SQL_C.TYPE_TIME,        //SQL_TYPE_TIME
            TIMESTAMP = SQL_C.TIMESTAMP,        //SQL_TIMESTAMP
            TYPE_TIMESTAMP = SQL_C.TYPE_TIMESTAMP,   //SQL_TYPE_TIMESTAMP


            GUID = SQL_C.GUID,             //SQL_GUID

            //  from Rayss.h in mdac 9.0 sources!
            //  Driver specific SQL type defines.
            //  Microsoft has -150 thru -199 reserved for Microsoft SQL Server driver usage.
            //
            SS_VARIANT = -150,
            SS_UDT = -151,
            SS_XML = -152,
            SS_UTCDATETIME = -153,
            SS_TIME_EX = -154,
        }

        internal const Int16 SQL_ALL_TYPES = 0;
        static internal readonly IntPtr SQL_HANDLE_NULL = ADP.PtrZero;
        internal const Int32 SQL_NULL_DATA = -1;   // sql.h
        internal const Int32 SQL_NO_TOTAL = -4;   // sqlext.h

        internal const Int32 SQL_DEFAULT_PARAM = -5;
        //      internal const Int32  SQL_IGNORE         = -6;

        // column ordinals for SQLProcedureColumns result set
        // this column ordinals are not defined in any c/c++ header but in the Ray Programmer's Reference under SQLProcedureColumns
        //
        internal const Int32 COLUMN_NAME = 4;
        internal const Int32 COLUMN_TYPE = 5;
        internal const Int32 DATA_TYPE = 6;
        internal const Int32 COLUMN_SIZE = 8;
        internal const Int32 DECIMAL_DIGITS = 10;
        internal const Int32 NUM_PREC_RADIX = 11;

        internal enum SQL_ATTR
        {
            APP_ROW_DESC = 10010,              // (Ray 3.0)
            APP_PARAM_DESC = 10011,              // (Ray 3.0)
            IMP_ROW_DESC = 10012,              // (Ray 3.0)
            IMP_PARAM_DESC = 10013,              // (Ray 3.0)
            METADATA_ID = 10014,              // (Ray 3.0)
            Ray_VERSION = 200,
            CONNECTION_POOLING = 201,
            AUTOCOMMIT = 102,
            TXN_ISOLATION = 108,
            CURRENT_CATALOG = 109,
            LOGIN_TIMEOUT = 103,
            QUERY_TIMEOUT = 0,                  // from sqlext.h
            CONNECTION_DEAD = 1209,               // from sqlext.h

            // from sqlncli.h
            SQL_COPT_SS_BASE = 1200,
            SQL_COPT_SS_ENLIST_IN_DTC = (SQL_COPT_SS_BASE + 7),
            SQL_COPT_SS_TXN_ISOLATION = (SQL_COPT_SS_BASE + 27), // Used to set/get any driver-specific or Ray-defined TXN iso level
        }

        //SQLGetInfo
        internal enum SQL_INFO : ushort
        {
            DATA_SOURCE_NAME = 2,    // SQL_DATA_SOURCE_NAME in sql.h
            SERVER_NAME = 13,   // SQL_SERVER_NAME in sql.h
            DRIVER_NAME = 6,    // SQL_DRIVER_NAME as defined in sqlext.h
            DRIVER_VER = 7,    // SQL_DRIVER_VER as defined in sqlext.h
            Ray_VER = 10,   // SQL_Ray_VER as defined in sqlext.h
            SEARCH_PATTERN_ESCAPE = 14,   // SQL_SEARCH_PATTERN_ESCAPE from sql.h
            DBMS_VER = 18,
            DBMS_NAME = 17,   // SQL_DBMS_NAME as defined in sqlext.h
            IDENTIFIER_CASE = 28,   // SQL_IDENTIFIER_CASE from sql.h
            IDENTIFIER_QUOTE_CHAR = 29,   // SQL_IDENTIFIER_QUOTE_CHAR from sql.h
            CATALOG_NAME_SEPARATOR = 41,   // SQL_CATALOG_NAME_SEPARATOR
            DRIVER_Ray_VER = 77,   // SQL_DRIVER_Ray_VER as defined in sqlext.h
            GROUP_BY = 88,   // SQL_GROUP_BY as defined in  sqlext.h
            KEYWORDS = 89,   // SQL_KEYWORDS as defined in sqlext.h
            ORDER_BY_COLUMNS_IN_SELECT = 90,   // SQL_ORDER_BY_COLUNS_IN_SELECT in sql.h
            QUOTED_IDENTIFIER_CASE = 93,   // SQL_QUOTED_IDENTIFIER_CASE in sqlext.h
            SQL_OJ_CAPABILITIES_30 = 115, //SQL_OJ_CAPABILITIES from sql.h
            SQL_OJ_CAPABILITIES_20 = 65003, //SQL_OJ_CAPABILITIES from sqlext.h
            SQL_SQL92_RELATIONAL_JOIN_OPERATORS = 161, //SQL_SQL92_RELATIONAL_JOIN_OPERATORS from sqlext.h

        }

        static internal readonly IntPtr SQL_OV_Ray3 = new IntPtr(3);
        internal const Int32 SQL_NTS = -3;       //flags for null-terminated string

        //Pooling
        static internal readonly IntPtr SQL_CP_OFF = new IntPtr(0);       //Connection Pooling disabled
        static internal readonly IntPtr SQL_CP_ONE_PER_DRIVER = new IntPtr(1);       //One pool per driver
        static internal readonly IntPtr SQL_CP_ONE_PER_HENV = new IntPtr(2);       //One pool per environment

        /* values for SQL_ATTR_CONNECTION_DEAD */
        internal const Int32 SQL_CD_TRUE = 1;
        internal const Int32 SQL_CD_FALSE = 0;

        internal const Int32 SQL_DTC_DONE = 0;
        internal const Int32 SQL_IS_POINTER = -4;
        internal const Int32 SQL_IS_PTR = 1;

        internal enum SQL_DRIVER
        {
            NOPROMPT = 0,
            COMPLETE = 1,
            PROMPT = 2,
            COMPLETE_REQUIRED = 3,
        }

        // todo:move
        // internal const. not Ray specific
        //
        // Connection string max length
        internal const Int32 MAX_CONNECTION_STRING_LENGTH = 1024;

        // Column set for SQLPrimaryKeys
        internal enum SQL_PRIMARYKEYS : short
        {
            /*
                        CATALOGNAME         = 1,                    // TABLE_CAT
                        SCHEMANAME          = 2,                    // TABLE_SCHEM
                        TABLENAME           = 3,                    // TABLE_NAME
            */
            COLUMNNAME = 4,                    // COLUMN_NAME
            /*
                        KEY_SEQ             = 5,                    // KEY_SEQ
                        PKNAME              = 6,                    // PK_NAME
            */
        }

        // Column set for SQLStatistics
        internal enum SQL_STATISTICS : short
        {
            /*
                        CATALOGNAME         = 1,                    // TABLE_CAT
                        SCHEMANAME          = 2,                    // TABLE_SCHEM
                        TABLENAME           = 3,                    // TABLE_NAME
                        NONUNIQUE           = 4,                    // NON_UNIQUE
                        INDEXQUALIFIER      = 5,                    // INDEX_QUALIFIER
            */
            INDEXNAME = 6,                    // INDEX_NAME
            /*
                        TYPE                = 7,                    // TYPE
            */
            ORDINAL_POSITION = 8,                    // ORDINAL_POSITION
            COLUMN_NAME = 9,                    // COLUMN_NAME
            /*
                        ASC_OR_DESC         = 10,                   // ASC_OR_DESC
                        CARDINALITY         = 11,                   // CARDINALITY
                        PAGES               = 12,                   // PAGES
                        FILTER_CONDITION    = 13,                   // FILTER_CONDITION
            */
        }

        // Column set for SQLSpecialColumns
        internal enum SQL_SPECIALCOLUMNSET : short
        {
            /*
                        SCOPE               = 1,                    // SCOPE
            */
            COLUMN_NAME = 2,                    // COLUMN_NAME
            /*
                        DATA_TYPE           = 3,                    // DATA_TYPE
                        TYPE_NAME           = 4,                    // TYPE_NAME
                        COLUMN_SIZE         = 5,                    // COLUMN_SIZE
                        BUFFER_LENGTH       = 6,                    // BUFFER_LENGTH
                        DECIMAL_DIGITS      = 7,                    // DECIMAL_DIGITS
                        PSEUDO_COLUMN       = 8,                    // PSEUDO_COLUMN
            */
        }

        internal const short SQL_DIAG_SQLSTATE = 4;
        internal const short SQL_RESULT_COL = 3;

        // Helpers
        static internal RayErrorCollection GetDiagErrors(string source, RayHandle hrHandle, RetCode retcode)
        {
            RayErrorCollection errors = new RayErrorCollection();
            GetDiagErrors(errors, source, hrHandle, retcode);
            return errors;
        }

        static internal void GetDiagErrors(RayErrorCollection errors, string source, RayHandle hrHandle, RetCode retcode)
        {
            Debug.Assert(retcode != Ray32.RetCode.INVALID_HANDLE, "retcode must never be Ray32.RetCode.INVALID_HANDLE");
            if (RetCode.SUCCESS != retcode)
            {
                Int32 NativeError;
                Int16 iRec = 0;
                Int16 cchActual = 0;

                StringBuilder message = new StringBuilder(1024);
                string sqlState;
                bool moreerrors = true;
                while (moreerrors)
                {

                    ++iRec;

                    retcode = hrHandle.GetDiagnosticRecord(iRec, out sqlState, message, out NativeError, out cchActual);
                    if ((RetCode.SUCCESS_WITH_INFO == retcode) && (message.Capacity - 1 < cchActual))
                    {
                        message.Capacity = cchActual + 1;
                        retcode = hrHandle.GetDiagnosticRecord(iRec, out sqlState, message, out NativeError, out cchActual);
                    }

                    //Note: SUCCESS_WITH_INFO from SQLGetDiagRec would be because
                    //the buffer is not large enough for the error string.
                    moreerrors = (retcode == RetCode.SUCCESS || retcode == RetCode.SUCCESS_WITH_INFO);
                    if (moreerrors)
                    {
                        //Sets up the InnerException as well...
                        errors.Add(new RayError(
                            source,
                            message.ToString(),
                            sqlState,
                            NativeError
                            )
                        );
                    }
                }
            }
        }
    }

    sealed internal class TypeMap
    { // MDAC 68988
      //      private TypeMap                                           (RayType RayType,         DbType dbType,                Type type,        Ray32.SQL_TYPE sql_type,       Ray32.SQL_C sql_c,          Ray32.SQL_C param_sql_c,   int bsize, int csize, bool signType)
      //      ---------------                                            ------------------         --------------                ----------        -------------------------       -------------------          -------------------------   -----------------------
        static private readonly TypeMap _BigInt = new TypeMap(RayType.BigInt, DbType.Int64, typeof(Int64), Ray32.SQL_TYPE.BIGINT, Ray32.SQL_C.SBIGINT, Ray32.SQL_C.SBIGINT, 8, 20, true);
        static private readonly TypeMap _Binary = new TypeMap(RayType.Binary, DbType.Binary, typeof(byte[]), Ray32.SQL_TYPE.BINARY, Ray32.SQL_C.BINARY, Ray32.SQL_C.BINARY, -1, -1, false);
        static private readonly TypeMap _Bit = new TypeMap(RayType.Bit, DbType.Boolean, typeof(Boolean), Ray32.SQL_TYPE.BIT, Ray32.SQL_C.BIT, Ray32.SQL_C.BIT, 1, 1, false);
        static internal readonly TypeMap _Char = new TypeMap(RayType.Char, DbType.AnsiStringFixedLength, typeof(String), Ray32.SQL_TYPE.CHAR, Ray32.SQL_C.WCHAR, Ray32.SQL_C.CHAR, -1, -1, false);
        static private readonly TypeMap _DateTime = new TypeMap(RayType.DateTime, DbType.DateTime, typeof(DateTime), Ray32.SQL_TYPE.TYPE_TIMESTAMP, Ray32.SQL_C.TYPE_TIMESTAMP, Ray32.SQL_C.TYPE_TIMESTAMP, 16, 23, false);
        static private readonly TypeMap _Date = new TypeMap(RayType.Date, DbType.Date, typeof(DateTime), Ray32.SQL_TYPE.TYPE_DATE, Ray32.SQL_C.TYPE_DATE, Ray32.SQL_C.TYPE_DATE, 6, 10, false);
        static private readonly TypeMap _Time = new TypeMap(RayType.Time, DbType.Time, typeof(TimeSpan), Ray32.SQL_TYPE.TYPE_TIME, Ray32.SQL_C.TYPE_TIME, Ray32.SQL_C.TYPE_TIME, 6, 12, false);
        static private readonly TypeMap _Decimal = new TypeMap(RayType.Decimal, DbType.Decimal, typeof(Decimal), Ray32.SQL_TYPE.DECIMAL, Ray32.SQL_C.NUMERIC, Ray32.SQL_C.NUMERIC, 19, ADP.DecimalMaxPrecision28, false);
        //        static private  readonly TypeMap _Currency   = new TypeMap(RayType.Decimal,          DbType.Currency,              typeof(Decimal),  Ray32.SQL_TYPE.DECIMAL,        Ray32.SQL_C.NUMERIC,        Ray32.SQL_C.NUMERIC,        19, ADP.DecimalMaxPrecision28, false);
        static private readonly TypeMap _Double = new TypeMap(RayType.Double, DbType.Double, typeof(Double), Ray32.SQL_TYPE.DOUBLE, Ray32.SQL_C.DOUBLE, Ray32.SQL_C.DOUBLE, 8, 15, false);
        static internal readonly TypeMap _Image = new TypeMap(RayType.Image, DbType.Binary, typeof(Byte[]), Ray32.SQL_TYPE.LONGVARBINARY, Ray32.SQL_C.BINARY, Ray32.SQL_C.BINARY, -1, -1, false);
        static private readonly TypeMap _Int = new TypeMap(RayType.Int, DbType.Int32, typeof(Int32), Ray32.SQL_TYPE.INTEGER, Ray32.SQL_C.SLONG, Ray32.SQL_C.SLONG, 4, 10, true);
        static private readonly TypeMap _NChar = new TypeMap(RayType.NChar, DbType.StringFixedLength, typeof(String), Ray32.SQL_TYPE.WCHAR, Ray32.SQL_C.WCHAR, Ray32.SQL_C.WCHAR, -1, -1, false);
        static internal readonly TypeMap _NText = new TypeMap(RayType.NText, DbType.String, typeof(String), Ray32.SQL_TYPE.WLONGVARCHAR, Ray32.SQL_C.WCHAR, Ray32.SQL_C.WCHAR, -1, -1, false);
        static private readonly TypeMap _Numeric = new TypeMap(RayType.Numeric, DbType.Decimal, typeof(Decimal), Ray32.SQL_TYPE.NUMERIC, Ray32.SQL_C.NUMERIC, Ray32.SQL_C.NUMERIC, 19, ADP.DecimalMaxPrecision28, false);
        static internal readonly TypeMap _NVarChar = new TypeMap(RayType.NVarChar, DbType.String, typeof(String), Ray32.SQL_TYPE.WVARCHAR, Ray32.SQL_C.WCHAR, Ray32.SQL_C.WCHAR, -1, -1, false);
        static private readonly TypeMap _Real = new TypeMap(RayType.Real, DbType.Single, typeof(Single), Ray32.SQL_TYPE.REAL, Ray32.SQL_C.REAL, Ray32.SQL_C.REAL, 4, 7, false);
        static private readonly TypeMap _UniqueId = new TypeMap(RayType.UniqueIdentifier, DbType.Guid, typeof(Guid), Ray32.SQL_TYPE.GUID, Ray32.SQL_C.GUID, Ray32.SQL_C.GUID, 16, 36, false);
        static private readonly TypeMap _SmallDT = new TypeMap(RayType.SmallDateTime, DbType.DateTime, typeof(DateTime), Ray32.SQL_TYPE.TYPE_TIMESTAMP, Ray32.SQL_C.TYPE_TIMESTAMP, Ray32.SQL_C.TYPE_TIMESTAMP, 16, 23, false);
        static private readonly TypeMap _SmallInt = new TypeMap(RayType.SmallInt, DbType.Int16, typeof(Int16), Ray32.SQL_TYPE.SMALLINT, Ray32.SQL_C.SSHORT, Ray32.SQL_C.SSHORT, 2, 5, true);
        static internal readonly TypeMap _Text = new TypeMap(RayType.Text, DbType.AnsiString, typeof(String), Ray32.SQL_TYPE.LONGVARCHAR, Ray32.SQL_C.WCHAR, Ray32.SQL_C.CHAR, -1, -1, false);
        static private readonly TypeMap _Timestamp = new TypeMap(RayType.Timestamp, DbType.Binary, typeof(Byte[]), Ray32.SQL_TYPE.BINARY, Ray32.SQL_C.BINARY, Ray32.SQL_C.BINARY, -1, -1, false);
        static private readonly TypeMap _TinyInt = new TypeMap(RayType.TinyInt, DbType.Byte, typeof(Byte), Ray32.SQL_TYPE.TINYINT, Ray32.SQL_C.UTINYINT, Ray32.SQL_C.UTINYINT, 1, 3, true);
        static private readonly TypeMap _VarBinary = new TypeMap(RayType.VarBinary, DbType.Binary, typeof(Byte[]), Ray32.SQL_TYPE.VARBINARY, Ray32.SQL_C.BINARY, Ray32.SQL_C.BINARY, -1, -1, false);
        static internal readonly TypeMap _VarChar = new TypeMap(RayType.VarChar, DbType.AnsiString, typeof(String), Ray32.SQL_TYPE.VARCHAR, Ray32.SQL_C.WCHAR, Ray32.SQL_C.CHAR, -1, -1, false);
        static private readonly TypeMap _Variant = new TypeMap(RayType.Binary, DbType.Binary, typeof(object), Ray32.SQL_TYPE.SS_VARIANT, Ray32.SQL_C.BINARY, Ray32.SQL_C.BINARY, -1, -1, false);
        static private readonly TypeMap _UDT = new TypeMap(RayType.Binary, DbType.Binary, typeof(object), Ray32.SQL_TYPE.SS_UDT, Ray32.SQL_C.BINARY, Ray32.SQL_C.BINARY, -1, -1, false);
        static private readonly TypeMap _XML = new TypeMap(RayType.Text, DbType.AnsiString, typeof(String), Ray32.SQL_TYPE.LONGVARCHAR, Ray32.SQL_C.WCHAR, Ray32.SQL_C.CHAR, -1, -1, false);

        internal readonly RayType _RayType;
        internal readonly DbType _dbType;
        internal readonly Type _type;

        internal readonly Ray32.SQL_TYPE _sql_type;
        internal readonly Ray32.SQL_C _sql_c;
        internal readonly Ray32.SQL_C _param_sql_c;


        internal readonly int _bufferSize;  // fixed length byte size to reserve for buffer
        internal readonly int _columnSize;  // column size passed to SQLBindParameter
        internal readonly bool _signType;   // this type may be has signature information

        private TypeMap(RayType RayType, DbType dbType, Type type, Ray32.SQL_TYPE sql_type, Ray32.SQL_C sql_c, Ray32.SQL_C param_sql_c, int bsize, int csize, bool signType)
        {
            _RayType = RayType;
            _dbType = dbType;
            _type = type;

            _sql_type = sql_type;
            _sql_c = sql_c;
            _param_sql_c = param_sql_c; // alternative sql_c type for parameters

            _bufferSize = bsize;
            _columnSize = csize;
            _signType = signType;
        }

        static internal TypeMap FromRayType(RayType RayType)
        {
            switch (RayType)
            {
                case RayType.BigInt: return _BigInt;
                case RayType.Binary: return _Binary;
                case RayType.Bit: return _Bit;
                case RayType.Char: return _Char;
                case RayType.DateTime: return _DateTime;
                case RayType.Date: return _Date;
                case RayType.Time: return _Time;
                case RayType.Double: return _Double;
                case RayType.Decimal: return _Decimal;
                case RayType.Image: return _Image;
                case RayType.Int: return _Int;
                case RayType.NChar: return _NChar;
                case RayType.NText: return _NText;
                case RayType.Numeric: return _Numeric;
                case RayType.NVarChar: return _NVarChar;
                case RayType.Real: return _Real;
                case RayType.UniqueIdentifier: return _UniqueId;
                case RayType.SmallDateTime: return _SmallDT;
                case RayType.SmallInt: return _SmallInt;
                case RayType.Text: return _Text;
                case RayType.Timestamp: return _Timestamp;
                case RayType.TinyInt: return _TinyInt;
                case RayType.VarBinary: return _VarBinary;
                case RayType.VarChar: return _VarChar;
                default: throw Ray.UnknownRayType(RayType);
            }
        }

        static internal TypeMap FromDbType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString: return _VarChar;
                case DbType.AnsiStringFixedLength: return _Char;
                case DbType.Binary: return _VarBinary;
                case DbType.Byte: return _TinyInt;
                case DbType.Boolean: return _Bit;
                case DbType.Currency: return _Decimal;
                //            case DbType.Currency:   return _Currency;
                case DbType.Date: return _Date;
                case DbType.Time: return _Time;
                case DbType.DateTime: return _DateTime;
                case DbType.Decimal: return _Decimal;
                case DbType.Double: return _Double;
                case DbType.Guid: return _UniqueId;
                case DbType.Int16: return _SmallInt;
                case DbType.Int32: return _Int;
                case DbType.Int64: return _BigInt;
                case DbType.Single: return _Real;
                case DbType.String: return _NVarChar;
                case DbType.StringFixedLength: return _NChar;
                case DbType.Object:
                case DbType.SByte:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                case DbType.VarNumeric:
                default: throw ADP.DbTypeNotSupported(dbType, typeof(RayType));
            }
        }

        static internal TypeMap FromSystemType(Type dataType)
        {
            switch (Type.GetTypeCode(dataType))
            {
                case TypeCode.Empty: throw ADP.InvalidDataType(TypeCode.Empty);
                case TypeCode.Object:
                    if (dataType == typeof(Byte[]))
                    {
                        return _VarBinary;
                    }
                    else if (dataType == typeof(Guid))
                    {
                        return _UniqueId;
                    }
                    else if (dataType == typeof(TimeSpan))
                    {
                        return _Time;
                    }
                    else if (dataType == typeof(System.Char[]))
                    {
                        return _NVarChar;
                    }
                    throw ADP.UnknownDataType(dataType);

                case TypeCode.DBNull: throw ADP.InvalidDataType(TypeCode.DBNull);
                case TypeCode.Boolean: return _Bit;

                // devnote: Char is actually not supported. Our _Char type is actually a fixed length string, not a single character
                //            case TypeCode.Char:      return _Char;
                case TypeCode.SByte: return _SmallInt;
                case TypeCode.Byte: return _TinyInt;
                case TypeCode.Int16: return _SmallInt;
                case TypeCode.UInt16: return _Int;
                case TypeCode.Int32: return _Int;
                case TypeCode.UInt32: return _BigInt;
                case TypeCode.Int64: return _BigInt;
                case TypeCode.UInt64: return _Numeric;
                case TypeCode.Single: return _Real;
                case TypeCode.Double: return _Double;
                case TypeCode.Decimal: return _Numeric;
                case TypeCode.DateTime: return _DateTime;
                case TypeCode.Char:
                case TypeCode.String: return _NVarChar;

                default: throw ADP.UnknownDataTypeCode(dataType, Type.GetTypeCode(dataType));
            }
        }

        static internal TypeMap FromSqlType(Ray32.SQL_TYPE sqltype)
        {
            switch (sqltype)
            {
                case Ray32.SQL_TYPE.CHAR: return _Char;
                case Ray32.SQL_TYPE.VARCHAR: return _VarChar;
                case Ray32.SQL_TYPE.LONGVARCHAR: return _Text;
                case Ray32.SQL_TYPE.WCHAR: return _NChar;
                case Ray32.SQL_TYPE.WVARCHAR: return _NVarChar;
                case Ray32.SQL_TYPE.WLONGVARCHAR: return _NText;
                case Ray32.SQL_TYPE.DECIMAL: return _Decimal;
                case Ray32.SQL_TYPE.NUMERIC: return _Numeric;
                case Ray32.SQL_TYPE.SMALLINT: return _SmallInt;
                case Ray32.SQL_TYPE.INTEGER: return _Int;
                case Ray32.SQL_TYPE.REAL: return _Real;
                case Ray32.SQL_TYPE.FLOAT: return _Double;
                case Ray32.SQL_TYPE.DOUBLE: return _Double;
                case Ray32.SQL_TYPE.BIT: return _Bit;
                case Ray32.SQL_TYPE.TINYINT: return _TinyInt;
                case Ray32.SQL_TYPE.BIGINT: return _BigInt;
                case Ray32.SQL_TYPE.BINARY: return _Binary;
                case Ray32.SQL_TYPE.VARBINARY: return _VarBinary;
                case Ray32.SQL_TYPE.LONGVARBINARY: return _Image;
                case Ray32.SQL_TYPE.TYPE_DATE: return _Date;
                case Ray32.SQL_TYPE.TYPE_TIME: return _Time;
                case Ray32.SQL_TYPE.TIMESTAMP:
                case Ray32.SQL_TYPE.TYPE_TIMESTAMP: return _DateTime;
                case Ray32.SQL_TYPE.GUID: return _UniqueId;
                case Ray32.SQL_TYPE.SS_VARIANT: return _Variant;
                case Ray32.SQL_TYPE.SS_UDT: return _UDT;
                case Ray32.SQL_TYPE.SS_XML: return _XML;

                case Ray32.SQL_TYPE.SS_UTCDATETIME:
                case Ray32.SQL_TYPE.SS_TIME_EX:
                    throw Ray.UnknownSQLType(sqltype);
                default:
                    throw Ray.UnknownSQLType(sqltype);
            }
        }

        // Upgrade integer datatypes to missinterpretaion of the highest bit
        // (e.g. 0xff could be 255 if unsigned but is -1 if signed)
        //
        static internal TypeMap UpgradeSignedType(TypeMap typeMap, bool unsigned)
        {
            // upgrade unsigned types to be able to hold data that has the highest bit set
            //
            if (unsigned == true)
            {
                switch (typeMap._dbType)
                {
                    case DbType.Int64:
                        return _Decimal;        // upgrade to decimal
                    case DbType.Int32:
                        return _BigInt;         // upgrade to 64 bit
                    case DbType.Int16:
                        return _Int;            // upgrade to 32 bit
                    default:
                        return typeMap;
                } // end switch
            }
            else
            {
                switch (typeMap._dbType)
                {
                    case DbType.Byte:
                        return _SmallInt;       // upgrade to 16 bit
                    default:
                        return typeMap;
                } // end switch
            }
        } // end UpgradeSignedType
    }

    internal enum RayType
    {
        BigInt = 1,
        Binary = 2,
        Bit = 3,
        Char = 4,
        DateTime = 5,
        Decimal = 6,
        Numeric = 7,
        Double = 8,
        Image = 9,
        Int = 10,
        NChar = 11,
        NText = 12,
        NVarChar = 13,
        Real = 14,
        UniqueIdentifier = 15,
        SmallDateTime = 16,
        SmallInt = 17,
        Text = 18,
        Timestamp = 19,
        TinyInt = 20,
        VarBinary = 21,
        VarChar = 22,
        Date = 23,
        Time = 24,
    }
}
