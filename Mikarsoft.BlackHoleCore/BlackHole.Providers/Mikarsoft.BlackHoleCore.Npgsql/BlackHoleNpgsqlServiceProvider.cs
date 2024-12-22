using Mikarsoft.BlackHoleCore.Connector;

namespace Mikarsoft.BlackHoleCore.Npgsql
{
    internal class BlackHoleNpgsqlServiceProvider : IBHServiceProvider
    {
        public IBHCommandBuilder GetCommandBuilder()
        {
            throw new NotImplementedException();
        }

        public IBHDataProvider GetDataProvider()
        {
            throw new NotImplementedException();
        }

        public IBHInnerTransaction GetInnerTransaction()
        {
            throw new NotImplementedException();
        }

        public IBHMethodParser GetMethodParser()
        {
            throw new NotImplementedException();
        }
    }
}
