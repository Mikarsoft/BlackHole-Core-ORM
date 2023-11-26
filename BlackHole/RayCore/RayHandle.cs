using System.Runtime.InteropServices;
using System.Text;

namespace BlackHole.RayCore
{
    internal class RayHandle : SafeHandle
    {
        private readonly Ray64.SQL_HANDLE _handle;
        private readonly Ray64.SQL_ATTR _attr;
        public RayHandle(Ray64.SQL_HANDLE sqlHandle, RayHandle ownsHandle) : base(IntPtr.Zero, true)
        {
            _handle = sqlHandle;
        }

        public RayHandle(Ray64.SQL_ATTR sqlHandle, RayStatementHandle ownsHandle) : base(IntPtr.Zero, true)
        {
            _attr = sqlHandle;
        }

        public override bool IsInvalid => throw new NotImplementedException();

        protected override bool ReleaseHandle()
        {
            throw new NotImplementedException();
        }

        internal Ray64.RetCode GetDiagnosticRecord(short record, out string sqlState, StringBuilder message, out int nativeError, out short cchActual)
        {
            // Ray (MSDN) documents it expects a buffer large enough to hold 4(+L'\0') unicode characters
            StringBuilder sb = new StringBuilder(5);
            Ray64.RetCode retcode = UnsafeNativeMethods.SQLGetDiagRecW(_handle, this, record, sb, out nativeError, message, checked((short)message.Capacity), out cchActual);
            RS.TraceRay(3, "SQLGetDiagRecW", retcode);

            if ((retcode == Ray64.RetCode.SUCCESS) || (retcode == Ray64.RetCode.SUCCESS_WITH_INFO))
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
}
