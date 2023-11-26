

namespace BlackHole.RayCore
{
    internal static class ExternDll
    {

#if FEATURE_PAL && !SILVERLIGHT

#if !PLATFORM_UNIX
        internal const String DLLPREFIX = "";
        internal const String DLLSUFFIX = ".dll";
#else // !PLATFORM_UNIX
#if __APPLE__
        internal const String DLLPREFIX = "lib";
        internal const String DLLSUFFIX = ".dylib";
#elif _AIX
        internal const String DLLPREFIX = "lib";
        internal const String DLLSUFFIX = ".a";
#elif __hppa__ || IA64
        internal const String DLLPREFIX = "lib";
        internal const String DLLSUFFIX = ".sl";
#else
        internal const String DLLPREFIX = "lib";
        internal const String DLLSUFFIX = ".so";
#endif
#endif // !PLATFORM_UNIX

        public const string Kernel32 = DLLPREFIX + "rotor_pal" + DLLSUFFIX;
        public const string User32 = DLLPREFIX + "rotor_pal" + DLLSUFFIX;
        public const string Mscoree  = DLLPREFIX + "sscoree" + DLLSUFFIX;

#elif FEATURE_PAL && SILVERLIGHT

        public const string Kernel32 = "coreclr";
        public const string User32 = "coreclr";


#else
        public const string Kernel32 = "kernel32.dll";
        public const string User32 = "user32.dll";
        public const string Mscoree = "mscoree.dll";
#endif //!FEATURE_PAL
        public const string Oleaut32 = "oleaut32.dll";

        // system.data specific
        internal const string Ray64 = "odbc32.dll";
        internal const string SNI = "System.Data.dll";

        // system.data.oracleclient specific
        internal const string OciDll = "oci.dll";
        internal const string OraMtsDll = "oramts.dll";
    }
}
