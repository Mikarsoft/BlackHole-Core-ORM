using System.Data;

namespace Mikarsoft.BlackHoleCore.Connector
{
    public interface IBHInnerTransaction : IDisposable
    {
        IDbTransaction GetTransaction(int connectionIndex);
        IDbConnection GetConnection(int connectionIndex);

        void SetError(bool error);
        bool HasError { get;}
    }
}
