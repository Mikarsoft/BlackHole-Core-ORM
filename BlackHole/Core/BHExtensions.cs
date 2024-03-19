using BlackHole.Engine;
using BlackHole.Identifiers;

namespace BlackHole.Core
{
    internal static class BHExtensions
    {
        internal static List<Dto> ExecuteQuery<Dto>(this JoinsData data) where Dto: BHDtoIdentifier
        {
            if(data.TranslateJoin<Dto>() is ColumnsAndParameters command)
            {
                int connectionIndex = data.ConnectionIndex;
                IDataProvider connection = connectionIndex.GetDataProvider();
                return connection.Query<Dto>(command.Columns, command.Parameters);
            }
            return new List<Dto>();
        }

        internal static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinsData data) where Dto : BHDtoIdentifier
        {
            if (data.TranslateJoin<Dto>() is ColumnsAndParameters command)
            {
                int connectionIndex = data.ConnectionIndex;
                IDataProvider connection = connectionIndex.GetDataProvider();
                return await connection.QueryAsync<Dto>(command.Columns, command.Parameters);
            }
            return new List<Dto>();
        }

        internal static List<Dto> ExecuteQuery<Dto>(this JoinsData data, IBHTransaction bhTransaction) where Dto : BHDtoIdentifier
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;

            if (data.TranslateJoin<Dto>() is ColumnsAndParameters command)
            {
                int connectionIndex = data.ConnectionIndex;
                IDataProvider connection = connectionIndex.GetDataProvider();
                return connection.Query<Dto>(command.Columns, command.Parameters, transactionBh.transaction, connectionIndex);
            }
            return new List<Dto>();
        }

        internal static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinsData data, IBHTransaction bhTransaction) where Dto : BHDtoIdentifier
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;

            if (data.TranslateJoin<Dto>() is ColumnsAndParameters command)
            {
                int connectionIndex = data.ConnectionIndex;
                IDataProvider connection = connectionIndex.GetDataProvider();
                return await connection.QueryAsync<Dto>(command.Columns, command.Parameters, transactionBh.transaction, connectionIndex);
            }
            return new List<Dto>();
        }

        internal static List<Dto> ExecuteQuery<Dto>(this JoinsData data, Action<BHOrderBy<Dto>> orderBy) where Dto : BHDtoIdentifier
        {
            if (data.TranslateJoin<Dto>() is ColumnsAndParameters command)
            {
                BHOrderBy<Dto> orderClass = new();
                orderBy.Invoke(orderClass);
                data.OrderByToSqlJoins(orderClass);
                int connectionIndex = data.ConnectionIndex;
                IDataProvider connection = connectionIndex.GetDataProvider();
                return connection.Query<Dto>(command.Columns, command.Parameters);
            }
            return new List<Dto>();
        }

        internal static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinsData data, Action<BHOrderBy<Dto>> orderBy) where Dto : BHDtoIdentifier
        {
            if (data.TranslateJoin<Dto>() is ColumnsAndParameters command)
            {
                BHOrderBy<Dto> orderClass = new();
                orderBy.Invoke(orderClass);
                data.OrderByToSqlJoins(orderClass);
                int connectionIndex = data.ConnectionIndex;
                IDataProvider connection = connectionIndex.GetDataProvider();
                return await connection.QueryAsync<Dto>(command.Columns, command.Parameters);
            }
            return new List<Dto>();
        }

        internal static List<Dto> ExecuteQuery<Dto>(this JoinsData data, Action<BHOrderBy<Dto>> orderBy, IBHTransaction bhTransaction) where Dto : BHDtoIdentifier
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;

            if (data.TranslateJoin<Dto>() is ColumnsAndParameters command)
            {
                BHOrderBy<Dto> orderClass = new();
                orderBy.Invoke(orderClass);
                data.OrderByToSqlJoins(orderClass);
                int connectionIndex = data.ConnectionIndex;
                IDataProvider connection = connectionIndex.GetDataProvider(); 
                return connection.Query<Dto>(command.Columns, command.Parameters, transactionBh.transaction, connectionIndex);
            }
            return new List<Dto>();
        }

        internal static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinsData data, Action<BHOrderBy<Dto>> orderBy, IBHTransaction bhTransaction) where Dto : BHDtoIdentifier
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;

            if (data.TranslateJoin<Dto>() is ColumnsAndParameters command)
            {
                BHOrderBy<Dto> orderClass = new();
                orderBy.Invoke(orderClass);
                data.OrderByToSqlJoins(orderClass);
                int connectionIndex = data.ConnectionIndex;
                IDataProvider connection = connectionIndex.GetDataProvider();
                return await connection.QueryAsync<Dto>(command.Columns, command.Parameters, transactionBh.transaction, connectionIndex);
            }
            return new List<Dto>();
        }
    }
}
