using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Ray
{
    sealed internal class RayConnectionHandle : RayHandle
    {
        private HandleState _handleState;

        private enum HandleState
        {
            Allocated = 0,
            Connected = 1,
            Transacted = 2,
            TransactionInProgress = 3,
        }

        internal RayConnectionHandle(RayConnection connection, RayConnectionString constr, RayEnvironmentHandle environmentHandle) : base(Ray32.SQL_HANDLE.DBC, environmentHandle)
        {
            if (null == connection)
            {
                throw ADP.ArgumentNull("connection");
            }
            if (null == constr)
            {
                throw ADP.ArgumentNull("constr");
            }

            Ray32.RetCode retcode;

            //Set connection timeout (only before open).
            //Note: We use login timeout since its Ray 1.0 option, instead of using
            //connectiontimeout (which affects other things besides just login) and its
            //a Ray 3.0 feature.  The ConnectionTimeout on the managed providers represents
            //the login timeout, nothing more.
            int connectionTimeout = connection.ConnectionTimeout;
            retcode = SetConnectionAttribute2(Ray32.SQL_ATTR.LOGIN_TIMEOUT, (IntPtr)connectionTimeout, (Int32)Ray32.SQL_IS.UINTEGER);

            string connectionString = constr.UsersConnectionString(false);

            // Connect to the driver.  (Using the connection string supplied)
            //Note: The driver doesn't filter out the password in the returned connection string
            //so their is no need for us to obtain the returned connection string
            // Prepare to handle a ThreadAbort Exception between SQLDriverConnectW and update of the state variables
            retcode = Connect(connectionString);
            connection.HandleError(this, retcode);
        }

        private Ray32.RetCode AutoCommitOff()
        {
            Ray32.RetCode retcode;

            Debug.Assert(HandleState.Connected <= _handleState, "AutoCommitOff while in wrong state?");

            // Avoid runtime injected errors in the following block.
            // must call SQLSetConnectAttrW and set _handleState
            RuntimeHelpers.PrepareConstrainedRegions();
            try { }
            finally
            {
                retcode = UnsafeNativeMethods.SQLSetConnectAttrW(this, Ray32.SQL_ATTR.AUTOCOMMIT, Ray32.SQL_AUTOCOMMIT_OFF, (Int32)Ray32.SQL_IS.UINTEGER);
                switch (retcode)
                {
                    case Ray32.RetCode.SUCCESS:
                    case Ray32.RetCode.SUCCESS_WITH_INFO:
                        _handleState = HandleState.Transacted;
                        break;
                }
            }
            Ray.TraceRay(3, "SQLSetConnectAttrW", retcode);
            return retcode;
        }

        internal Ray32.RetCode BeginTransaction(ref IsolationLevel isolevel)
        {
            Ray32.RetCode retcode = Ray32.RetCode.SUCCESS;
            Ray32.SQL_ATTR isolationAttribute;
            if (IsolationLevel.Unspecified != isolevel)
            {
                Ray32.SQL_TRANSACTION sql_iso;
                switch (isolevel)
                {
                    case IsolationLevel.ReadUncommitted:
                        sql_iso = Ray32.SQL_TRANSACTION.READ_UNCOMMITTED;
                        isolationAttribute = Ray32.SQL_ATTR.TXN_ISOLATION;
                        break;
                    case IsolationLevel.ReadCommitted:
                        sql_iso = Ray32.SQL_TRANSACTION.READ_COMMITTED;
                        isolationAttribute = Ray32.SQL_ATTR.TXN_ISOLATION;
                        break;
                    case IsolationLevel.RepeatableRead:
                        sql_iso = Ray32.SQL_TRANSACTION.REPEATABLE_READ;
                        isolationAttribute = Ray32.SQL_ATTR.TXN_ISOLATION;
                        break;
                    case IsolationLevel.Serializable:
                        sql_iso = Ray32.SQL_TRANSACTION.SERIALIZABLE;
                        isolationAttribute = Ray32.SQL_ATTR.TXN_ISOLATION;
                        break;
                    case IsolationLevel.Snapshot:
                        sql_iso = Ray32.SQL_TRANSACTION.SNAPSHOT;
                        // VSDD 414121: Snapshot isolation level must be set through SQL_COPT_SS_TXN_ISOLATION (http://msdn.microsoft.com/en-us/library/ms131709.aspx)
                        isolationAttribute = Ray32.SQL_ATTR.SQL_COPT_SS_TXN_ISOLATION;
                        break;
                    case IsolationLevel.Chaos:
                        throw Ray.NotSupportedIsolationLevel(isolevel);
                    default:
                        throw ADP.InvalidIsolationLevel(isolevel);
                }

                //Set the isolation level (unless its unspecified)
                retcode = SetConnectionAttribute2(isolationAttribute, (IntPtr)sql_iso, (Int32)Ray32.SQL_IS.INTEGER);

                //Note: The Driver can return success_with_info to indicate it "rolled" the
                //isolevel to the next higher value.  If this is the case, we need to requery
                //the value if th euser asks for it...
                //We also still propagate the info, since it could be other info as well...

                if (Ray32.RetCode.SUCCESS_WITH_INFO == retcode)
                {
                    isolevel = IsolationLevel.Unspecified;
                }
            }

            switch (retcode)
            {
                case Ray32.RetCode.SUCCESS:
                case Ray32.RetCode.SUCCESS_WITH_INFO:
                    //Turn off auto-commit (which basically starts the transaction)
                    retcode = AutoCommitOff();
                    _handleState = HandleState.TransactionInProgress;
                    break;
            }
            return retcode;
        }

        internal Ray32.RetCode CompleteTransaction(short transactionOperation)
        {
            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);
                Ray32.RetCode retcode = CompleteTransaction(transactionOperation, base.handle);
                return retcode;
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        private Ray32.RetCode CompleteTransaction(short transactionOperation, IntPtr handle)
        {
            // must only call this code from ReleaseHandle or DangerousAddRef region

            Ray32.RetCode retcode = Ray32.RetCode.SUCCESS;

            // using ConstrainedRegions to make the native Ray call and change the _handleState
            RuntimeHelpers.PrepareConstrainedRegions();
            try { }
            finally
            {
                if (HandleState.TransactionInProgress == _handleState)
                {
                    retcode = UnsafeNativeMethods.SQLEndTran(HandleType, handle, transactionOperation);
                    if ((Ray32.RetCode.SUCCESS == retcode) || (Ray32.RetCode.SUCCESS_WITH_INFO == retcode))
                    {
                        _handleState = HandleState.Transacted;
                    }
                    Bid.TraceSqlReturn("<Ray.SQLEndTran|API|Ray|RET> %08X{SQLRETURN}\n", retcode);
                }

                if (HandleState.Transacted == _handleState)
                { // AutoCommitOn
                    retcode = UnsafeNativeMethods.SQLSetConnectAttrW(handle, Ray32.SQL_ATTR.AUTOCOMMIT, Ray32.SQL_AUTOCOMMIT_ON, (Int32)Ray32.SQL_IS.UINTEGER);
                    _handleState = HandleState.Connected;
                    Bid.TraceSqlReturn("<Ray.SQLSetConnectAttr|API|Ray|RET> %08X{SQLRETURN}\n", retcode);
                }
            }
            //Overactive assert which fires if handle was allocated - but failed to connect to the server
            //it can more legitmately fire if transaction failed to rollback - but there isn't much we can do in that situation
            //Debug.Assert((HandleState.Connected == _handleState) || (HandleState.TransactionInProgress == _handleState), "not expected HandleState.Connected");
            return retcode;
        }
        private Ray32.RetCode Connect(string connectionString)
        {
            Debug.Assert(HandleState.Allocated == _handleState, "SQLDriverConnect while in wrong state?");
            Ray32.RetCode retcode;

            // Avoid runtime injected errors in the following block.
            RuntimeHelpers.PrepareConstrainedRegions();
            try { }
            finally
            {

                short cbActualSize;
                retcode = UnsafeNativeMethods.SQLDriverConnectW(this, ADP.PtrZero, connectionString, Ray32.SQL_NTS, ADP.PtrZero, 0, out cbActualSize, (short)Ray32.SQL_DRIVER.NOPROMPT);
                switch (retcode)
                {
                    case Ray32.RetCode.SUCCESS:
                    case Ray32.RetCode.SUCCESS_WITH_INFO:
                        _handleState = HandleState.Connected;
                        break;
                }
            }
            Ray.TraceRay(3, "SQLDriverConnectW", retcode);
            return retcode;
        }

        override protected bool ReleaseHandle()
        {
            // NOTE: The SafeHandle class guarantees this will be called exactly once and is non-interrutible.
            Ray32.RetCode retcode;

            // must call complete the transaction rollback, change handle state, and disconnect the connection
            retcode = CompleteTransaction(Ray32.SQL_ROLLBACK, handle);

            if ((HandleState.Connected == _handleState) || (HandleState.TransactionInProgress == _handleState))
            {
                retcode = UnsafeNativeMethods.SQLDisconnect(handle);
                _handleState = HandleState.Allocated;
                Bid.TraceSqlReturn("<Ray.SQLDisconnect|API|Ray|RET> %08X{SQLRETURN}\n", retcode);
            }
            Debug.Assert(HandleState.Allocated == _handleState, "not expected HandleState.Allocated");
            return base.ReleaseHandle();
        }

        internal Ray32.RetCode GetConnectionAttribute(Ray32.SQL_ATTR attribute, byte[] buffer, out int cbActual)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLGetConnectAttrW(this, attribute, buffer, buffer.Length, out cbActual);
            Bid.Trace("<Ray.SQLGetConnectAttr|Ray> SQLRETURN=%d, Attribute=%d, BufferLength=%d, StringLength=%d\n", (int)retcode, (int)attribute, buffer.Length, (int)cbActual);
            return retcode;
        }

        internal Ray32.RetCode GetFunctions(Ray32.SQL_API fFunction, out Int16 fExists)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLGetFunctions(this, fFunction, out fExists);
            Ray.TraceRay(3, "SQLGetFunctions", retcode);
            return retcode;
        }

        internal Ray32.RetCode GetInfo2(Ray32.SQL_INFO info, byte[] buffer, out short cbActual)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLGetInfoW(this, info, buffer, checked((short)buffer.Length), out cbActual);
            Bid.Trace("<Ray.SQLGetInfo|Ray> SQLRETURN=%d, InfoType=%d, BufferLength=%d, StringLength=%d\n", (int)retcode, (int)info, buffer.Length, (int)cbActual);
            return retcode;
        }

        internal Ray32.RetCode GetInfo1(Ray32.SQL_INFO info, byte[] buffer)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLGetInfoW(this, info, buffer, checked((short)buffer.Length), ADP.PtrZero);
            Bid.Trace("<Ray.SQLGetInfo|Ray> SQLRETURN=%d, InfoType=%d, BufferLength=%d\n", (int)retcode, (int)info, buffer.Length);
            return retcode;
        }

        internal Ray32.RetCode SetConnectionAttribute2(Ray32.SQL_ATTR attribute, IntPtr value, Int32 length)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLSetConnectAttrW(this, attribute, value, length);
            Ray.TraceRay(3, "SQLSetConnectAttrW", retcode);
            return retcode;
        }

        internal Ray32.RetCode SetConnectionAttribute3(Ray32.SQL_ATTR attribute, string buffer, Int32 length)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLSetConnectAttrW(this, attribute, buffer, length);
            Bid.Trace("<Ray.SQLSetConnectAttr|Ray> SQLRETURN=%d, Attribute=%d, BufferLength=%d\n", (int)retcode, (int)attribute, buffer.Length);
            return retcode;
        }

        internal Ray32.RetCode SetConnectionAttribute4(Ray32.SQL_ATTR attribute, System.Transactions.IDtcTransaction transaction, Int32 length)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLSetConnectAttrW(this, attribute, transaction, length);
            Ray.TraceRay(3, "SQLSetConnectAttrW", retcode);
            return retcode;
        }
    }
}
