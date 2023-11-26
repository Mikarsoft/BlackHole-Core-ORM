using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;

namespace BlackHole.RayCore
{
    [SuppressUnmanagedCodeSecurity()]
    internal static class SafeNativeMethods
    {

        static internal unsafe IntPtr InterlockedExchangePointer(
                IntPtr lpAddress,
                IntPtr lpValue)
        {
            IntPtr previousPtr;
            IntPtr actualPtr = *(IntPtr*)lpAddress.ToPointer();

            do
            {
                previousPtr = actualPtr;
                actualPtr = Interlocked.CompareExchange(ref *(IntPtr*)lpAddress.ToPointer(), lpValue, previousPtr);
            }
            while (actualPtr != previousPtr);

            return actualPtr;
        }

        [DllImport(ExternDll.Kernel32, SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern IntPtr LocalAlloc(int flags, IntPtr countOfBytes);

        [DllImport(ExternDll.Kernel32, SetLastError = true)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern IntPtr LocalFree(IntPtr handle);

        [DllImport(ExternDll.Kernel32, PreserveSig = true)]
        [ResourceExposure(ResourceScope.None)]
        static internal extern void ZeroMemory(IntPtr dest, IntPtr length);

        [DllImport(ExternDll.Oleaut32, CharSet = CharSet.Unicode, PreserveSig = false)]
        [ResourceExposure(ResourceScope.Process)]
        static private extern void SetErrorInfo(int dwReserved, IntPtr pIErrorInfo);

        sealed internal class Wrapper
        {

            private Wrapper() { }

            [ResourceExposure(ResourceScope.None)]
            [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)]
            static internal void ClearErrorInfo()
            {
                SetErrorInfo(0, IntPtr.Zero);
            }
        }


    }
}
