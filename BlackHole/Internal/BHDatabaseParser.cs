
namespace BlackHole.Internal
{
    internal class BHDatabaseParser
    {
        private string SQLServerSelectTables = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA = 'adoo'";
    }
}
