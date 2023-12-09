using BlackHole.Entities;
using BlackHole.Enums;
using BlackHole.Statics;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using BlackHole.CoreSupport;
using BlackHole.ExecutionProviders;
using BlackHole.Identifiers;
using System.Text;

namespace BlackHole.Core
{
    /// <summary>
    /// Queries for the Table Joins Feature
    /// </summary>
    public static class BHExtensions
    {
        #region Joins
        /// <summary>
        /// Performs a Right Join between the First and the Second specified Entities. 
        /// <para><b>Important</b> => For safety reasons, The first Entity must have been used 
        /// at least once in the previous Joins, otherwise this Join and its settings will be ignored on the 
        /// Execution and you might get some null values on the exported DTOs.</para>
        /// </summary>
        /// <typeparam name="Tsource">First Entity</typeparam>
        /// <typeparam name="TOther">Second Entity</typeparam>
        /// <typeparam name="Tkey">Type of their Joint Column</typeparam>
        /// <typeparam name="Dto">Class of the Output</typeparam>
        /// <param name="data">Previous Joins Data</param>
        /// <param name="key">First Table Joint Column</param>
        /// <param name="otherkey">Second Table Joint Column</param>
        /// <returns>The Calculated Data of this Join</returns>
        public static JoinsData<Dto, Tsource, TOther> RightJoinOn<Tsource, TOther, Tkey, Dto>(this JoinsData<Dto> data,
            Expression<Func<Tsource, Tkey>> key, Expression<Func<TOther, Tkey>> otherkey)
            where TOther : IBHEntityIdentifier where Tsource : IBHEntityIdentifier where Tkey : IComparable
        {
            JoinsData<Dto, Tsource, TOther> newJoin = new()
            {
                BaseTable = data.BaseTable,
                OccupiedDtoProps = data.OccupiedDtoProps,
                TablesToLetters = data.TablesToLetters,
                Joins = data.Joins,
                Letters = data.Letters,
                WherePredicates = data.WherePredicates,
                DynamicParams = data.DynamicParams,
                HelperIndex = data.HelperIndex,
                isMyShit = data.isMyShit,
                ParamsCount = data.ParamsCount,
                Ignore = data.Ignore
            };

            return newJoin.CreateJoin(key, otherkey, "right");
        }

        /// <summary>
        /// Performs a Left Join between the First and the Second specified Entities. 
        /// <para><b>Important</b> => For safety reasons, The first Entity must have been used 
        /// at least once in the previous Joins, otherwise this Join and its settings will be ignored on the 
        /// Execution and you might get some null values on the exported DTOs.</para>
        /// </summary>
        /// <typeparam name="Tsource">First Entity</typeparam>
        /// <typeparam name="TOther">Second Entity</typeparam>
        /// <typeparam name="Tkey">Type of their Joint Column</typeparam>
        /// <typeparam name="Dto">Class of the Output</typeparam>
        /// <param name="data">Previous Joins Data</param>
        /// <param name="key">First Table Joint Column</param>
        /// <param name="otherkey">Second Table Joint Column</param>
        /// <returns>The Calculated Data of this Join</returns>
        public static JoinsData<Dto, Tsource, TOther> LeftJoinOn<Tsource, TOther, Tkey, Dto>(this JoinsData<Dto> data,
            Expression<Func<Tsource, Tkey>> key, Expression<Func<TOther, Tkey>> otherkey)
            where TOther : IBHEntityIdentifier where Tsource : IBHEntityIdentifier where Tkey : IComparable
        {
            JoinsData<Dto, Tsource, TOther> newJoin = new()
            {
                BaseTable = data.BaseTable,
                OccupiedDtoProps = data.OccupiedDtoProps,
                TablesToLetters = data.TablesToLetters,
                Joins = data.Joins,
                Letters = data.Letters,
                WherePredicates = data.WherePredicates,
                DynamicParams = data.DynamicParams,
                HelperIndex = data.HelperIndex,
                isMyShit = data.isMyShit,
                ParamsCount = data.ParamsCount,
                Ignore = data.Ignore
            };

            return newJoin.CreateJoin(key, otherkey, "left");
        }

        /// <summary>
        /// Performs an Outer Join between the First and the Second specified Entities. 
        /// <para><b>Important</b> => For safety reasons, The first Entity must have been used 
        /// at least once in the previous Joins, otherwise this Join and its settings will be ignored on the 
        /// Execution and you might get some null values on the exported DTOs.</para>
        /// </summary>
        /// <typeparam name="Tsource">First Entity</typeparam>
        /// <typeparam name="TOther">Second Entity</typeparam>
        /// <typeparam name="Tkey">Type of their Joint Column</typeparam>
        /// <typeparam name="Dto">Class of the Output</typeparam>
        /// <param name="data">Previous Joins Data</param>
        /// <param name="key">First Table Joint Column</param>
        /// <param name="otherkey">Second Table Joint Column</param>
        /// <returns>The Calculated Data of this Join</returns>
        public static JoinsData<Dto, Tsource, TOther> OuterJoinOn<Tsource, TOther, Tkey, Dto>(this JoinsData<Dto> data,
            Expression<Func<Tsource, Tkey>> key, Expression<Func<TOther, Tkey>> otherkey)
            where TOther : IBHEntityIdentifier where Tsource : IBHEntityIdentifier where Tkey : IComparable
        {
            JoinsData<Dto, Tsource, TOther> newJoin = new()
            {
                BaseTable = data.BaseTable,
                OccupiedDtoProps = data.OccupiedDtoProps,
                TablesToLetters = data.TablesToLetters,
                Joins = data.Joins,
                Letters = data.Letters,
                WherePredicates = data.WherePredicates,
                DynamicParams = data.DynamicParams,
                HelperIndex = data.HelperIndex,
                isMyShit = data.isMyShit,
                ParamsCount = data.ParamsCount,
                Ignore = data.Ignore
            };

            return newJoin.CreateJoin(key, otherkey, "full outer");
        }

