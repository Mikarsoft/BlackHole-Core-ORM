using BlackHole.Ray;
using System.Diagnostics;
using System.Text;

namespace BlackHole.RayCore
{
    internal static class Ray64
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
        { 
            // must be int instead of short for Everett RayException Serializablity.
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

        static internal readonly IntPtr SQL_AUTOCOMMIT_OFF = IntPtr.Zero;
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
        static internal readonly IntPtr SQL_HANDLE_NULL = IntPtr.Zero;
        internal const Int32 SQL_NULL_DATA = -1;   // sql.h
        internal const Int32 SQL_NO_TOTAL = -4;   // sqlext.h

        internal const Int32 SQL_DEFAULT_PARAM = -5;
        //      internal const Int32  SQL_IGNORE         = -6;

        // column ordinals for SQLProcedureColumns result set
        // this column ordinals are not defined in any c/c++ header but in the Ray Programmer's Reference under SQLProcedureColumns
        //
        internal const int COLUMN_NAME = 4;
        internal const int COLUMN_TYPE = 5;
        internal const int DATA_TYPE = 6;
        internal const int COLUMN_SIZE = 8;
        internal const int DECIMAL_DIGITS = 10;
        internal const int NUM_PREC_RADIX = 11;

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
        internal const int SQL_CD_TRUE = 1;
        internal const int SQL_CD_FALSE = 0;

        internal const int SQL_DTC_DONE = 0;
        internal const int SQL_IS_POINTER = -4;
        internal const int SQL_IS_PTR = 1;

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
        internal const int MAX_CONNECTION_STRING_LENGTH = 1024;

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
            return errors;
        }

        static internal void GetDiagErrors(RayErrorCollection errors, string source, RayHandle hrHandle, RetCode retcode)
        {
            Debug.Assert(retcode != Ray64.RetCode.INVALID_HANDLE, "retcode must never be Ray32.RetCode.INVALID_HANDLE");
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

                    retcode = (RetCode)hrHandle.GetDiagnosticRecord(iRec, out sqlState, message, out NativeError, out cchActual);
                    if ((RetCode.SUCCESS_WITH_INFO == retcode) && (message.Capacity - 1 < cchActual))
                    {
                        message.Capacity = cchActual + 1;
                        retcode = (RetCode)hrHandle.GetDiagnosticRecord(iRec, out sqlState, message, out NativeError, out cchActual);
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
}
