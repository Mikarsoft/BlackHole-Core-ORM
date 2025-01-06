using Mikarsoft.BlackHoleCore.Tools;

namespace Mikarsoft.BlackHoleCore
{
    internal static class BHMessanger
    {
        internal static bool Execute(this BHSelectStatementBuilder statement)
        {
            return true;
        }

        internal static async Task<bool> ExecuteAsync(this BHSelectStatementBuilder statement)
        {
            return false;
        }

        internal static T? FirstOrDefault<T>(this BHSelectStatementBuilder statement) where T : class
        {
            return default(T?);
        }

        internal static async Task<T?> FirstOrDefaultAsync<T>(this BHSelectStatementBuilder statement) where T : class
        {
            return default(T?);
        }

        internal static List<T> ToList<T>(this BHSelectStatementBuilder statement) where T : class
        {
            return new List<T>();
        }

        internal static async Task<List<T>> ToListAsync<T>(this BHSelectStatementBuilder statement) where T: class
        {
            return new List<T>();
        }
    }
}
