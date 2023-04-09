using BlackHole.CoreSupport;

namespace BlackHole.Core
{
    public class BHTransaction : IDisposable
    {
        internal BlackHoleTransaction transaction;

        public BHTransaction()
        {
            transaction = new BlackHoleTransaction();
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
