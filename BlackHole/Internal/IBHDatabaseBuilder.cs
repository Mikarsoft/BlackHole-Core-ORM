namespace BlackHole.Internal
{
    internal interface IBHDatabaseBuilder
    {
        bool CreateDatabase();
        bool DropDatabase();
        bool DoesDbExists();
        bool IsCreatedFirstTime();
    }
}
