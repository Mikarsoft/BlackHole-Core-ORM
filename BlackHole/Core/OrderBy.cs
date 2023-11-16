using BlackHole.Identifiers;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    public static class OrderBy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static BlackHoleOrderBy<T> Ascending<T>(Func<T, object?> action) where T : IBHEntityIdentifier
        {
            return new();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static BlackHoleOrderBy<T> Descending<T>(Func<T, object?> action) where T : IBHEntityIdentifier
        {
            return new();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static BlackHoleOrderBy<T> ThenByAscending<T>(this BlackHoleOrderBy<T> orderBy, Func<T, object?> action) where T : IBHEntityIdentifier
        {
            return new();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static BlackHoleOrderBy<T> ThenByDescending<T>(this BlackHoleOrderBy<T> orderBy, Func<T, object?> action) where T : IBHEntityIdentifier
        {
            return new();
        }
    }
}
