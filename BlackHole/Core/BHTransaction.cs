using BlackHole.Engine;

namespace BlackHole.Core
{
    /// <summary>
    /// A Transaction Object that automatically creates a connection and a transaction
    /// and it can be used in every BlackHole Method.
    /// <para>Tip: Don't forget to dispose this Object after using it. If you don't perform any 
    /// action on this class, the Commit Method gets triggered on Dispose</para>
    /// </summary>
    internal class BHTransaction : IBHTransaction
    {
        internal BlackHoleTransaction transaction;

        /// <summary>
        /// A Transaction Object that automatically creates a connection and a transaction
        /// and it can be used in every BlackHole Method.
        /// <para>Tip: Don't forget to dispose this Object after using it. If you don't perform any 
        /// action on this class, the Commit Method gets triggered on Dispose</para>
        /// </summary>
        internal BHTransaction()
        {
            transaction = new BlackHoleTransaction(BlackHoleEngine.GetAvailableConnections());
        }

        /// <summary>
        /// Commit the transaction.
        /// </summary>
        /// <returns></returns>
        public bool Commit()
        {
            return transaction.Commit();
        }

        /// <summary>
        /// Block the transaction.
        /// </summary>
        /// <returns></returns>
        public bool DoNotCommit()
        {
            return transaction.DoNotCommit();
        }

        /// <summary>
        /// RollBack the transaction
        /// </summary>
        /// <returns></returns>
        public bool RollBack()
        {
            return transaction.RollBack();
        }

        /// <summary>
        /// Disposes the Connection and the transaction. If no other action have been used,
        /// it also Commits the transaction.
        /// </summary>
        public void Dispose()
        {
            transaction.Dispose();
        }
    }
}
