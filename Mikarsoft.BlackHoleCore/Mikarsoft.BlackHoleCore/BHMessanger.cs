using Mikarsoft.BlackHoleCore.Tools;

namespace Mikarsoft.BlackHoleCore
{
    internal static class BHMessanger
    {
        internal static bool Execute(this BHStatementBuilder statement)
        {
            return true;
        }

        internal static async Task<bool> ExecuteAsync(this BHStatementBuilder statement)
        {
            return false;
        }

        internal static T? FirstOrDefault<T>(this BHStatementBuilder statement) where T : class
        {
            return default(T?);
        }

        internal static async Task<T?> FirstOrDefaultAsync<T>(this BHStatementBuilder statement) where T : class
        {
            return default(T?);
        }

        internal static List<T> ToList<T>(this BHStatementBuilder statement) where T : class
        {
            return new List<T>();
        }

        internal static async Task<List<T>> ToListAsync<T>(this BHStatementBuilder statement) where T: class
        {
            return new List<T>();
        }
    }
}