        /// <summary>
        /// Performs an Inner Join between the First and the Second specified Entities. 
        /// <para><b>Important</b> => For safety reasons, The first Entity must have been used 
        /// at least once in the previous Joins, otherwise this Join and its settings will be ignored on the 
        /// Execution and you might get some null values on the exported DTOs.</para>
        /// </summary>
        /// <typeparam name="Tsource">First Entity</typeparam>
        /// <typeparam name="TOther">Second Entity</typeparam>
        /// <typeparam name="Tkey">Type of their Joint Column</typeparam>
        /// <typeparam name="Dto">Class of the Output</typeparam>
        /// <param name="data">Previous Joins Data</param>
        /// <param name="key">First Table Joint Column</param>
        /// <param name="otherkey">Second Table Joint Column</param>
        /// <returns>The Calculated Data of this Join</returns>
        public static JoinsData<Dto, Tsource, TOther> InnerJoinOn<Tsource, TOther, Tkey, Dto>(this JoinsData<Dto> data,
            Expression<Func<Tsource, Tkey>> key, Expression<Func<TOther, Tkey>> otherkey)
            where TOther : IBHEntityIdentifier where Tsource : IBHEntityIdentifier where Tkey : IComparable where Dto : IBHDtoIdentifier
        {
            JoinsData<Dto, Tsource, TOther> newJoin = new()
            {
                BaseTable = data.BaseTable,
                OccupiedDtoProps = data.OccupiedDtoProps,
                TablesToLetters = data.TablesToLetters,
                Joins = data.Joins,
                Letters = data.Letters,
                WherePredicates = data.WherePredicates,
                DynamicParams = data.DynamicParams,
                HelperIndex = data.HelperIndex,
                isMyShit = data.isMyShit,
                ParamsCount = data.ParamsCount,
                Ignore = data.Ignore,
            };

            return newJoin.CreateJoin(key, otherkey, "inner");
        }
        #endregion

        #region Additional Join Methods
        /// <summary>
        /// Uses Additional columns to the Join
        /// </summary>
        /// <typeparam name="Dto">Class of the Output</typeparam>
        /// <typeparam name="Tsource">First Entity</typeparam>
        /// <typeparam name="TOther">Second Entity</typeparam>
        /// <typeparam name="Tkey">Type of their Joint Column</typeparam>
        /// <param name="data">Previous Joins Data</param>
        /// <param name="key">First Table Joint Column</param>
        /// <param name="otherkey">Second Table Joint Column</param>
        /// <returns>The Calculated Data of this Join</returns>
        public static JoinsData<Dto, Tsource, TOther> And<Dto, Tsource, TOther, Tkey>(this JoinsData<Dto, Tsource, TOther> data,
            Expression<Func<Tsource, Tkey>> key, Expression<Func<TOther, Tkey>> otherkey) where Tkey : IComparable
        {
            if (data.Ignore)
            {
                return data;
            }

            string? firstLetter = data.TablesToLetters.Where(t => t.Table == typeof(Tsource)).First().Letter;
            string? secondLetter = data.TablesToLetters.Where(t => t.Table == typeof(TOther)).First().Letter;

            MemberExpression? member = key.Body as MemberExpression;
            string? propName = member?.Member.Name;
            MemberExpression? memberOther = otherkey.Body as MemberExpression;
            string? propNameOther = memberOther?.Member.Name;

            data.Joins += $" and {secondLetter}.{propNameOther.SkipNameQuotes(data.isMyShit)} = {firstLetter}.{propName.SkipNameQuotes(data.isMyShit)}";
            return data;
        }

