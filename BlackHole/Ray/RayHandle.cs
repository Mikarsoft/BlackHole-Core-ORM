using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using BlackHole.RayCore;

namespace BlackHole.Ray
{
    internal abstract class RayHandle : SafeHandle
    {

        private Ray32.SQL_HANDLE _handleType;
        private RayHandle _parentHandle;

        protected RayHandle(Ray32.SQL_HANDLE handleType, RayHandle parentHandle) : base(IntPtr.Zero, true)
        {

            _handleType = handleType;

            bool mustRelease = false;
            Ray32.RetCode retcode = Ray32.RetCode.SUCCESS;

            // using ConstrainedRegions to make the native Ray call and AddRef the parent
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                // validate handleType
                switch (handleType)
                {
                    case Ray32.SQL_HANDLE.ENV:
                        Debug.Assert(null == parentHandle, "did not expect a parent handle");
                        retcode = UnsafeNativeMethods.SQLAllocHandle(handleType, IntPtr.Zero, out base.handle);
                        break;
                    case Ray32.SQL_HANDLE.DBC:
                    case Ray32.SQL_HANDLE.STMT:
                        // must addref before calling native so it won't be released just after
                        Debug.Assert(null != parentHandle, "expected a parent handle"); // safehandle can't be null
                        parentHandle.DangerousAddRef(ref mustRelease);

                        retcode = UnsafeNativeMethods.SQLAllocHandle(handleType, parentHandle, out base.handle);
                        break;
                    //              case Ray32.SQL_HANDLE.DESC:
                    default:
                        Debug.Assert(false, "unexpected handleType");
                        break;
                }
            }
            finally
            {
                if (mustRelease)
                {
                    switch (handleType)
                    {
                        case Ray32.SQL_HANDLE.DBC:
                        case Ray32.SQL_HANDLE.STMT:
                            if (IntPtr.Zero != base.handle)
                            {
                                // must assign _parentHandle after a handle is actually created
                                // since ReleaseHandle will only call DangerousRelease if a handle exists
                                _parentHandle = parentHandle;
                            }
                            else
                            {
                                // without a handle, ReleaseHandle may not be called
                                parentHandle.DangerousRelease();
                            }
                            break;
                    }
                }
            }
            Bid.TraceSqlReturn("<Ray.SQLAllocHandle|API|Ray|RET> %08X{SQLRETURN}\n", retcode);

            if ((IntPtr.Zero == base.handle) || (Ray32.RetCode.SUCCESS != retcode))
            {
                // 
                throw Ray.CantAllocateEnvironmentHandle(retcode);
            }
        }

