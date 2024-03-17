using BlackHole.Engine;
using BlackHole.Entities;
using BlackHole.Identifiers;

namespace BlackHole.Core
{
    /// <summary>
    /// Queries for the Table Joins Feature
    /// </summary>
    public static class BHExtensions
    {
        #region Joins Execution
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="data"></param>
        /// <param name="bhTransaction"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="data"></param>
        /// <param name="bhTransaction"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="data"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="data"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="data"></param>
        /// <param name="orderBy"></param>
        /// <param name="bhTransaction"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="data"></param>
        /// <param name="orderBy"></param>
        /// <param name="bhTransaction"></param>
        /// <returns></returns>
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
        #endregion
    }
}
