
using System.Data.Common;
using System.Security.Permissions;
using System.Security;

namespace BlackHole.Ray
{
    public sealed class RayFactory : DbProviderFactory
    {

        public static readonly RayFactory Instance = new RayFactory();

        private RayFactory()
        {
        }

        public override DbCommand CreateCommand()
        {
            return new RayCommand();
        }

        public override DbCommandBuilder CreateCommandBuilder()
        {
            return new RayCommandBuilder();
        }

        public override DbConnection CreateConnection()
        {
            return new RayConnection();
        }

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return new RayConnectionStringBuilder();
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            return new RayDataAdapter();
        }

        public override DbParameter CreateParameter()
        {
            return new RayParameter();
        }

    }
}
