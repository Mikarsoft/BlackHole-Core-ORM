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
        /// Stores the Joins Data with the DTO Class as Identifier
        /// and then you can execute them as many times as you want
        /// using the 'IBlackHoleViewStorage' Interface.
        /// <para><b>Benefit</b> : With this method, the program doesn't have to calculate the
        /// Joins Data multiple times and it executes the Joins faster.</para>
        /// <para><b>Tip</b> : This method is recommended if the parameters in the current
        /// Joins Data are not depending on the user's inputs.
        /// Run your Joins Once in the StartUp of your program and store them
        /// as Views.</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="data">Joins Data</param>
        /// <returns>The index of this Joins Data in the Stored Views List</returns>
        public static int StoreAsView<Dto>(this JoinComplete<Dto> data) where Dto : BHDtoIdentifier
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.FirstOrDefault(x => x.DtoType == typeof(Dto));

            if (existingJoin != null)
            {
                BlackHoleViews.Stored.Remove(existingJoin);
            }

            BlackHoleViews.Stored.Add(data.Data);

            return BlackHoleViews.Stored.Count;
        }

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
