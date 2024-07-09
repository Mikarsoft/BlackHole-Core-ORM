namespace BlackHole.Core
{
    internal class BHContextBase : IBHContextBase
    {
        public IBHTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public IBHCommand Command(string commandText, string? databaseIdentity = null)
        {
            throw new NotImplementedException();
        }

        public IBHParameters CreateParameters()
        {
            throw new NotImplementedException();
        }
    }
}
