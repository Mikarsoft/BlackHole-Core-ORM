using BlackHole.Ray;
using BlackHole.Statics;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace BlackHole.RayCore
{
    internal class RayConnection : DbConnection, ICloneable, IComponent
    {
        private string _databaseName;
        private string _databaseConnectionString;
        private string _dataSource;
        private RayInfoMessageEventHandler infoMessageEventHandler;
        private ConnectionState _state;

        internal RayConnection(string connectionString)
        {
            _state = ConnectionState.Closed;
            _databaseConnectionString = connectionString;
            _databaseName = DatabaseStatics.DatabaseName;
            _dataSource = DatabaseStatics.ServerConnection;
            infoMessageEventHandler = new RayInfoMessageEventHandler(OnInfoMessage);
        }

        [AllowNull]
        public override string ConnectionString 
        {
            get => _databaseConnectionString;
            set
            {
                _databaseConnectionString = value ?? string.Empty;
            } 
        }

        public override string DataSource => _dataSource;

        public override string ServerVersion => throw new NotImplementedException();

        public override ConnectionState State
        {
            get => _state;
        }

        public override string Database => _databaseName;

        public override void ChangeDatabase(string databaseName)
        {
            _databaseName = databaseName;
        }

        public async override void Close()
        {
            await CloseAsync();
        }

        public async override void Open()
        {
            await OpenAsync();
        }

        public override ValueTask DisposeAsync()
        {
            Dispose();
            return default;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return BeginTransaction(isolationLevel);
        }

        protected override DbCommand CreateDbCommand()
        {
            return CreateCommand();
        }

        internal void HandleError(RayHandle hrHandle, Ray64.RetCode retcode)
        {
            Exception? e = HandleErrorNoThrow(hrHandle, retcode);
            switch (retcode)
            {
                case Ray64.RetCode.SUCCESS:
                case Ray64.RetCode.SUCCESS_WITH_INFO:
                    Debug.Assert(e == null, "success exception");
                    break;
                default:
                    Debug.Assert(e != null, "failure without exception");
                    throw e;
            }
        }

        internal Exception? HandleErrorNoThrow(RayHandle hrHandle, Ray64.RetCode retcode)
        {
            Debug.Assert(retcode != Ray64.RetCode.INVALID_HANDLE, "retcode must never be Ray64.RetCode.INVALID_HANDLE");

            switch (retcode)
            {
                case Ray64.RetCode.SUCCESS:
                    break;
                case Ray64.RetCode.SUCCESS_WITH_INFO:
                    {
                        if (infoMessageEventHandler != null)
                        {
                            RayErrorCollection errors = Ray64.GetDiagErrors(string.Empty, hrHandle, retcode);
                            errors.SetSource(Driver);
                            InfoMessageHandle(new RayInfoMessageEventArgs(errors));
                        }
                        break;
                    }
                default:
                    RayException e = RayException.CreateException(Ray64.GetDiagErrors(string.Empty, hrHandle, retcode), retcode);
                    if (e != null)
                    {
                        e.Errors.SetSource(Driver);
                    }
                    ConnectionIsAlive(e);
                    return e;
            }
            return null;
        }

        internal bool ConnectionIsAlive(Exception? innerException)
        {
            if (IsOpen)
            {
                if (!ProviderInfo.NoConnectionDead)
                {
                    int isDead = GetConnectAttr(Ray64.SQL_ATTR.CONNECTION_DEAD, Ray64.HANDLER.IGNORE);
                    if (Ray64.SQL_CD_TRUE == isDead)
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

        private void InfoMessageHandle(RayInfoMessageEventArgs args)
        {
            try
            {
                infoMessageEventHandler(this, args);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void OnInfoMessage(object sender , RayInfoMessageEventArgs args)
        {
            for(int i = 0; i < args.Errors.Count; i++)
            {
                Console.WriteLine(args.Errors.GetError(i));
            }
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

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
    }
}
