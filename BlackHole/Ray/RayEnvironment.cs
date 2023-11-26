using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Ray
{
    sealed internal class RayEnvironment
    {
        static private object _globalEnvironmentHandle;
        static private object _globalEnvironmentHandleLock = new object();

        private RayEnvironment() { }  // default const.

        static internal RayEnvironmentHandle GetGlobalEnvironmentHandle()
        {
            RayEnvironmentHandle globalEnvironmentHandle = _globalEnvironmentHandle as RayEnvironmentHandle;
            if (null == globalEnvironmentHandle)
            {
                //ADP.CheckVersionMDAC(true);

                lock (_globalEnvironmentHandleLock)
                {
                    globalEnvironmentHandle = _globalEnvironmentHandle as RayEnvironmentHandle;
                    if (null == globalEnvironmentHandle)
                    {
                        globalEnvironmentHandle = new RayEnvironmentHandle();
                        _globalEnvironmentHandle = globalEnvironmentHandle;
                    }
                }
            }
            return globalEnvironmentHandle;
        }

        static internal void ReleaseObjectPool()
        {
            object globalEnvironmentHandle = Interlocked.Exchange(ref _globalEnvironmentHandle, null);
            if (null != globalEnvironmentHandle)
            {
                (globalEnvironmentHandle as RayEnvironmentHandle).Dispose(); // internally refcounted so will happen correctly
            }
        }
    }
}
