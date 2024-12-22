namespace Mikarsoft.BlackHoleCore.Connector
{
    public interface IBHServiceProvider
    {
        IBHCommandBuilder GetCommandBuilder();

        IBHDataProvider GetDataProvider();

        IBHMethodParser GetMethodParser();

        IBHInnerTransaction GetInnerTransaction();
    }
}
