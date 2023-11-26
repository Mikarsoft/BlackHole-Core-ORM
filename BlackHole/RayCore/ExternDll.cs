

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

#else
        // system.data specific
        internal const string Ray64 = "odbc32.dll";
        internal const string SNI = "System.Data.dll";
#endif //!FEATURE_PAL
    }
}
