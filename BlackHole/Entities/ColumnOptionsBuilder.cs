
using System.Reflection;

namespace BlackHole.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class ColumnOptionsBuilder<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="tKey"></typeparam>
        /// <param name="primaryKey"></param>
        public void SetPrimaryKey<tKey>(Func<T,tKey> primaryKey)
        {

        }
    }
}