        internal RayHandle(RayStatementHandle parentHandle, Ray32.SQL_ATTR attribute) : base(IntPtr.Zero, true)
        {
            Debug.Assert((Ray32.SQL_ATTR.APP_PARAM_DESC == attribute) || (Ray32.SQL_ATTR.APP_ROW_DESC == attribute), "invalid attribute");
            _handleType = Ray32.SQL_HANDLE.DESC;

            int cbActual;
            Ray32.RetCode retcode;
            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                // must addref before calling native so it won't be released just after
                parentHandle.DangerousAddRef(ref mustRelease);

                retcode = parentHandle.GetStatementAttribute(attribute, out base.handle, out cbActual);
            }
            finally
            {
                if (mustRelease)
                {
                    if (IntPtr.Zero != base.handle)
                    {
                        // must call DangerousAddRef after a handle is actually created
                        // since ReleaseHandle will only call DangerousRelease if a handle exists
                        _parentHandle = parentHandle;
                    }
                    else
                    {
                        // without a handle, ReleaseHandle may not be called
                        parentHandle.DangerousRelease();
                    }
                }
            }
            if (IntPtr.Zero == base.handle)
            {
                throw Ray.FailedToGetDescriptorHandle(retcode);
            }
            // no info-message handle on getting a descriptor handle
        }

        internal Ray32.SQL_HANDLE HandleType
        {
            get
            {
                return _handleType;
            }
        }

        public override bool IsInvalid
        {
            get
            {
                // we should not have a parent if we do not have a handle
                return (IntPtr.Zero == base.handle);
            }
        }

        override protected bool ReleaseHandle()
        {
            // NOTE: The SafeHandle class guarantees this will be called exactly once and is non-interrutible.
            IntPtr handle = base.handle;
            base.handle = IntPtr.Zero;

            if (IntPtr.Zero != handle)
            {
                Ray32.SQL_HANDLE handleType = HandleType;

                switch (handleType)
                {
                    case Ray32.SQL_HANDLE.DBC:
                    // Disconnect happens in RayConnectionHandle.ReleaseHandle
                    case Ray32.SQL_HANDLE.ENV:
                    case Ray32.SQL_HANDLE.STMT:
                        Ray32.RetCode retcode = UnsafeNativeMethods.SQLFreeHandle(handleType, handle);
                        Bid.TraceSqlReturn("<Ray.SQLFreeHandle|API|Ray|RET> %08X{SQLRETURN}\n", retcode);
                        break;

                    case Ray32.SQL_HANDLE.DESC:
                        // nothing to free on the handle
                        break;

                    // case 0: ThreadAbortException setting handle before HandleType
                    default:
                        //Debug.Assert(ADP.PtrZero == handle, "unknown handle type");
                        break;
                }
            }

            // If we ended up getting released, then we have to release
            // our reference on our parent.
            RayHandle parentHandle = _parentHandle;
            _parentHandle = null;
            if (null != parentHandle)
            {
                parentHandle.DangerousRelease();
                parentHandle = null;
            }
            return true;
        }

        internal Ray32.RetCode GetDiagnosticField(out string sqlState)
        {
            short cbActual;
            // Ray (MSDN) documents it expects a buffer large enough to hold 5(+L'\0') unicode characters
            StringBuilder sb = new StringBuilder(6);
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLGetDiagFieldW(
                HandleType,
                this,
                (short)1,
                Ray32.SQL_DIAG_SQLSTATE,
                sb,
                checked((short)(2 * sb.Capacity)), // expects number of bytes, see \\kbinternal\kb\articles\294\1\69.HTM
                out cbActual);
            Ray.TraceRay(3, "SQLGetDiagFieldW", retcode);
            if ((retcode == Ray32.RetCode.SUCCESS) || (retcode == Ray32.RetCode.SUCCESS_WITH_INFO))
            {
                sqlState = sb.ToString();
            }
            else
            {
                sqlState = string.Empty; //ADP.StrEmpty;
            }
            return retcode;
        }

        internal Ray32.RetCode GetDiagnosticRecord(short record, out string sqlState, StringBuilder message, out int nativeError, out short cchActual)
        {
            // Ray (MSDN) documents it expects a buffer large enough to hold 4(+L'\0') unicode characters
            StringBuilder sb = new StringBuilder(5);
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLGetDiagRecW(HandleType, this, record, sb, out nativeError, message, checked((short)message.Capacity), out cchActual);
            Ray.TraceRay(3, "SQLGetDiagRecW", retcode);

            if ((retcode == Ray32.RetCode.SUCCESS) || (retcode == Ray32.RetCode.SUCCESS_WITH_INFO))
            {
                sqlState = sb.ToString();
            }
            else
            {
                sqlState = string.Empty;// ADP.StrEmpty;
            }
            return retcode;
        }
    }

    sealed internal class RayDescriptorHandle : RayHandle
    {

        internal RayDescriptorHandle(RayStatementHandle statementHandle, Ray32.SQL_ATTR attribute) : base(statementHandle, attribute)
        {
        }

        internal Ray32.RetCode GetDescriptionField(int i, Ray32.SQL_DESC attribute, CNativeBuffer buffer, out int numericAttribute)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLGetDescFieldW(this, checked((short)i), attribute, buffer, buffer.ShortLength, out numericAttribute);
            Ray.TraceRay(3, "SQLGetDescFieldW", retcode);
            return retcode;
        }

        internal Ray32.RetCode SetDescriptionField1(short ordinal, Ray32.SQL_DESC type, IntPtr value)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLSetDescFieldW(this, ordinal, type, value, 0);
            Ray.TraceRay(3, "SQLSetDescFieldW", retcode);
            return retcode;
        }

        internal Ray32.RetCode SetDescriptionField2(short ordinal, Ray32.SQL_DESC type, HandleRef value)
        {
            Ray32.RetCode retcode = UnsafeNativeMethods.SQLSetDescFieldW(this, ordinal, type, value, 0);
            Ray.TraceRay(3, "SQLSetDescFieldW", retcode);
            return retcode;
        }
    }
}
