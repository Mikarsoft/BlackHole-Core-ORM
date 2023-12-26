using BlackHole.CoreSupport;
using BlackHole.Identifiers;

namespace BlackHole.Core
{
    /// <summary>
    /// Queries for the Table Joins Feature
    /// </summary>
    public static class BHExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<Dto> ExecuteQuery<Dto>(this JoinComplete<Dto> data) where Dto: BHDtoIdentifier
        {
            if(data.Data.TranslateJoin<Dto>() is ColumnsAndParameters command)
            {
                IDataProvider connection = BHCore.GetDataProvider();
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
        public static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinComplete<Dto> data) where Dto : BHDtoIdentifier
        {
            if (data.Data.TranslateJoin<Dto>() is ColumnsAndParameters command)
            {
                IDataProvider connection = BHCore.GetDataProvider();
                return await connection.QueryAsync<Dto>(command.Columns, command.Parameters);
            }
            return new List<Dto>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="data"></param>
        /// <param name="bHTransaction"></param>
        /// <returns></returns>
        public static List<Dto> ExecuteQuery<Dto>(this JoinComplete<Dto> data, BHTransaction bHTransaction) where Dto : BHDtoIdentifier
        {
            if (data.Data.TranslateJoin<Dto>() is ColumnsAndParameters command)
            {
                IDataProvider connection = BHCore.GetDataProvider();
                return connection.Query<Dto>(command.Columns, command.Parameters, bHTransaction.transaction);
            }
            return new List<Dto>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="data"></param>
        /// <param name="bHTransaction"></param>
        /// <returns></returns>
        public static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinComplete<Dto> data, BHTransaction bHTransaction) where Dto : BHDtoIdentifier
        {
            if (data.Data.TranslateJoin<Dto>() is ColumnsAndParameters command)
            {
                IDataProvider connection = BHCore.GetDataProvider();
                return await connection.QueryAsync<Dto>(command.Columns, command.Parameters, bHTransaction.transaction);
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
        public static List<Dto> ExecuteQuery<Dto>(this JoinComplete<Dto> data, Action<BHOrderBy<Dto>> orderBy) where Dto : BHDtoIdentifier
        {
            if (data.Data.TranslateJoin<Dto>() is ColumnsAndParameters command)
            {
                BHOrderBy<Dto> orderClass = new();
                orderBy.Invoke(orderClass);
                data.Data.OrderByToSqlJoins(orderClass);
                IDataProvider connection = BHCore.GetDataProvider();
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
        public static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinComplete<Dto> data, Action<BHOrderBy<Dto>> orderBy) where Dto : BHDtoIdentifier
        {
            if (data.Data.TranslateJoin<Dto>() is ColumnsAndParameters command)
            {
                BHOrderBy<Dto> orderClass = new();
                orderBy.Invoke(orderClass);
                data.Data.OrderByToSqlJoins(orderClass);
                IDataProvider connection = BHCore.GetDataProvider();
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
        /// <param name="bHTransaction"></param>
        /// <returns></returns>
        public static List<Dto> ExecuteQuery<Dto>(this JoinComplete<Dto> data, Action<BHOrderBy<Dto>> orderBy, BHTransaction bHTransaction) where Dto : BHDtoIdentifier
        {
            if (data.Data.TranslateJoin<Dto>() is ColumnsAndParameters command)
            {
                BHOrderBy<Dto> orderClass = new();
                orderBy.Invoke(orderClass);
                data.Data.OrderByToSqlJoins(orderClass);
                IDataProvider connection = BHCore.GetDataProvider();
                return connection.Query<Dto>(command.Columns, command.Parameters, bHTransaction.transaction);
            }
            return new List<Dto>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="data"></param>
        /// <param name="orderBy"></param>
        /// <param name="bHTransaction"></param>
        /// <returns></returns>
        public static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinComplete<Dto> data, Action<BHOrderBy<Dto>> orderBy, BHTransaction bHTransaction) where Dto : BHDtoIdentifier
        {
            if (data.Data.TranslateJoin<Dto>() is ColumnsAndParameters command)
            {
                BHOrderBy<Dto> orderClass = new();
                orderBy.Invoke(orderClass);
                data.Data.OrderByToSqlJoins(orderClass);
                IDataProvider connection = BHCore.GetDataProvider();
                return await connection.QueryAsync<Dto>(command.Columns, command.Parameters, bHTransaction.transaction);
            }
            return new List<Dto>();
        }
    }
}
