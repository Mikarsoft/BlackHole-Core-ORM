using BlackHole.Configuration.ConfigTypes;

namespace BlackHole.Configuration
{
    public class BlackHoleSetup
    {

        public SqlServerConfig UseSqlServer()
        {
            return new SqlServerConfig();
        }
    }
}
