

namespace BlackHole.RayCore
{
    internal class RayEnvironmentHandle : RayHandle
    {
        public RayEnvironmentHandle(Ray64.SQL_HANDLE sqlHandle, RayHandle ownsHandle) : base(sqlHandle, ownsHandle)
        {
        }
    }
}
