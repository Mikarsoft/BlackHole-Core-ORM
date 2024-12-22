using Mikarsoft.BlackHoleCore.Connector;

namespace Mikarsoft.BlackHoleCore.Npgsql
{
    internal class BlackHoleNpgsqlCommandBuilder : IBHCommandBuilder
    {
        public string DeactivateCommand<T>()
        {
            throw new NotImplementedException();
        }

        public string DeleteCommand<T>()
        {
            throw new NotImplementedException();
        }

        public string InsertCommand<T>()
        {
            throw new NotImplementedException();
        }

        public string RestoreCommand<T>()
        {
            throw new NotImplementedException();
        }

        public string SelectCommand<T>()
        {
            throw new NotImplementedException();
        }

        public string SelectCommand<T, Dto>()
        {
            throw new NotImplementedException();
        }

        public string UpdateCommand<T>()
        {
            throw new NotImplementedException();
        }

        public string UpdateCommand<T, Dto>()
        {
            throw new NotImplementedException();
        }

        public BHStatement WhereStatement(BHExpressionPart[] parts, string? command = null, int currentIndex = 0, string? letter = null)
        {
            throw new NotImplementedException();
        }
    }
}
