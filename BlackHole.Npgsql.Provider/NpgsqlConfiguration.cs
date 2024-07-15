using BlackHole.Provider.Abstractions;

namespace BlackHole.Npgsql.Provider
{
    public static class NpgsqlConfiguration
    {
        public static NpgsqlConfig UseNpgsql(this BlackHoleBaseConfig configBase ,int connectionTimeout = 60)
        {
            configBase._databaseConfig = new NpgsqlConfig(true, connectionTimeout);
            return (NpgsqlConfig)configBase._databaseConfig;
        }
    }
}