        /// <summary>
        /// Uses Additional case to the Join
        /// </summary>
        /// <typeparam name="Dto">Class of the Output</typeparam>
        /// <typeparam name="Tsource">First Entity</typeparam>
        /// <typeparam name="TOther">Second Entity</typeparam>
        /// <typeparam name="Tkey">Type of their Joint Column</typeparam>
        /// <param name="data">Previous Joins Data</param>
        /// <param name="key">First Table Joint Column</param>
        /// <param name="otherkey">Second Table Joint Column</param>
        /// <returns>The Calculated Data of this Join</returns>
        public static JoinsData<Dto, Tsource, TOther> Or<Dto, Tsource, TOther, Tkey>(this JoinsData<Dto, Tsource, TOther> data,
            Expression<Func<Tsource, Tkey>> key, Expression<Func<TOther, Tkey>> otherkey) where Tkey : IComparable
        {
            if (data.Ignore)
            {
                return data;
            }

            string? firstLetter = data.TablesToLetters.Where(t => t.Table == typeof(Tsource)).First().Letter;
            string? secondLetter = data.TablesToLetters.Where(t => t.Table == typeof(TOther)).First().Letter;

            MemberExpression? member = key.Body as MemberExpression;
            string? propName = member?.Member.Name;
            MemberExpression? memberOther = otherkey.Body as MemberExpression;
            string? propNameOther = memberOther?.Member.Name;

            data.Joins += $" or {secondLetter}.{propNameOther.SkipNameQuotes(data.isMyShit)} = {firstLetter}.{propName.SkipNameQuotes(data.isMyShit)}";
            return data;
        }

        /// <summary>
        /// Casts a column of the second Entity as a column of the 
        /// output's DTO. 
        /// <para><b>Important</b> => There are some restrictions
        /// to the types of the properties that can be casted. Read the Documentation
        /// for details. If a data type is not allowed to be converted to another type,
        /// then the cast will be ignored in the Execution and the DTO column will be null.</para>
        /// <para><b>Tip</b> : Cast has priority over normal mapping, For example the Column Id of the DTO is 
        /// by default mapped to the First Entity of all Joins. If you want to map a different
        /// Entity's Id into that column, use a Cast.</para>
        /// </summary>
        /// <typeparam name="Dto">Class of the Output</typeparam>
        /// <typeparam name="Tsource">First Entity</typeparam>
        /// <typeparam name="TOther">Second Entity</typeparam>
        /// <typeparam name="Tkey">Type of their Joint Column</typeparam>
        /// <typeparam name="TotherKey">Type of the Dto property</typeparam>
        /// <param name="data">Previous Joins Data</param>
        /// <param name="predicate">First Table Joint Column</param>
        /// <param name="castOnDto">Second Table Joint Column</param>
        /// <returns>The Calculated Data of this Join</returns>
        public static JoinsData<Dto, Tsource, TOther> CastColumnOfSecondAs<Dto, Tsource, TOther, Tkey, TotherKey>(this JoinsData<Dto, Tsource, TOther> data,
            Expression<Func<TOther, Tkey>> predicate, Expression<Func<Dto, TotherKey>> castOnDto)
        {
            if (data.Ignore)
            {
                return data;
            }

            Type propertyType = predicate.Body.Type;
            MemberExpression? member = predicate.Body as MemberExpression;
            string? propName = member?.Member.Name;

            Type dtoPropType = castOnDto.Body.Type;
            MemberExpression? memberOther = castOnDto.Body as MemberExpression;
            string? propNameOther = memberOther?.Member.Name;
            int allow = propertyType.AllowCast(dtoPropType);

            if (allow != 0)
            {
                var oDp = data.OccupiedDtoProps.Where(x => x.PropName == propNameOther).First();
                int index = data.OccupiedDtoProps.IndexOf(oDp);
                data.OccupiedDtoProps[index].Occupied = true;
                data.OccupiedDtoProps[index].TableLetter = data.TablesToLetters.Where(x => x.Table == typeof(TOther)).First().Letter;
                data.OccupiedDtoProps[index].TableProperty = propName;
                data.OccupiedDtoProps[index].TablePropertyType = propertyType;
                data.OccupiedDtoProps[index].WithCast = allow;
            }

            return data;
        }

        /// <summary>
        /// Casts a column of the first Entity as a column of the 
        /// output's DTO. 
        /// <para><b>Important</b> => There are some restrictions
        /// to the types of the properties that can be casted. Read the Documentation
        /// for details. If a data type is not allowed to be converted to another type,
        /// then the cast will be ignored in the Execution and the DTO column will be null.</para>
        /// <para><b>Tip</b> : Cast has priority over normal mapping, For example the Column Id of the DTO is 
        /// by default mapped to the First Entity of all Joins. If you want to map a different
        /// Entity's Id into that column, use a Cast.</para>
        /// </summary>
        /// <typeparam name="Dto">Class of the Output</typeparam>
        /// <typeparam name="Tsource">First Entity</typeparam>
        /// <typeparam name="TOther">Second Entity</typeparam>
        /// <typeparam name="Tkey">Type of their Joint Column</typeparam>        
        /// <typeparam name="TotherKey">Type of the Dto property</typeparam>
        /// <param name="data">Previous Joins Data</param>
        /// <param name="predicate">First Table Joint Column</param>
        /// <param name="castOnDto">Second Table Joint Column</param>
        /// <returns>The Calculated Data of this Join</returns>
        public static JoinsData<Dto, Tsource, TOther> CastColumnOfFirstAs<Dto, Tsource, TOther, Tkey, TotherKey>(this JoinsData<Dto, Tsource, TOther> data,
            Expression<Func<Tsource, Tkey>> predicate, Expression<Func<Dto, TotherKey>> castOnDto)
        {
            if (data.Ignore)
            {
                return data;
            }

            Type propertyType = predicate.Body.Type;
            MemberExpression? member = predicate.Body as MemberExpression;
            string? propName = member?.Member.Name;

            Type dtoPropType = castOnDto.Body.Type;
            MemberExpression? memberOther = castOnDto.Body as MemberExpression;
            string? propNameOther = memberOther?.Member.Name;
            int allow = propertyType.AllowCast(dtoPropType);
            if (allow != 0)
            {
                var oDp = data.OccupiedDtoProps.Where(x => x.PropName == propNameOther).First();
                int index = data.OccupiedDtoProps.IndexOf(oDp);
                data.OccupiedDtoProps[index].Occupied = true;
                data.OccupiedDtoProps[index].TableLetter = data.TablesToLetters.Where(x => x.Table == typeof(Tsource)).First().Letter;
                data.OccupiedDtoProps[index].TableProperty = propName;
                data.OccupiedDtoProps[index].TablePropertyType = propertyType;
                data.OccupiedDtoProps[index].WithCast = allow;
            }

            return data;
        }

