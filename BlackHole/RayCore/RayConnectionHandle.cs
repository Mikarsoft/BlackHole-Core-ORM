
namespace BlackHole.RayCore
{
    internal class RayConnectionHandle : RayHandle
    {
        internal RayConnectionHandle(RayConnection connection, RayConnectionString constr, RayEnvironmentHandle environmentHandle)
            : base(Ray64.SQL_HANDLE.DBC, environmentHandle)
        {

        }
    }
}
