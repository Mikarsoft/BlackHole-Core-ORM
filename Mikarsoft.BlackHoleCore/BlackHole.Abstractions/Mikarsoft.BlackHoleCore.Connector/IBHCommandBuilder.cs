namespace Mikarsoft.BlackHoleCore.Connector
{
    public interface IBHCommandBuilder
    {
        string SelectCommand<T>();
        string SelectCommand<T, Dto>();

        string UpdateCommand<T>();
        string UpdateCommand<T, Dto>();

        string InsertCommand<T>();
        string RestoreCommand<T>();

        string DeleteCommand<T>();
        string DeactivateCommand<T>();

        BHStatement WhereStatement(BHExpressionPart[] parts, string? command = null, int currentIndex = 0, string? letter = null);
    }
}