        /// <summary>
        /// Uses a Lambda Expression as filters for the First table of the
        /// current Join.Only the entries of the table that match these filters
        /// will be used
        /// </summary>
        /// <typeparam name="Dto">Class of the Output</typeparam>
        /// <typeparam name="Tsource">First Entity</typeparam>
        /// <typeparam name="TOther">Second Entity</typeparam>
        /// <param name="data">Previous Joins Data</param>
        /// <param name="predicate">First Table Joint Column</param>
        /// <returns>The Calculated Data of this Join</returns>
        public static JoinsData<Dto, Tsource, TOther> WhereFirst<Dto, Tsource, TOther>(this JoinsData<Dto, Tsource, TOther> data,
            Expression<Func<Tsource, bool>> predicate)
        {
            if (data.Ignore)
            {
                return data;
            }

            string? letter = data.TablesToLetters.Where(x => x.Table == typeof(Tsource)).First().Letter;
            ColumnsAndParameters colsAndParams = predicate.Body.SplitMembers<Tsource>(data.isMyShit, letter, data.DynamicParams, data.ParamsCount);
            data.DynamicParams = colsAndParams.Parameters;
            data.ParamsCount = colsAndParams.Count;

            if (data.WherePredicates == string.Empty)
            {
                data.WherePredicates = $" where {colsAndParams.Columns}";
            }
            else
            {
                data.WherePredicates += $" and {colsAndParams.Columns}";
            }

            return data;
        }

        /// <summary>
        /// Uses a Lambda Expression as filters for the Second table of the
        /// current Join.Only the entries of the table that match these filters
        /// will be used
        /// </summary>
        /// <typeparam name="Dto">Class of the Output</typeparam>
        /// <typeparam name="Tsource">First Entity</typeparam>
        /// <typeparam name="TOther">Second Entity</typeparam>
        /// <param name="data">Previous Joins Data</param>
        /// <param name="predicate">First Table Joint Column</param>
        /// <returns>The Calculated Data of this Join</returns>
        public static JoinsData<Dto, Tsource, TOther> WhereSecond<Dto, Tsource, TOther>(this JoinsData<Dto, Tsource, TOther> data,
            Expression<Func<TOther, bool>> predicate)
        {
            if (data.Ignore)
            {
                return data;
            }

            string? letter = data.TablesToLetters.Where(x => x.Table == typeof(TOther)).First().Letter;
            ColumnsAndParameters colsAndParams = predicate.Body.SplitMembers<TOther>(data.isMyShit, letter, data.DynamicParams, data.ParamsCount);
            data.DynamicParams = colsAndParams.Parameters;
            data.ParamsCount = colsAndParams.Count;

            if (data.WherePredicates == string.Empty)
            {
                data.WherePredicates = $" where {colsAndParams.Columns}";
            }
            else
            {
                data.WherePredicates += $" and {colsAndParams.Columns}";
            }

            return data;
        }

        /// <summary>
        /// Combines the Data of the Join and let's you start
        /// another Join or Execute the Joins Data or Store them as View
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <typeparam name="Tsource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <param name="data">Joins Data</param>
        /// <returns>The Calculated Data of all previous Joins</returns>
        public static JoinsData<Dto> Then<Dto, Tsource, TOther>(this JoinsData<Dto, Tsource, TOther> data)
        {
            return new JoinsData<Dto>
            {
                BaseTable = data.BaseTable,
                OccupiedDtoProps = data.OccupiedDtoProps,
                TablesToLetters = data.TablesToLetters,
                Joins = data.Joins,
                Letters = data.Letters,
                WherePredicates = data.WherePredicates,
                DynamicParams = data.DynamicParams,
                HelperIndex = data.HelperIndex,
                isMyShit = data.isMyShit,
                ParamsCount = data.ParamsCount,
                Ignore = false
            };
        }

        #endregion

        #region Execution Methods

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
        public static int StoreAsView<Dto>(this JoinsData<Dto> data) where Dto : IBHDtoIdentifier
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.FirstOrDefault(x => x.DtoType == typeof(Dto));

            if (existingJoin != null)
            {
                BlackHoleViews.Stored.Remove(existingJoin);
            }

