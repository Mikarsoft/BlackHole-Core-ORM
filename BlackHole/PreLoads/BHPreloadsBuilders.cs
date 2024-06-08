using BlackHole.Core;
using BlackHole.Entities;
using BlackHole.Identifiers;

namespace BlackHole.PreLoads
{
    internal class InitialTransaction
    {
        internal BHTransaction bhTransaction;

        internal int hasChanges;

        internal InitialTransaction()
        {
            bhTransaction = new();
        }

        internal bool Commit()
        {
            bool success;

            if (hasChanges > 0)
            {
                success = bhTransaction.Commit();
            }
            else
            {
                success = bhTransaction.DoNotCommit();
            }

            bhTransaction.Dispose();
            return success;
        }
    }

    internal class DefaultDataBuilder : IDefaultDataBuilder
    {
        internal InitialTransaction _initialTransaction;

        internal DefaultDataBuilder(InitialTransaction initialTransaction)
        {
            _initialTransaction = initialTransaction;
        }

        public void InsertEntrty<T, G>(T entry) where G: IComparable<G> where T : BHEntityAI<G>
        {
            _initialTransaction.hasChanges +=1;
            IBHDataProvider<T,G> provider = new BHDataProvider<T, G>();
            provider.InsertEntry(entry, _initialTransaction.bhTransaction);
        }

        public void InsertOpenEntry<T>(T entry) where T: BHEntity<T>
        {
            _initialTransaction.hasChanges += 1;
            IBHOpenDataProvider<T> provider = new BHOpenDataProvider<T>();
            provider.InsertEntry(entry, _initialTransaction.bhTransaction);
        }
    }

    internal class StoredViewsBuilder : IStoredViewsBuilder
    {
        public IStoredViewsProcess<Dto> CreateViewUsing<Dto>() where Dto : BHDtoIdentifier
        {
            return new StoredViewsProcess<Dto>();
        }
    }

    internal class StoredProceduresBuilder : IStoredProceduresBuilder
    {
        public IStoredProcedureProcess<Dto> DeclareExisting<Dto>(string procedureName) where Dto : BHDtoIdentifier
        {
            return new StoredProcedureProcess<Dto>(procedureName);
        }

        public IStoredProcedureProcess<Dto> DeclareExisting<Dto>(string procedureName, string[] parameters) where Dto : BHDtoIdentifier
        {
            return new StoredProcedureProcess<Dto>(procedureName);
        }

        public IStoredProcedureProcess<Dto> CreateOrUpdate<Dto>(string procedureName, string commandText) where Dto : BHDtoIdentifier
        {
            return new StoredProcedureProcess<Dto>(procedureName, commandText);
        }
    }
}
