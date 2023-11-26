using System.ComponentModel;
using System.Data.Common;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Globalization;
using System.Security.Permissions;
using System.Text;
using SysTx = System.Transactions;


namespace BlackHole.Ray
{
    [DefaultEvent("InfoMessage")]
    internal partial class RayConnection : CONNECTIONOBJECTNAME, ICloneable
    {
        private int connectionTimeout = 60;
        private RayInfoMessageEventHandler infoMessageEventHandler;
        //public RayConnectionFactory ConnectionFactory;

        //private RayConnectionOpen InnerConnection;
        private WeakReference? weakTransaction;
        //private DbConnectionPoolGroup PoolGroup;

        private RayConnectionHandle? _connectionHandle;
        private ConnectionState _extraState;    // extras, like Executing and Fetching, that we add to the State.

        internal RayConnection(string connectionString)
        {
            //ConnectionString = connectionString;
            //ConnectionFactory = RayConnectionFactory.SingletonInstance;
            //Open();
        }

        internal RayConnection(string connectionString, int timeout)
        {
            ConnectionString = connectionString;
            ConnectionTimeout = timeout;
            //InnerConnection = this;
        }

        internal RayConnectionHandle? ConnectionHandle
        {
            get
            {
                return _connectionHandle;
            }
            set
            {
                Debug.Assert(null == _connectionHandle, "reopening a connection?");
                _connectionHandle = value;
            }
        }

        [DefaultValue(""),SettingsBindable(true),RefreshProperties(RefreshProperties.All)]
        override public string ConnectionString
        {
            get
            {
                return ConnectionString;
            }
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
            set => ConnectionString = value ?? string.Empty;
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        }


        [DefaultValue(60),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        new public int ConnectionTimeout
        {
            get
            {
                return connectionTimeout;
            }
            set
            {
                if (!IsOpen)
                {
                    if (value < 30)
                    {
                        ConnectionTimeout = 30;
                    }
                    else
                    {
                        connectionTimeout = value;
                    }
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        override public string Database
        {
            get
            {
                if (IsOpen && !ProviderInfo.NoCurrentCatalog)
                {
                    //Note: CURRENT_CATALOG may not be supported by the current driver.  In which
                    //case we ignore any error (without throwing), and just return string.empty.
                    //As we really don't want people to have to have try/catch around simple properties
                    return GetConnectAttrString(Ray32.SQL_ATTR.CURRENT_CATALOG);
                }
                //Database is not available before open, and its not worth parsing the
                //connection string over.
                return String.Empty;
            }
        }

        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        ]
        override public string DataSource
        {
            get
            {
                if (IsOpen)
                {
                    // note: This will return an empty string if the driver keyword was used to connect
                    // see Ray3.0 Programmers Reference, SQLGetInfo
                    //
                    return GetInfoStringUnhandled(Ray32.SQL_INFO.SERVER_NAME, true);
                }
                return String.Empty;
            }
        }

        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        ]
        override public string ServerVersion
        {
            get
            {
                return InnerConnection.ServerVersion;
            }
        }

        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        ]
        override public ConnectionState State
        {
            get
            {
                return InnerConnection.State;
            }
        }

        internal RayConnectionPoolGroupProviderInfo ProviderInfo
        {
            get
            {
                Debug.Assert(null != this.PoolGroup, "PoolGroup must never be null when accessing ProviderInfo");
                return (RayConnectionPoolGroupProviderInfo)this.PoolGroup.ProviderInfo;
            }
        }

        internal ConnectionState InternalState
        {
            get
            {
                return (this.State | _extraState);
            }
        }

        internal bool IsOpen
        {
            get
            {
                return (InnerConnection is RayConnectionOpen);
            }
        }

        internal RayTransaction LocalTransaction
        {

            get
            {
                RayTransaction result = null;
                if (null != weakTransaction)
                {
                    result = ((RayTransaction)weakTransaction.Target);
                }
                return result;
            }

            set
            {
                weakTransaction = null;

                if (null != value)
                {
                    weakTransaction = new WeakReference((RayTransaction)value);
                }
            }
        }

        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        ]
        public string Driver
        {
            get
            {
                if (IsOpen)
                {
                    if (ProviderInfo.DriverName == null)
                    {
                        ProviderInfo.DriverName = GetInfoStringUnhandled(Ray32.SQL_INFO.DRIVER_NAME);
                    }
                    return ProviderInfo.DriverName;
                }
                return string.Empty;
            }
        }

