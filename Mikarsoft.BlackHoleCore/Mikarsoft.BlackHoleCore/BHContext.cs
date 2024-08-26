using Mikarsoft.BlackHoleCore.Entities;

namespace Mikarsoft.BlackHoleCore
{
    internal class BHContext : BHContextBase, IBHContext
    {
        public IBHTable<T> Table<T>() where T : BHEntity<T>
        {
            return new BHTable<T>();
        }

        public IBHTable<T, G> Table<T, G>()
            where T : BHEntityAI<T, G>
            where G : struct, IBHStruct
        {
            return new BHTable<T, G>();
        }
    }

    internal class BHContextBase : IBHContextBase
    {
        public IBHTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public IBHCommand Command(string commandText, string? databaseIdentity = null)
        {
            throw new NotImplementedException();
        }

        public BHParameters CreateParameters()
        {
            return new BHParameters();
        }
    }
}
