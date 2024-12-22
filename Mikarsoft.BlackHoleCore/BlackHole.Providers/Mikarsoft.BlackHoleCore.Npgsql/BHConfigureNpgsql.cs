using Mikarsoft.BlackHoleCore.Configuration;

namespace Mikarsoft.BlackHoleCore.Npgsql
{
    public static class BHConfigureNpgsql
    {
        public static void UseNpgsql(this BHBaseConfig config,  string connectionString)
        {
            config.BHServiceProvider = new BlackHoleNpgsqlServiceProvider();
        }
    }
}
