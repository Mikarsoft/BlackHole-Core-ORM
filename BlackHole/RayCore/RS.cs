
using BlackHole.Ray;
using System.Diagnostics;

namespace BlackHole.RayCore
{
    internal static class RS
    {
        internal static Exception ReaderClosed(string propName)
        {
            return new Exception($"Error at retrieving {propName}: RayDataReader is closed.");
        }

        static internal void TraceRay(int level, string method, Ray64.RetCode retcode)
        {
            RBid.TraceSqlReturn("<Ray|API|Ray|RET> %08X{SQLRETURN}, method=%ls\n", retcode, method);
        }

        static internal IntPtr IntPtrOffset(IntPtr pbase, Int32 offset)
        {
            if (4 == IntPtr.Size)
            {
                return (IntPtr)checked(pbase.ToInt32() + offset);
            }
            Debug.Assert(8 == IntPtr.Size, "8 != IntPtr.Size"); // MDAC 73747
            return (IntPtr)checked(pbase.ToInt64() + offset);
        }

        static internal Exception NumericToDecimalOverflow()
        {
            return InvalidCast("Numeric decimal overflow");
        }

        static internal InvalidCastException InvalidCast(string error)
        {
            return InvalidCast(error, null);
        }

        static internal InvalidCastException InvalidCast(string error, Exception? inner)
        {
            InvalidCastException e = new InvalidCastException(error, inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        internal enum InternalErrorCode
        {
            UnpooledObjectHasOwner = 0,
            UnpooledObjectHasWrongOwner = 1,
            PushingObjectSecondTime = 2,
            PooledObjectHasOwner = 3,
            PooledObjectInPoolMoreThanOnce = 4,
            CreateObjectReturnedNull = 5,
            NewObjectCannotBePooled = 6,
            NonPooledObjectUsedMoreThanOnce = 7,
            AttemptingToPoolOnRestrictedToken = 8,
            ConnectionOptionsInUse = 9,
            ConvertSidToStringSidWReturnedNull = 10,
            UnexpectedTransactedObject = 11,
            AttemptingToConstructReferenceCollectionOnStaticObject = 12,
            AttemptingToEnlistTwice = 13,
            CreateReferenceCollectionReturnedNull = 14,
            PooledObjectWithoutPool = 15,
            UnexpectedWaitAnyResult = 16,
            SynchronousConnectReturnedPending = 17,
            CompletedConnectReturnedPending = 18,

            NameValuePairNext = 20,
            InvalidParserState1 = 21,
            InvalidParserState2 = 22,
            InvalidParserState3 = 23,

            InvalidBuffer = 30,

            UnimplementedSMIMethod = 40,
            InvalidSmiCall = 41,

            SqlDependencyObtainProcessDispatcherFailureObjectHandle = 50,
            SqlDependencyProcessDispatcherFailureCreateInstance = 51,
            SqlDependencyProcessDispatcherFailureAppDomain = 52,
            SqlDependencyCommandHashIsNotAssociatedWithNotification = 53,

            UnknownTransactionFailure = 60,
        }

        static internal Exception InternalError(InternalErrorCode internalError)
        {
            return InvalidOperation($"Internal Providers Error: {(int)internalError}");
        }

        static internal Exception InternalError(InternalErrorCode internalError, Exception innerException)
        {
            return InvalidOperation($"Internal Providers Error: {(int)internalError}", innerException);
        }

        static internal InvalidOperationException InvalidOperation(string error)
        {
            InvalidOperationException e = new InvalidOperationException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        static internal TimeoutException TimeoutException(string error)
        {
            TimeoutException e = new TimeoutException(error);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        static internal InvalidOperationException InvalidOperation(string error, Exception inner)
        {
            InvalidOperationException e = new InvalidOperationException(error, inner);
            TraceExceptionAsReturnValue(e);
            return e;
        }

        static internal void TraceExceptionAsReturnValue(Exception e)
        {
            TraceException("<comm.ADP.TraceException|ERR|THROW> '%ls'\n", e);
        }

        [BidMethod] // this method accepts BID format as an argument, this attribute allows FXCopBid rule to validate calls to it
        static private void TraceException(
        string trace,
        [BidArgumentType(typeof(String))] Exception e)
        {
            Debug.Assert(null != e, "TraceException: null Exception");
            if (null != e)
            {
                Bid.Trace(trace, e.ToString()); // will include callstack if permission is available
            }
        }
    }
}
