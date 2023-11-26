using System.Runtime.InteropServices;

namespace BlackHole.RayCore
{
    internal class RayHandle : SafeHandle
    {
        public RayHandle(Ray64.SQL_HANDLE sqlHandle, RayHandle ownsHandle) : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid => throw new NotImplementedException();

        protected override bool ReleaseHandle()
        {
            throw new NotImplementedException();
        }
    }
}
