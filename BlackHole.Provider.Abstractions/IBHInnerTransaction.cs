using System.Data;

namespace BlackHole.Provider.Abstractions
{
    public interface IBHInnerTransaction : IDisposable
    {
        IDbTransaction GetTransaction(int connectionIndex);
        IDbConnection GetConnection(int connectionIndex);

        void SetError(bool error);
        bool HasError { get;}
    }
}
