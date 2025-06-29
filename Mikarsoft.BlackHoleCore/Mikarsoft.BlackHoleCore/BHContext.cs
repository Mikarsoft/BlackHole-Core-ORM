
namespace Mikarsoft.BlackHoleCore
{
    internal class BHContext : BHContextBase, IBHContext
    {
        IBHTable<T> IBHContext.Table<T>()
        {
            throw new NotImplementedException();
        }

        IBHTable<T, G> IBHContext.Table<T, G>()
        {
            throw new NotImplementedException();
        }
    }

    internal class BHContextBase : IBHContextBase
    {
        IBHTransaction IBHContextBase.BeginTransaction()
        {
            throw new NotImplementedException();
        }

        IBHCommand IBHContextBase.Command(string commandText, string? databaseIdentity)
        {
            throw new NotImplementedException();
        }

        BHParameters IBHContextBase.CreateParameters()
        {
            throw new NotImplementedException();
        }
    }
}
