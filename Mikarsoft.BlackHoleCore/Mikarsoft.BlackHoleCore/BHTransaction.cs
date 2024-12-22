using Mikarsoft.BlackHoleCore.Connector;

namespace Mikarsoft.BlackHoleCore
{
    internal class BHTransaction : IBHTransaction
    {
        internal IBHInnerTransaction _innerTransaction;

        internal BHTransaction()
        {
            _innerTransaction = BHServiceInjector.GetInnerTransaction();
        }

        public bool Commit() => _innerTransaction.Commit();

        public bool DoNotCommit() => _innerTransaction.DoNotCommit();

        public bool RollBack() => _innerTransaction.Rollback();

        public void Dispose() => _innerTransaction.Dispose();
    }
}
