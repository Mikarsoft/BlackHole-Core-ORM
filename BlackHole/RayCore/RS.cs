
namespace BlackHole.RayCore
{
    internal static class RS
    {
        internal static Exception ReaderClosed(string propName)
        {
            return new Exception($"Error at retrieving {propName}: RayDataReader is closed.");
        }

        static internal void TraceRay(int level, string method, Ray64.RetCode retcode)
        {
            RBid.TraceSqlReturn("<Ray|API|Ray|RET> %08X{SQLRETURN}, method=%ls\n", retcode, method);
        }
    }
}
