using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Ray
{
    internal class RayExtensionMethods
    {
        static internal Exception UninitializedParameterSize(int index, Type dataType)
        {
            return InvalidOperation(Res.GetString(Res.ADP_UninitializedParameterSize, index.ToString(CultureInfo.InvariantCulture), dataType.Name));
        }
        static internal InvalidOperationException InvalidOperation(string error)
        {
            InvalidOperationException e = new InvalidOperationException(error);
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

        [System.Security.SecurityCritical]  // auto-generated_required
        public static IntPtr ReadIntPtr(this int ptr)
        {
#if WIN32
                return (IntPtr) ReadInt32(ptr, 0);
#else
            return (IntPtr.)ReadInt64(ptr, 0);
#endif
        }

        [System.Security.SecurityCritical]  // auto-generated_required
        [ResourceExposure(ResourceScope.None)]
        public static unsafe long ReadInt64(IntPtr ptr, int ofs)
        {
            try
            {
                byte* addr = (byte*)ptr + ofs;
                if ((unchecked((int)addr) & 0x7) == 0)
                {
                    // aligned read
                    return *((long*)addr);
                }
                else
                {
                    // unaligned read
                    long val;
                    byte* valPtr = (byte*)&val;
                    valPtr[0] = addr[0];
                    valPtr[1] = addr[1];
                    valPtr[2] = addr[2];
                    valPtr[3] = addr[3];
                    valPtr[4] = addr[4];
                    valPtr[5] = addr[5];
                    valPtr[6] = addr[6];
                    valPtr[7] = addr[7];
                    return val;
                }
            }
            catch (NullReferenceException)
            {
                // this method is documented to throw AccessViolationException on any AV
                throw new AccessViolationException();
            }
        }
    }
}