            BlackHoleViews.Stored.Add(new JoinsData
            {
                DtoType = typeof(Dto),
                BaseTable = data.BaseTable,
                OccupiedDtoProps = data.OccupiedDtoProps,
                TablesToLetters = data.TablesToLetters,
                Joins = data.Joins,
                Letters = data.Letters,
                WherePredicates = data.WherePredicates,
                DynamicParams = data.DynamicParams,
                HelperIndex = data.HelperIndex,
                isMyShit = data.isMyShit,
                ParamsCount = data.ParamsCount,
                Ignore = false
            });

            return BlackHoleViews.Stored.Count;
        }

        internal static List<Dto> ExecuteQuery<Dto>(this JoinsData data) where Dto : IBHDtoIdentifier
        {
            if (data.DtoType == typeof(Dto))
            {
                IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
                data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
                TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                string schemaName = DatabaseStatics.DatabaseSchema.GetSchema();
                string commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
                return connection.Query<Dto>(commandText, data.DynamicParams);
            }
            return new List<Dto>();
        }

        internal static List<Dto> ExecuteQuery<Dto>(this JoinsData data, BHTransaction bHTransaction) where Dto : IBHDtoIdentifier
        {
            if (data.DtoType == typeof(Dto))
            {
                IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
                data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
                TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                string schemaName = DatabaseStatics.DatabaseSchema.GetSchema();
                string commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
                return connection.Query<Dto>(commandText, data.DynamicParams, bHTransaction.transaction);
            }
            return new List<Dto>();
        }

