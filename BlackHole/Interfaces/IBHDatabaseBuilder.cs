
namespace BlackHole.Interfaces
{
    internal interface IBHDatabaseBuilder
    {
        bool CheckDatabaseExistance();
        bool DropDatabase();
        bool DoesDbExists();
    }
}
