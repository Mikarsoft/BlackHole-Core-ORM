namespace BlackHole.Internal
{
    internal interface IBHDatabaseBuilder
    {
        bool CheckDatabaseExistance();
        bool DropDatabase();
        bool DoesDbExists();
    }
}
