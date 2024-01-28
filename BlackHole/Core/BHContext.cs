using BlackHole.Entities;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    public static class BHContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <returns></returns>
        public static IBHDataProvider<T,G> For<T, G>() where T :BlackHoleEntity<G> where G: IComparable<G>
        {
            return new BHDataProvider<T, G>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IBHOpenDataProvider<T> For<T>() where T : BHOpenEntity<T>
        {
            return new BHOpenDataProvider<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IBHConnection Connection()
        {
            return new BHConnection();
        }
    }
}