        internal static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinsData data) where Dto : IBHDtoIdentifier
        {
            if (data.DtoType == typeof(Dto))
            {
                IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
                data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
                TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                string schemaName = DatabaseStatics.DatabaseSchema.GetSchema();
                string commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
                return await connection.QueryAsync<Dto>(commandText, data.DynamicParams);
            }

            return new List<Dto>();
        }

        internal static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinsData data, BHTransaction bHTransaction) where Dto : IBHDtoIdentifier
        {
            if (data.DtoType == typeof(Dto))
            {
                IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
                data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
                TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                string schemaName = DatabaseStatics.DatabaseSchema.GetSchema();
                string commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
                return await connection.QueryAsync<Dto>(commandText, data.DynamicParams, bHTransaction.transaction);
            }
            return new List<Dto>();
        }

        /// <summary>
        /// Executes the Joins Data and returns the result
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="data">Joins Data</param>
        /// <returns>The Entries of the Joins mapped into DTO</returns>
        public static List<Dto> ExecuteQuery<Dto>(this JoinsData<Dto> data) where Dto : IBHDtoIdentifier
        {
            IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
            data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
            TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
            string schemaName = DatabaseStatics.DatabaseSchema.GetSchema();
            string commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
            return connection.Query<Dto>(commandText, data.DynamicParams);
        }

        /// <summary>
        /// <b>Transaction</b>. Executes the Joins Data and returns the result
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="data">Joins Data</param>
        /// <param name="bHTransaction">Transaction object</param>
        /// <returns>The Entries of the Joins mapped into DTO</returns>
        public static List<Dto> ExecuteQuery<Dto>(this JoinsData<Dto> data, BHTransaction bHTransaction) where Dto : IBHDtoIdentifier
        {
            IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
            data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
            TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
            string schemaName = DatabaseStatics.DatabaseSchema.GetSchema();
            string commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
            return connection.Query<Dto>(commandText, data.DynamicParams, bHTransaction.transaction);
        }

        /// <summary>
        /// <b>Asyncronous</b>. Executes the Joins Data and returns the result
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="data">Joins Data</param>
        /// <returns>The Entries of the Joins mapped into DTO</returns>
        public static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinsData<Dto> data) where Dto : IBHDtoIdentifier
        {
            IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
            data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
            TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
            string schemaName = DatabaseStatics.DatabaseSchema.GetSchema();
            string commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
            return await connection.QueryAsync<Dto>(commandText, data.DynamicParams);
        }

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Executes the Joins Data and returns the result
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="data">Joins Data</param>
        /// <param name="bHTransaction">Transaction object</param>
        /// <returns>The Entries of the Joins mapped into DTO</returns>
        public static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinsData<Dto> data, BHTransaction bHTransaction) where Dto : IBHDtoIdentifier
        {
            IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
            data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
            TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
            string schemaName = DatabaseStatics.DatabaseSchema.GetSchema();
            string commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
            return await connection.QueryAsync<Dto>(commandText, data.DynamicParams, bHTransaction.transaction);
        }

        internal static List<Dto> ExecuteQuery<Dto>(this JoinsData data, Action<BHOrderBy<Dto>> orderBy) where Dto : IBHDtoIdentifier
        {
            if (data.DtoType == typeof(Dto))
            {
                BHOrderBy<Dto> orderClass = new();
                orderBy.Invoke(orderClass);
                IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
                data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
                TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                string schemaName = DatabaseStatics.DatabaseSchema.GetSchema();
                string orderAndLimit = orderClass.OrderByToSqlJoins(data.isMyShit, data.OccupiedDtoProps);
                string commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates} {orderAndLimit}";
                return connection.Query<Dto>(commandText, data.DynamicParams);
            }
            return new List<Dto>();
        }

        internal static List<Dto> ExecuteQuery<Dto>(this JoinsData data, Action<BHOrderBy<Dto>> orderBy, BHTransaction bHTransaction) where Dto : IBHDtoIdentifier
        {
            if (data.DtoType == typeof(Dto))
            {
                BHOrderBy<Dto> orderClass = new();
                orderBy.Invoke(orderClass);
                IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
                data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
                TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                string schemaName = DatabaseStatics.DatabaseSchema.GetSchema();
                string orderAndLimit = orderClass.OrderByToSqlJoins(data.isMyShit, data.OccupiedDtoProps);
                string commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates} {orderAndLimit}";
                return connection.Query<Dto>(commandText, data.DynamicParams, bHTransaction.transaction);
            }
            return new List<Dto>();
        }

        internal static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinsData data, Action<BHOrderBy<Dto>> orderBy) where Dto : IBHDtoIdentifier
        {
            if (data.DtoType == typeof(Dto))
            {
                BHOrderBy<Dto> orderClass = new();
                orderBy.Invoke(orderClass);
                IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
                data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
                TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                string schemaName = DatabaseStatics.DatabaseSchema.GetSchema();
                string orderAndLimit = orderClass.OrderByToSqlJoins(data.isMyShit, data.OccupiedDtoProps);
                string commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates} {orderAndLimit}";
                return await connection.QueryAsync<Dto>(commandText, data.DynamicParams);
            }

            return new List<Dto>();
        }

        internal static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinsData data, Action<BHOrderBy<Dto>> orderBy, BHTransaction bHTransaction) where Dto : IBHDtoIdentifier
        {
            if (data.DtoType == typeof(Dto))
            {
                BHOrderBy<Dto> orderClass = new();
                orderBy.Invoke(orderClass);
                IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
                data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
                TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                string schemaName = DatabaseStatics.DatabaseSchema.GetSchema();
                string orderAndLimit = orderClass.OrderByToSqlJoins(data.isMyShit, data.OccupiedDtoProps);
                string commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates} {orderAndLimit}";
                return await connection.QueryAsync<Dto>(commandText, data.DynamicParams, bHTransaction.transaction);
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
        public static List<Dto> ExecuteQuery<Dto>(this JoinsData<Dto> data, Action<BHOrderBy<Dto>> orderBy) where Dto : IBHDtoIdentifier
        {
            BHOrderBy<Dto> orderClass = new();
            orderBy.Invoke(orderClass);
            IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
            data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
            TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
            string schemaName = DatabaseStatics.DatabaseSchema.GetSchema();
            string orderAndLimit = orderClass.OrderByToSqlJoins(data.isMyShit, data.OccupiedDtoProps);
            string commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates} {orderAndLimit}";
            return connection.Query<Dto>(commandText, data.DynamicParams);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="data"></param>
        /// <param name="orderBy"></param>
        /// <param name="bHTransaction"></param>
        /// <returns></returns>
        public static List<Dto> ExecuteQuery<Dto>(this JoinsData<Dto> data, Action<BHOrderBy<Dto>> orderBy, BHTransaction bHTransaction) where Dto : IBHDtoIdentifier
        {
            BHOrderBy<Dto> orderClass = new();
            orderBy.Invoke(orderClass);
            IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
            data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
            TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
            string schemaName = DatabaseStatics.DatabaseSchema.GetSchema();
            string orderAndLimit = orderClass.OrderByToSqlJoins(data.isMyShit, data.OccupiedDtoProps);
            string commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates} {orderAndLimit}";
            return connection.Query<Dto>(commandText, data.DynamicParams, bHTransaction.transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="data"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinsData<Dto> data, Action<BHOrderBy<Dto>> orderBy) where Dto : IBHDtoIdentifier
        {
            BHOrderBy<Dto> orderClass = new();
            orderBy.Invoke(orderClass);
            IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
            data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
            TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
            string schemaName = DatabaseStatics.DatabaseSchema.GetSchema();
            string orderAndLimit = orderClass.OrderByToSqlJoins(data.isMyShit, data.OccupiedDtoProps);
            string commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates} {orderAndLimit}";
            return await connection.QueryAsync<Dto>(commandText, data.DynamicParams);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="data"></param>
        /// <param name="orderBy"></param>
        /// <param name="bHTransaction"></param>
        /// <returns></returns>
        public static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinsData<Dto> data, Action<BHOrderBy<Dto>> orderBy, BHTransaction bHTransaction) where Dto : IBHDtoIdentifier
        {
            BHOrderBy<Dto> orderClass = new();
            orderBy.Invoke(orderClass);
            IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
            data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
            TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
            string schemaName = DatabaseStatics.DatabaseSchema.GetSchema();
            string orderAndLimit = orderClass.OrderByToSqlJoins(data.isMyShit, data.OccupiedDtoProps);
            string commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates} {orderAndLimit}";
            return await connection.QueryAsync<Dto>(commandText, data.DynamicParams, bHTransaction.transaction);
        }

        #endregion

        #region Internal Functionality

        private static string OrderByToSqlJoins<T>(this BHOrderBy<T> orderByConfig, bool isMyShit, List<PropertyOccupation> bindedProperties)
        {
            if (orderByConfig.orderBy.LockedByError)
            {
                return string.Empty;
            }

            StringBuilder orderby = new();
            string limiter = string.Empty;
            int counter = 0;

            foreach (OrderByPair pair in orderByConfig.orderBy.OrderProperties)
            {
                if(bindedProperties.FirstOrDefault(x => x.PropName == pair.PropertyName) is PropertyOccupation occupation)
                {
                    counter++;
                    orderby.Append($", {occupation.TableLetter}.{occupation.TableProperty.SkipNameQuotes(isMyShit)} {pair.Oriantation}");
                }
            }

            if (orderByConfig.orderBy.TakeSpecificRange)
            {
                limiter = orderByConfig.orderBy.RowsLimiterJoins();
            }

            if(counter > 0)
            {
                return $"order by{orderby.ToString().Remove(0, 1)}{limiter}";
            }

            return string.Empty;
        }

        private static string RowsLimiterJoins<T>(this BlackHoleOrderBy<T> limiter)
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => $" OFFSET {limiter.FromRow} ROWS FETCH NEXT {limiter.ToRow} ROWS ONLY",
                BlackHoleSqlTypes.MySql => $" LIMIT {limiter.ToRow} OFFSET {limiter.FromRow}",
                BlackHoleSqlTypes.Postgres => $" LIMIT {limiter.ToRow} OFFSET {limiter.FromRow}",
                BlackHoleSqlTypes.Oracle => $" OFFSET {limiter.FromRow} ROWS FETCH NEXT {limiter.ToRow} ROWS ONLY",
                _ => $" LIMIT {limiter.ToRow} OFFSET {limiter.FromRow}"
            };
        }

        private static string RejectInactiveEntities(this List<TableLetters> involvedTables, string whereCommand, bool isMyShit)
        {
            string command = string.Empty;
            string inactiveColumn = "Inactive";
            string anD = "and";

            if (whereCommand == string.Empty)
            {
                anD = "where";
            }

            foreach (TableLetters table in involvedTables)
            {
                if (!table.IsOpenEntity)
                {
                    if (command != string.Empty)
                    {
                        anD = "and";
                    }

                    command += $" {anD} {table.Letter}.{inactiveColumn.SkipNameQuotes(isMyShit)} = 0 ";
                }
            }

            return whereCommand + command;
        }

        private static string BuildCommand(this List<PropertyOccupation> usedProperties, bool isMyShit)
        {
            string sqlCommand = "select ";

            foreach (PropertyOccupation prop in usedProperties.Where(x => x.Occupied))
            {
                sqlCommand += prop.WithCast switch
                {
                    1 => $" {prop.TableLetter}.{prop.TableProperty.SkipNameQuotes(isMyShit)} as {prop.PropName.SkipNameQuotes(isMyShit)},",
                    2 => $" cast({prop.TableLetter}.{prop.TableProperty.SkipNameQuotes(isMyShit)} as {prop.PropType.SqlTypeFromType()}) as {prop.PropName.SkipNameQuotes(isMyShit)},",
                    _ => $" {prop.TableLetter}.{prop.PropName.SkipNameQuotes(isMyShit)},",
                };
            }

            return sqlCommand[..^1];
        }

        private static string SqlTypeFromType(this Type? type)
        {
            string[] SqlDatatypes = DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => new[] { "nvarchar(4000)", "int", "bigint", "decimal", "float" },
                BlackHoleSqlTypes.MySql => new[] { "char(2000)", "int", "bigint", "dec", "double" },
                BlackHoleSqlTypes.Postgres => new[] { "varchar(4000)", "integer", "bigint", "numeric(10,5)", "numeric" },
                BlackHoleSqlTypes.SqlLite => new[] { "varchar(4000)", "integer", "bigint", "decimal(10,5)", "numeric" },
                _ => new[] { "varchar2(4000)", "Number(8,0)", "Number(16,0)", "Number(19,0)", "Number" },
            };

            string result = SqlDatatypes[0];
            string? TypeName = type?.Name;

            if(type != null && !string.IsNullOrEmpty(TypeName))
            {
                if (TypeName.Contains("Nullable"))
                {
                    if (type?.GenericTypeArguments != null && type?.GenericTypeArguments.Length > 0)
                    {
                        TypeName = type.GenericTypeArguments[0].Name;
                    }
                }

                result = TypeName switch
                {
                    "Int32" => SqlDatatypes[1],
                    "Int64" => SqlDatatypes[2],
                    "Decimal" => SqlDatatypes[3],
                    "Double" => SqlDatatypes[4],
                    _ => SqlDatatypes[0],
                };
            }

            return result;
        }

        private static IExecutionProvider GetConnectionExtension(this BlackHoleSqlTypes connectionType)
        {
            return connectionType switch
            {
                BlackHoleSqlTypes.SqlServer => new SqlServerExecutionProvider(DatabaseStatics.ConnectionString, DatabaseStatics.IsQuotedDatabase),
                BlackHoleSqlTypes.MySql => new MySqlExecutionProvider(DatabaseStatics.ConnectionString),
                BlackHoleSqlTypes.Postgres => new PostgresExecutionProvider(DatabaseStatics.ConnectionString),
                BlackHoleSqlTypes.SqlLite => new SqLiteExecutionProvider(DatabaseStatics.ConnectionString, DatabaseStatics.IsQuotedDatabase),
                _ => new OracleExecutionProvider(DatabaseStatics.ConnectionString),
            };
        }

        private static JoinsData<Dto, Tsource, TOther> CreateJoin<Dto, Tsource, TOther>(this JoinsData<Dto, Tsource, TOther> data, LambdaExpression key, LambdaExpression otherKey, string joinType)
        {
            string? parameter = string.Empty;

            TableLetters? firstType = data.TablesToLetters.Where(x => x.Table == typeof(Tsource)).FirstOrDefault();

            if (firstType == null)
            {
                data.Ignore = true;
            }
            else
            {
                parameter = firstType.Letter;
            }

            if (data.Ignore)
            {
                return data;
            }

            MemberExpression? member = key.Body as MemberExpression;
            string? propName = member?.Member.Name;
            string? parameterOther = otherKey.Parameters[0].Name;
            MemberExpression? memberOther = otherKey.Body as MemberExpression;
            string? propNameOther = memberOther?.Member.Name;

            TableLetters? secondTable = data.TablesToLetters.Where(x => x.Table == typeof(TOther)).FirstOrDefault();

            if (secondTable == null)
            {
                bool letterExists = data.Letters.Contains(parameterOther);

                if (letterExists)
                {
                    parameterOther += data.HelperIndex.ToString();
                    data.Letters.Add(parameterOther);
                    data.HelperIndex++;
                }
                bool isOpen = typeof(TOther).BaseType?.GetGenericTypeDefinition() == typeof(BHOpenEntity<>);
                data.TablesToLetters.Add(new TableLetters { Table = typeof(TOther), Letter = parameterOther, IsOpenEntity = isOpen });
            }
            else
            {
                parameterOther = secondTable.Letter;
            }

            string schemaName = DatabaseStatics.DatabaseSchema.GetSchema();

            data.Joins += $" {joinType} join {schemaName}{typeof(TOther).Name.SkipNameQuotes(data.isMyShit)} {parameterOther} on {parameterOther}.{propNameOther.SkipNameQuotes(data.isMyShit)} = {parameter}.{propName.SkipNameQuotes(data.isMyShit)}";
            data.OccupiedDtoProps = data.OccupiedDtoProps.BindPropertiesToDtoExtension(typeof(TOther), parameterOther);
            return data;
        }

        private static string GetSchema(this string StaticSchema)
        {
            string schemaName = string.Empty;

            if (StaticSchema != string.Empty)
            {
                schemaName = $"{StaticSchema}.";
            }

            return schemaName;
        }

        private static List<PropertyOccupation> BindPropertiesToDtoExtension(this List<PropertyOccupation> props, Type secondTable, string? paramB)
        {
            List<string> OtherPropNames = new();

            foreach (PropertyInfo otherProp in secondTable.GetProperties())
            {
                OtherPropNames.Add(otherProp.Name);
            }

            foreach (PropertyOccupation property in props)
            {
                if (OtherPropNames.Contains(property.PropName) && !property.Occupied)
                {
                    Type? TOtherPropType = secondTable.GetProperty(property.PropName)?.PropertyType;

                    if (TOtherPropType == property.PropType)
                    {
                        property.Occupied = true;
                        property.TableProperty = TOtherPropType?.Name;
                        property.TablePropertyType = TOtherPropType;
                        property.TableLetter = paramB;
                    }
                }
            }

            return props;
        }

        private static int AllowCast(this Type firstType, Type secondType)
        {
            int allow = 2;
            string typeTotype = firstType.Name + secondType.Name;

            if(firstType == typeof(Guid))
            {
                BlackHoleSqlTypes sqlType = DatabaseStatics.DatabaseType;
                if(sqlType != BlackHoleSqlTypes.Postgres && sqlType != BlackHoleSqlTypes.SqlServer)
                {
                    allow = 1;
                }
            }

            if (firstType.Name != secondType.Name)
            {
                switch (typeTotype)
                {
                    case "Int16Int32":
                        break;
                    case "Int16String":
                        break;
                    case "Int16Int64":
                        break;
                    case "Int32String":
                        break;
                    case "Int32Int64":
                        break;
                    case "Int64String":
                        break;
                    case "DecimalString":
                        break;
                    case "DecimalDouble":
                        break;
                    case "SingleString":
                        break;
                    case "SingleDouble":
                        break;
                    case "SingleDecimal":
                        break;
                    case "DoubleString":
                        break;
                    case "Int32Decimal":
                        break;
                    case "GuidString":
                        break;
                    case "Int32Double":
                        break;
                    case "BooleanString":
                        break;
                    case "BooleanInt32":
                        break;
                    case "Byte[]String":
                        break;
                    case "DateTimeString":
                        break;
                    default:
                        allow = 0;
                        break;
                }
            }
            else
            {
                allow = 1;
            }

            return allow;
        }
        #endregion
    }
}
