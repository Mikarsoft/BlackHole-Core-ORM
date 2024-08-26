namespace Mikarsoft.BlackHoleCore.Connector
{
    public interface IBHInnerTransaction : IDisposable
    {
        bool Commit();
        bool DoNotCommit();
        bool Rollback();
    }
}
