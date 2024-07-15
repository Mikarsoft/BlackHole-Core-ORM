
namespace BlackHole.Provider.Abstractions
{
    public class BlackHoleBaseConfig
    {
        public BlackHoleBaseConfig()
        {
            DataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BlackHoleData");
            UseLogger = true;
            UseLogsCleaner = true;
            CleanUpDays = 60;
        }

        public BHDatabaseConfig? _databaseConfig { get; set; }

        public string DataPath { get; set; }

        public bool UseLogger { get; set; }

        public bool UseLogsCleaner { get; set; }

        public int CleanUpDays { get; set; }
    }
}