        internal bool IsV3Driver
        {
            get
            {
                if (ProviderInfo.DriverVersion == null)
                {
                    ProviderInfo.DriverVersion = GetInfoStringUnhandled(Ray32.SQL_INFO.DRIVER_Ray_VER);
                    // protected against null and index out of range. Number cannot be bigger than 99
                    if (ProviderInfo.DriverVersion != null && ProviderInfo.DriverVersion.Length >= 2)
                    {
                        try
                        {   // mdac 89269: driver may return malformatted string
                            ProviderInfo.IsV3Driver = (int.Parse(ProviderInfo.DriverVersion.Substring(0, 2), CultureInfo.InvariantCulture) >= 3);
                        }
                        catch (System.FormatException e)
                        {
                            ProviderInfo.IsV3Driver = false;
                            //ADP.TraceExceptionWithoutRethrow(e);
                        }
                    }
                    else
                    {
                        ProviderInfo.DriverVersion = "";
                    }
                }
                return ProviderInfo.IsV3Driver;
            }
        }

        public event RayInfoMessageEventHandler InfoMessage
        {
            add
            {
                infoMessageEventHandler += value;
            }
            remove
            {
                infoMessageEventHandler -= value;
            }
        }

        internal char EscapeChar(string method)
        {
            CheckState(method);
            if (!ProviderInfo.HasEscapeChar)
            {
                string escapeCharString;
                escapeCharString = GetInfoStringUnhandled(Ray32.SQL_INFO.SEARCH_PATTERN_ESCAPE);
                Debug.Assert((escapeCharString.Length <= 1), "Can't handle multichar quotes");
                ProviderInfo.EscapeChar = (escapeCharString.Length == 1) ? escapeCharString[0] : QuoteChar(method)[0];
            }
            return ProviderInfo.EscapeChar;
        }

        internal string QuoteChar(string method)
        {
            CheckState(method);
            if (!ProviderInfo.HasQuoteChar)
            {
                string quoteCharString;
                quoteCharString = GetInfoStringUnhandled(Ray32.SQL_INFO.IDENTIFIER_QUOTE_CHAR);
                Debug.Assert((quoteCharString.Length <= 1), "Can't handle multichar quotes");
                ProviderInfo.QuoteChar = (1 == quoteCharString.Length) ? quoteCharString : "\0";
            }
            return ProviderInfo.QuoteChar;
        }

