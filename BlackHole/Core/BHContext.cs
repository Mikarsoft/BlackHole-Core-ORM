
using BlackHole.Identifiers;

namespace BlackHole.Core
{
    public static class BHContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <returns></returns>
        public static bool Provider<T>(this T entity) where T : IBHEntityIdentifier
        {
            return true;
        }
    }
}
