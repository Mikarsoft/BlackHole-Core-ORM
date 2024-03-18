using BlackHole.Core;
using BlackHole.Engine;
using BlackHole.Entities;
using BlackHole.Identifiers;

namespace BlackHole.PreLoads
{
    /// <summary>
    /// 
    /// </summary>
    internal class DefaultDataBuilder : IDefaultDataBuilder
    {
        internal InitialTransaction _initialTransaction;

        internal DefaultDataBuilder(InitialTransaction initialTransaction)
        {
            _initialTransaction = initialTransaction;
        }

        public void InsertEntrty<T, G>(T entry) where G: IComparable<G> where T : BlackHoleEntity<G>
        {
            _initialTransaction.hasChanges +=1;
            IBHDataProvider<T,G> provider = new BHDataProvider<T, G>();
            provider.InsertEntry(entry, _initialTransaction.bhTransaction);
        }

        public void InsertOpenEntry<T>(T entry) where T: BHOpenEntity<T>
        {
            _initialTransaction.hasChanges += 1;
            IBHOpenDataProvider<T> provider = new BHOpenDataProvider<T>();
            provider.InsertEntry(entry, _initialTransaction.bhTransaction);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    internal class StoredViewsBuilder : IStoredViewsBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <returns></returns>
        public IStoredViewsProcess<Dto> StartJoinUsing<Dto>() where Dto : BHDtoIdentifier
        {
            return new StoredViewsProcess<Dto>();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StoredProceduresBuilder : IStoredProceduresBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        public IStoredProcedureProcess<Dto> DeclareExisting<Dto>(string procedureName) where Dto : BHDtoIdentifier
        {
            return new StoredProcedureProcess<Dto>(procedureName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public IStoredProcedureProcess<Dto> CreateOrUpdate<Dto>(string procedureName, string commandText) where Dto : BHDtoIdentifier
        {
            return new StoredProcedureProcess<Dto>(procedureName, commandText);
        }
    }
}
