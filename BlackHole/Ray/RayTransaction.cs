using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Ray
{
    public sealed class RayTransaction : DbTransaction
    {
        private RayConnection _connection;
        private IsolationLevel _isolevel = IsolationLevel.Unspecified;
        private RayConnectionHandle _handle;

        internal RayTransaction(RayConnection connection, IsolationLevel isolevel, RayConnectionHandle handle)
        {
            RayConnection.VerifyExecutePermission();

            _connection = connection;
            _isolevel = isolevel;
            _handle = handle;
        }

        new public RayConnection Connection
        { // MDAC 66655
            get
            {
                return _connection;
            }
        }

        override protected DbConnection DbConnection
        { // MDAC 66655
            get
            {
                return Connection;
            }
        }

        override public IsolationLevel IsolationLevel
        {
            get
            {
                RayConnection connection = _connection;
                if (null == connection)
                {
                    throw ADP.TransactionZombied(this);
                }

                //We need to query for the case where the user didn't set the isolevel
                //BeginTransaction(), but we should also query to see if the driver
                //"rolled" the level to a higher supported one...
                if (IsolationLevel.Unspecified == _isolevel)
                {
                    //Get the isolation level
                    int sql_iso = connection.GetConnectAttr(Ray32.SQL_ATTR.TXN_ISOLATION, Ray32.HANDLER.THROW);
                    switch ((Ray32.SQL_TRANSACTION)sql_iso)
                    {
                        case Ray32.SQL_TRANSACTION.READ_UNCOMMITTED:
                            _isolevel = IsolationLevel.ReadUncommitted;
                            break;
                        case Ray32.SQL_TRANSACTION.READ_COMMITTED:
                            _isolevel = IsolationLevel.ReadCommitted;
                            break;
                        case Ray32.SQL_TRANSACTION.REPEATABLE_READ:
                            _isolevel = IsolationLevel.RepeatableRead;
                            break;
                        case Ray32.SQL_TRANSACTION.SERIALIZABLE:
                            _isolevel = IsolationLevel.Serializable;
                            break;
                        case Ray32.SQL_TRANSACTION.SNAPSHOT:
                            _isolevel = IsolationLevel.Snapshot;
                            break;
                        default:
                            throw Ray.NoMappingForSqlTransactionLevel(sql_iso);
                    };
                }
                return _isolevel;
            }
        }

        override public void Commit()
        {
            RayConnection.ExecutePermission.Demand(); // MDAC 81476

            RayConnection connection = _connection;
            if (null == connection)
            {
                //throw ADP.TransactionZombied(this);
            }

            connection.CheckState(ADP.CommitTransaction); // MDAC 68289

            //Note: SQLEndTran success if not actually in a transaction, so we have to throw
            //since the IDbTransaciton spec indicates this is an error for the managed packages
            if (null == _handle)
            {
                throw Ray.NotInTransaction();
            }

            Ray32.RetCode retcode = _handle.CompleteTransaction(Ray32.SQL_COMMIT);
            if (retcode == Ray32.RetCode.ERROR)
            {
                //If an error has occurred, we will throw an exception in HandleError,
                //and leave the transaction active for the user to retry
                connection.HandleError(_handle, retcode);
            }

            //Transaction is complete...
            connection.LocalTransaction = null;
            _connection = null;
            _handle = null;

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RayConnectionHandle handle = _handle;
                _handle = null;
                if (null != handle)
                {
                    try
                    {
                        Ray32.RetCode retcode = handle.CompleteTransaction(Ray32.SQL_ROLLBACK);
                        if (retcode == Ray32.RetCode.ERROR)
                        {
                            //don't throw an exception here, but trace it so it can be logged
                            if (_connection != null)
                            {
                                Exception e = _connection.HandleErrorNoThrow(handle, retcode);
                                ADP.TraceExceptionWithoutRethrow(e);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // 
                        if (!ADP.IsCatchableExceptionType(e))
                        {
                            throw;
                        }
                    }
                }
                if (_connection != null)
                {
                    if (_connection.IsOpen)
                    {
                        _connection.LocalTransaction = null;
                    }
                }
                _connection = null;
                _isolevel = IsolationLevel.Unspecified;
            }
            base.Dispose(disposing);
        }

        override public void Rollback()
        {
            RayConnection connection = _connection;
            if (null == connection)
            {
                throw ADP.TransactionZombied(this);
            }
            connection.CheckState(ADP.RollbackTransaction); // MDAC 68289

            //Note: SQLEndTran success if not actually in a transaction, so we have to throw
            //since the IDbTransaciton spec indicates this is an error for the managed packages
            if (null == _handle)
            {
                throw Ray.NotInTransaction();
            }

            Ray32.RetCode retcode = _handle.CompleteTransaction(Ray32.SQL_ROLLBACK);
            if (retcode == Ray32.RetCode.ERROR)
            {
                //If an error has occurred, we will throw an exception in HandleError,
                //and leave the transaction active for the user to retry
                connection.HandleError(_handle, retcode);
            }
            connection.LocalTransaction = null;
            _connection = null;
            _handle = null;
        }
    }
}
