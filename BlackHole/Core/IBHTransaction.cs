
namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBHTransaction : IDisposable
    {

        /// <summary>
        /// Commit the transaction.
        /// </summary>
        /// <returns></returns>
        public bool Commit();

        /// <summary>
        /// Block the transaction.
        /// </summary>
        /// <returns></returns>
        public bool DoNotCommit();

        /// <summary>
        /// RollBack the transaction
        /// </summary>
        /// <returns></returns>
        public bool RollBack();
    }
}
