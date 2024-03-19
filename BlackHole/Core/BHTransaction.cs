using BlackHole.Engine;

namespace BlackHole.Core
{
    internal class BHTransaction : IBHTransaction
    {
        internal BlackHoleTransaction transaction;

        internal BHTransaction()
        {
            transaction = new BlackHoleTransaction(BlackHoleEngine.GetAvailableConnections());
        }

        public bool Commit()
        {
            return transaction.Commit();
        }

        public bool DoNotCommit()
        {
            return transaction.DoNotCommit();
        }

        public bool RollBack()
        {
            return transaction.RollBack();
        }

        public void Dispose()
        {
            transaction.Dispose();
        }
    }
}