        new public RayTransaction BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.Unspecified);
        }

        new public RayTransaction BeginTransaction(IsolationLevel isolevel)
        {
            if(InnerConnection.BeginTransaction(isolevel) is RayTransaction transaction)
            {
                return transaction;
            }
            return this.Open_BeginTransaction(isolevel);
        }

        private void RollbackDeadTransaction()
        {
            WeakReference weak = weakTransaction;
            if ((null != weak) && !weak.IsAlive)
            {
                weakTransaction = null;
                ConnectionHandle.CompleteTransaction(Ray32.SQL_ROLLBACK);
            }
        }

        override public void ChangeDatabase(string value)
        {
            InnerConnection.ChangeDatabase(value);
        }

        internal void CheckState(string method)
        {
            ConnectionState state = InternalState;
            if (ConnectionState.Open != state)
            {
                //throw ADP.OpenConnectionRequired(method, state); // MDAC 68323
            }
        }

        //object ICloneable.Clone()
        //{
        //    RayConnection clone = new RayConnection(this);
        //    Bid.Trace("<Ray.RayConnection.Clone|API> %d#, clone=%d#\n", ObjectID, clone.ObjectID);
        //    return clone;
        //}

        internal bool ConnectionIsAlive(Exception innerException)
        {
            if (IsOpen)
            {
                if (!ProviderInfo.NoConnectionDead)
                {
                    int isDead = GetConnectAttr(Ray32.SQL_ATTR.CONNECTION_DEAD, Ray32.HANDLER.IGNORE);
                    if (Ray32.SQL_CD_TRUE == isDead)
                    {
                        Close();
                        //throw ADP.ConnectionIsDisabled(innerException);
                    }
                }
                // else connection is still alive or attribute not supported
                return true;
            }
            return false;
        }

        new public RayCommand CreateCommand()
        {
            return new RayCommand(String.Empty, this);
        }

        internal RayStatementHandle CreateStatementHandle()
        {
            return new RayStatementHandle(ConnectionHandle);
        }

        override public void Close()
        {
            InnerConnection.CloseConnection(this, ConnectionFactory);

            RayConnectionHandle connectionHandle = _connectionHandle;

            if (null != connectionHandle)
            {
                _connectionHandle = null;

                // If there is a pending transaction, automatically rollback.
                WeakReference weak = this.weakTransaction;
                if (null != weak)
                {
                    this.weakTransaction = null;
                    IDisposable transaction = weak.Target as RayTransaction;
                    if ((null != transaction) && weak.IsAlive)
                    {
                        transaction.Dispose();
                    }
                    // else transaction will be rolled back when handle is disposed
                }
                connectionHandle.Dispose();
            }
        }

        private void DisposeMe(bool disposing)
        { // MDAC 65459
        }

        internal string GetConnectAttrString(Ray32.SQL_ATTR attribute)
        {
            string value = "";
            Int32 cbActual = 0;
            byte[] buffer = new byte[100];
            RayConnectionHandle connectionHandle = ConnectionHandle;
            if (null != connectionHandle)
            {
                Ray32.RetCode retcode = connectionHandle.GetConnectionAttribute(attribute, buffer, out cbActual);
                if (buffer.Length + 2 <= cbActual)
                {
                    // 2 bytes for unicode null-termination character
                    // retry with cbActual because original buffer was too small
                    buffer = new byte[cbActual + 2];
                    retcode = connectionHandle.GetConnectionAttribute(attribute, buffer, out cbActual);
                }
                if ((Ray32.RetCode.SUCCESS == retcode) || (Ray32.RetCode.SUCCESS_WITH_INFO == retcode))
                {
                    value = Encoding.Unicode.GetString(buffer, 0, Math.Min(cbActual, buffer.Length));
                }
                else if (retcode == Ray32.RetCode.ERROR)
                {
                    string sqlstate = GetDiagSqlState();
                    if (("HYC00" == sqlstate) || ("HY092" == sqlstate) || ("IM001" == sqlstate))
                    {
                        FlagUnsupportedConnectAttr(attribute);
                    }
                    // not throwing errors if not supported or other failure
                }
            }
            return value;
        }

        internal int GetConnectAttr(Ray32.SQL_ATTR attribute, Ray32.HANDLER handler)
        {
            Int32 retval = -1;
            Int32 cbActual = 0;
            byte[] buffer = new byte[4];
            RayConnectionHandle connectionHandle = ConnectionHandle;
            if (null != connectionHandle)
            {
                Ray32.RetCode retcode = connectionHandle.GetConnectionAttribute(attribute, buffer, out cbActual);

                if ((Ray32.RetCode.SUCCESS == retcode) || (Ray32.RetCode.SUCCESS_WITH_INFO == retcode))
                {
                    retval = BitConverter.ToInt32(buffer, 0);
                }
                else
                {
                    if (retcode == Ray32.RetCode.ERROR)
                    {
                        string sqlstate = GetDiagSqlState();
                        if (("HYC00" == sqlstate) || ("HY092" == sqlstate) || ("IM001" == sqlstate))
                        {
                            FlagUnsupportedConnectAttr(attribute);
                        }
                    }
                    if (handler == Ray32.HANDLER.THROW)
                    {
                        this.HandleError(connectionHandle, retcode);
                    }
                }
            }
            return retval;
        }

        private string GetDiagSqlState()
        {
            RayConnectionHandle connectionHandle = ConnectionHandle;
            string sqlstate;
            connectionHandle.GetDiagnosticField(out sqlstate);
            return sqlstate;
        }

        internal Ray32.RetCode GetInfoInt16Unhandled(Ray32.SQL_INFO info, out Int16 resultValue)
        {
            byte[] buffer = new byte[2];
            Ray32.RetCode retcode = ConnectionHandle.GetInfo1(info, buffer);
            resultValue = BitConverter.ToInt16(buffer, 0);
            return retcode;
        }

        internal Ray32.RetCode GetInfoInt32Unhandled(Ray32.SQL_INFO info, out Int32 resultValue)
        {
            byte[] buffer = new byte[4];
            Ray32.RetCode retcode = ConnectionHandle.GetInfo1(info, buffer);
            resultValue = BitConverter.ToInt32(buffer, 0);
            return retcode;
        }

        private Int32 GetInfoInt32Unhandled(Ray32.SQL_INFO infotype)
        {
            byte[] buffer = new byte[4];
            ConnectionHandle.GetInfo1(infotype, buffer);
            return BitConverter.ToInt32(buffer, 0);
        }

        internal string GetInfoStringUnhandled(Ray32.SQL_INFO info)
        {
            return GetInfoStringUnhandled(info, false);
        }

        private string GetInfoStringUnhandled(Ray32.SQL_INFO info, bool handleError)
        {
            //SQLGetInfo
            string value = null;
            Int16 cbActual = 0;
            byte[] buffer = new byte[100];
            RayConnectionHandle connectionHandle = ConnectionHandle;
            if (null != connectionHandle)
            {
                Ray32.RetCode retcode = connectionHandle.GetInfo2(info, buffer, out cbActual);
                if (buffer.Length < cbActual - 2)
                {
                    // 2 bytes for unicode null-termination character
                    // retry with cbActual because original buffer was too small
                    buffer = new byte[cbActual + 2];
                    retcode = connectionHandle.GetInfo2(info, buffer, out cbActual);
                }
                if (retcode == Ray32.RetCode.SUCCESS || retcode == Ray32.RetCode.SUCCESS_WITH_INFO)
                {
                    value = Encoding.Unicode.GetString(buffer, 0, Math.Min(cbActual, buffer.Length));
                }
                else if (handleError)
                {
                    this.HandleError(ConnectionHandle, retcode);
                }
            }
            else if (handleError)
            {
                value = "";
            }
            return value;
        }

        // non-throwing HandleError
        internal Exception HandleErrorNoThrow(RayHandle hrHandle, Ray32.RetCode retcode)
        {

            Debug.Assert(retcode != Ray32.RetCode.INVALID_HANDLE, "retcode must never be Ray32.RetCode.INVALID_HANDLE");

            switch (retcode)
            {
                case Ray32.RetCode.SUCCESS:
                    break;
                case Ray32.RetCode.SUCCESS_WITH_INFO:
                    {
                        //Optimize to only create the event objects and obtain error info if
                        //the user is really interested in retriveing the events...
                        if (infoMessageEventHandler != null)
                        {
                            RayErrorCollection errors = Ray32.GetDiagErrors(null, hrHandle, retcode);
                            errors.SetSource(this.Driver);
                            OnInfoMessage(new RayInfoMessageEventArgs(errors));
                        }
                        break;
                    }
                default:
                    RayException e = RayException.CreateException(Ray32.GetDiagErrors(null, hrHandle, retcode), retcode);
                    if (e != null)
                    {
                        e.Errors.SetSource(this.Driver);
                    }
                    ConnectionIsAlive(e);        // this will close and throw if the connection is dead
                    return (Exception)e;
            }
            return null;
        }

        internal void HandleError(RayHandle hrHandle, Ray32.RetCode retcode)
        {
            Exception e = HandleErrorNoThrow(hrHandle, retcode);
            switch (retcode)
            {
                case Ray32.RetCode.SUCCESS:
                case Ray32.RetCode.SUCCESS_WITH_INFO:
                    Debug.Assert(null == e, "success exception");
                    break;
                default:
                    Debug.Assert(null != e, "failure without exception");
                    throw e;
            }
        }

        override public void Open()
        {
            InnerConnection.OpenConnection(this, ConnectionFactory);

            // SQLBUDT #276132 - need to manually enlist in some cases, because
            // native Ray doesn't know about SysTx transactions.
            if (true)//ADP.NeedManualEnlistment())
            {
                EnlistTransaction(SysTx.Transaction.Current);
            }
        }

        private void OnInfoMessage(RayInfoMessageEventArgs args)
        {
            if (null != infoMessageEventHandler)
            {
                try
                {
                    infoMessageEventHandler(this, args);
                }
                catch (Exception e)
                {
                    // 
                    //if (!ADP.IsCatchableOrSecurityExceptionType(e))
                    //{
                    //    throw;
                    //}
                    //ADP.TraceExceptionWithoutRethrow(e);
                }
            }
        }


        internal RayTransaction SetStateExecuting(string method, RayTransaction transaction)
        { // MDAC 69003
            if (null != weakTransaction)
            { // transaction may exist
                RayTransaction weak = (weakTransaction.Target as RayTransaction);
                if (transaction != weak)
                { // transaction doesn't exist
                    if (null == transaction)
                    { // transaction exists
                        //throw ADP.TransactionRequired(method);
                    }
                    if (this != transaction.Connection)
                    {
                        // transaction can't have come from this connection
                        //throw ADP.TransactionConnectionMismatch();
                    }
                    // if transaction is zombied, we don't know the original connection
                    transaction = null; // MDAC 69264
                }
            }
            else if (null != transaction)
            { // no transaction started
                if (null != transaction.Connection)
                {
                    // transaction can't have come from this connection
                    //throw ADP.TransactionConnectionMismatch();
                }
                // if transaction is zombied, we don't know the original connection
                transaction = null; // MDAC 69264
            }
            ConnectionState state = InternalState;
            if (ConnectionState.Open != state)
            {
                NotifyWeakReference(RayReferenceCollection.Recover); // recover for a potentially finalized reader

                state = InternalState;
                if (ConnectionState.Open != state)
                {
                    if (0 != (ConnectionState.Fetching & state))
                    {
                        //throw ADP.OpenReaderExists();
                    }
                    //throw ADP.OpenConnectionRequired(method, state);
                }
            }
            return transaction;
        }

        internal void NotifyWeakReference(int message)
        {
            InnerConnection.NotifyWeakReference(message);
        }

        // This adds a type to the list of types that are supported by the driver
        // (don't need to know that for all the types)
        //

        internal void SetSupportedType(Ray32.SQL_TYPE sqltype)
        {
            Ray32.SQL_CVT sqlcvt;

            switch (sqltype)
            {
                case Ray32.SQL_TYPE.NUMERIC:
                    {
                        sqlcvt = Ray32.SQL_CVT.NUMERIC;
                        break;
                    }
                case Ray32.SQL_TYPE.WCHAR:
                    {
                        sqlcvt = Ray32.SQL_CVT.WCHAR;
                        break;
                    }
                case Ray32.SQL_TYPE.WVARCHAR:
                    {
                        sqlcvt = Ray32.SQL_CVT.WVARCHAR;
                        break;
                    }
                case Ray32.SQL_TYPE.WLONGVARCHAR:
                    {
                        sqlcvt = Ray32.SQL_CVT.WLONGVARCHAR;
                        break;
                    }
                default:
                    // other types are irrelevant at this time
                    return;
            }
            ProviderInfo.TestedSQLTypes |= (int)sqlcvt;
            ProviderInfo.SupportedSQLTypes |= (int)sqlcvt;
        }

        internal void FlagRestrictedSqlBindType(Ray32.SQL_TYPE sqltype)
        {
            Ray32.SQL_CVT sqlcvt;

            switch (sqltype)
            {
                case Ray32.SQL_TYPE.NUMERIC:
                    {
                        sqlcvt = Ray32.SQL_CVT.NUMERIC;
                        break;
                    }
                case Ray32.SQL_TYPE.DECIMAL:
                    {
                        sqlcvt = Ray32.SQL_CVT.DECIMAL;
                        break;
                    }
                default:
                    // other types are irrelevant at this time
                    return;
            }
            ProviderInfo.RestrictedSQLBindTypes |= (int)sqlcvt;
        }

        internal void FlagUnsupportedConnectAttr(Ray32.SQL_ATTR Attribute)
        {
            switch (Attribute)
            {
                case Ray32.SQL_ATTR.CURRENT_CATALOG:
                    ProviderInfo.NoCurrentCatalog = true;
                    break;
                case Ray32.SQL_ATTR.CONNECTION_DEAD:
                    ProviderInfo.NoConnectionDead = true;
                    break;
                default:
                    Debug.Assert(false, "Can't flag unknown Attribute");
                    break;
            }
        }

        internal void FlagUnsupportedStmtAttr(Ray32.SQL_ATTR Attribute)
        {
            switch (Attribute)
            {
                case Ray32.SQL_ATTR.QUERY_TIMEOUT:
                    ProviderInfo.NoQueryTimeout = true;
                    break;
                case (Ray32.SQL_ATTR)Ray32.SQL_SOPT_SS.NOBROWSETABLE:
                    ProviderInfo.NoSqlSoptSSNoBrowseTable = true;
                    break;
                case (Ray32.SQL_ATTR)Ray32.SQL_SOPT_SS.HIDDEN_COLUMNS:
                    ProviderInfo.NoSqlSoptSSHiddenColumns = true;
                    break;
                default:
                    Debug.Assert(false, "Can't flag unknown Attribute");
                    break;
            }
        }

        internal void FlagUnsupportedColAttr(Ray32.SQL_DESC v3FieldId, Ray32.SQL_COLUMN v2FieldId)
        {
            if (IsV3Driver)
            {
                switch (v3FieldId)
                {
                    case (Ray32.SQL_DESC)Ray32.SQL_CA_SS.COLUMN_KEY:
                        // SSS_WARNINGS_OFF
                        ProviderInfo.NoSqlCASSColumnKey = true;
                        break;
                    // SSS_WARNINGS_ON
                    default:
                        Debug.Assert(false, "Can't flag unknown Attribute");
                        break;
                }
            }
            else
            {
                switch (v2FieldId)
                {
                    default:
                        Debug.Assert(false, "Can't flag unknown Attribute");
                        break;
                }
            }
        }

        internal Boolean SQLGetFunctions(Ray32.SQL_API RayFunction)
        {
            //SQLGetFunctions
            Ray32.RetCode retcode;
            Int16 fExists;
            Debug.Assert((Int16)RayFunction != 0, "SQL_API_ALL_FUNCTIONS is not supported");
            RayConnectionHandle connectionHandle = ConnectionHandle;
            if (null != connectionHandle)
            {
                retcode = connectionHandle.GetFunctions(RayFunction, out fExists);
            }
            else
            {
                Debug.Assert(false, "GetFunctions called and ConnectionHandle is null (connection is disposed?)");
                throw Ray.ConnectionClosed();
            }

            if (retcode != Ray32.RetCode.SUCCESS)
                this.HandleError(connectionHandle, retcode);

            if (fExists == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        internal bool TestTypeSupport(Ray32.SQL_TYPE sqltype)
        {
            Ray32.SQL_CONVERT sqlconvert;
            Ray32.SQL_CVT sqlcvt;

            // we need to convert the sqltype to sqlconvert and sqlcvt first
            //
            switch (sqltype)
            {
                case Ray32.SQL_TYPE.NUMERIC:
                    {
                        sqlconvert = Ray32.SQL_CONVERT.NUMERIC;
                        sqlcvt = Ray32.SQL_CVT.NUMERIC;
                        break;
                    }
                case Ray32.SQL_TYPE.WCHAR:
                    {
                        sqlconvert = Ray32.SQL_CONVERT.CHAR;
                        sqlcvt = Ray32.SQL_CVT.WCHAR;
                        break;
                    }
                case Ray32.SQL_TYPE.WVARCHAR:
                    {
                        sqlconvert = Ray32.SQL_CONVERT.VARCHAR;
                        sqlcvt = Ray32.SQL_CVT.WVARCHAR;
                        break;
                    }
                case Ray32.SQL_TYPE.WLONGVARCHAR:
                    {
                        sqlconvert = Ray32.SQL_CONVERT.LONGVARCHAR;
                        sqlcvt = Ray32.SQL_CVT.WLONGVARCHAR;
                        break;
                    }
                default:
                    Debug.Assert(false, "Testing that sqltype is currently not supported");
                    return false;
            }
            // now we can check if we have already tested that type
            // if not we need to do so
            if (0 == (ProviderInfo.TestedSQLTypes & (int)sqlcvt))
            {
                int flags;

                flags = GetInfoInt32Unhandled((Ray32.SQL_INFO)sqlconvert);
                flags = flags & (int)sqlcvt;

                ProviderInfo.TestedSQLTypes |= (int)sqlcvt;
                ProviderInfo.SupportedSQLTypes |= flags;
            }

            // now check if the type is supported and return the result
            //
            return (0 != (ProviderInfo.SupportedSQLTypes & (int)sqlcvt));
        }

        internal bool TestRestrictedSqlBindType(Ray32.SQL_TYPE sqltype)
        {
            Ray32.SQL_CVT sqlcvt;
            switch (sqltype)
            {
                case Ray32.SQL_TYPE.NUMERIC:
                    {
                        sqlcvt = Ray32.SQL_CVT.NUMERIC;
                        break;
                    }
                case Ray32.SQL_TYPE.DECIMAL:
                    {
                        sqlcvt = Ray32.SQL_CVT.DECIMAL;
                        break;
                    }
                default:
                    Debug.Assert(false, "Testing that sqltype is currently not supported");
                    return false;
            }
            return (0 != (ProviderInfo.RestrictedSQLBindTypes & (int)sqlcvt));
        }

        // suppress this message - we cannot use SafeHandle here. Also, see notes in the code (VSTFDEVDIV# 560355)
        [SuppressMessage("Microsoft.Reliability", "CA2004:RemoveCallsToGCKeepAlive")]
        protected DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            IntPtr hscp;

            Bid.ScopeEnter(out hscp, "<prov.RayConnection.BeginDbTransaction|API> %d#, isolationLevel=%d{ds.IsolationLevel}", ObjectID, (int)isolationLevel);
            try
            {

                DbTransaction transaction = InnerConnection.BeginTransaction(isolationLevel);

                // VSTFDEVDIV# 560355 - InnerConnection doesn't maintain a ref on the outer connection (this) and 
                //   subsequently leaves open the possibility that the outer connection could be GC'ed before the DbTransaction
                //   is fully hooked up (leaving a DbTransaction with a null connection property). Ensure that this is reachable
                //   until the completion of BeginTransaction with KeepAlive
                GC.KeepAlive(this);

                return transaction;
            }
            finally
            {
                Bid.ScopeLeave(ref hscp);
            }
        }

        internal RayTransaction Open_BeginTransaction(IsolationLevel isolevel)
        {
            ExecutePermission.Demand();

            //CheckState(ADP.BeginTransaction); // MDAC 68323

            RollbackDeadTransaction();

            if ((null != this.weakTransaction) && this.weakTransaction.IsAlive)
            { // regression from Dispose/Finalize work
                //throw ADP.ParallelTransactionsNotSupported(this);
            }

            //Use the default for unspecified.
            switch (isolevel)
            {
                case IsolationLevel.Unspecified:
                case IsolationLevel.ReadUncommitted:
                case IsolationLevel.ReadCommitted:
                case IsolationLevel.RepeatableRead:
                case IsolationLevel.Serializable:
                case IsolationLevel.Snapshot:
                    break;
                case IsolationLevel.Chaos:
                    throw Ray.NotSupportedIsolationLevel(isolevel);
                default:
                    break;
                    //throw ADP.InvalidIsolationLevel(isolevel);
            };

            //Start the transaction
            RayConnectionHandle connectionHandle = ConnectionHandle;
            Ray32.RetCode retcode = connectionHandle.BeginTransaction(ref isolevel);
            if (retcode == Ray32.RetCode.ERROR)
            {
                HandleError(connectionHandle, retcode);
            }
            RayTransaction transaction = new RayTransaction(this, isolevel, connectionHandle);
            this.weakTransaction = new WeakReference(transaction); // MDAC 69188
            return transaction;
        }

        internal void Open_ChangeDatabase(string value)
        {
            ExecutePermission.Demand();

            //CheckState(ADP.ChangeDatabase);

            // Database name must not be null, empty or whitspace
            if ((null == value) || (0 == value.Trim().Length))
            { // MDAC 62679
                //throw ADP.EmptyDatabaseName();
            }
            if (1024 < value.Length * 2 + 2)
            {
                //throw ADP.DatabaseNameTooLong();
            }
            RollbackDeadTransaction();

            //Set the database
            RayConnectionHandle connectionHandle = ConnectionHandle;
            Ray32.RetCode retcode = connectionHandle.SetConnectionAttribute3(Ray32.SQL_ATTR.CURRENT_CATALOG, value, checked((Int32)value.Length * 2));

            if (retcode != Ray32.RetCode.SUCCESS)
            {
                HandleError(connectionHandle, retcode);
            }
        }

        internal void Open_EnlistTransaction(SysTx.Transaction transaction)
        {
            VerifyExecutePermission();

            if ((null != this.weakTransaction) && this.weakTransaction.IsAlive)
            {
                //throw ADP.LocalTransactionPresent();
            }

            SysTx.IDtcTransaction oleTxTransaction = SysTx.TransactionInterop.GetDtcTransaction(transaction);
            
            RayConnectionHandle connectionHandle = ConnectionHandle;
            Ray32.RetCode retcode;
            if (null == oleTxTransaction)
            {
                retcode = connectionHandle.SetConnectionAttribute2(Ray32.SQL_ATTR.SQL_COPT_SS_ENLIST_IN_DTC, (IntPtr)Ray32.SQL_DTC_DONE, Ray32.SQL_IS_PTR);
            }
            else
            {
                retcode = connectionHandle.SetConnectionAttribute4(Ray32.SQL_ATTR.SQL_COPT_SS_ENLIST_IN_DTC, oleTxTransaction, Ray32.SQL_IS_PTR);
            }

            if (retcode != Ray32.RetCode.SUCCESS)
            {
                HandleError(connectionHandle, retcode);
            }

            // Tell the base class about our enlistment
            ((RayConnectionOpen)InnerConnection).EnlistedTransaction = transaction;
        }

        internal string Open_GetServerVersion()
        {
            //SQLGetInfo - SQL_DBMS_VER
            return GetInfoStringUnhandled(Ray32.SQL_INFO.DBMS_VER, true);
        }

        protected override DbCommand CreateDbCommand()
        {
            throw new NotImplementedException();
        }

        protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
