using BlackHole.Entities;
using BlackHole.Enums;
using BlackHole.Statics;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using BlackHole.Logger;
using BlackHole.CoreSupport;
using BlackHole.ExecutionProviders;

namespace BlackHole.Core
{
    /// <summary>
    /// Queries for the Table Joins Feature
    /// </summary>
    public static class BHExtensions
    {
        /// <summary>
        /// Performs a Right Join between the First and the Second specified Entities. 
        /// !!Important!! => For safety reasons, The first Entity must have been used 
        /// at least once in the previous Joins, otherwise this Join and its settings will be ignored on the 
        /// Execution and you might get some null values on the exported DTOs.
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
            where TOther : BlackHoleEntity where Tsource : BlackHoleEntity where Tkey : IComparable
        {
            JoinsData<Dto, Tsource, TOther> newJoin = new JoinsData<Dto, Tsource, TOther>
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
        /// !!Important!! => For safety reasons, The first Entity must have been used 
        /// at least once in the previous Joins, otherwise this Join and its settings will be ignored on the 
        /// Execution and you might get some null values on the exported DTOs.
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
            where TOther : BlackHoleEntity where Tsource : BlackHoleEntity where Tkey : IComparable
        {
            JoinsData<Dto, Tsource, TOther> newJoin = new JoinsData<Dto, Tsource, TOther>
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
        /// !!Important!! => For safety reasons, The first Entity must have been used 
        /// at least once in the previous Joins, otherwise this Join and its settings will be ignored on the 
        /// Execution and you might get some null values on the exported DTOs.
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
            where TOther : BlackHoleEntity where Tsource : BlackHoleEntity where Tkey : IComparable
        {
            JoinsData<Dto, Tsource, TOther> newJoin = new JoinsData<Dto, Tsource, TOther>
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

            return newJoin.CreateJoin(key, otherkey, "outer");
        }

        /// <summary>
        /// Performs an Inner Join between the First and the Second specified Entities. 
        /// !!Important!! => For safety reasons, The first Entity must have been used 
        /// at least once in the previous Joins, otherwise this Join and its settings will be ignored on the 
        /// Execution and you might get some null values on the exported DTOs.
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
            where TOther : BlackHoleEntity where Tsource : BlackHoleEntity where Tkey : IComparable where Dto : BlackHoleDto
        {
            JoinsData<Dto, Tsource, TOther> newJoin = new JoinsData<Dto, Tsource, TOther>
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

            data.Joins += $" and {secondLetter}.{propNameOther.SqlPropertyName(data.isMyShit)} = {firstLetter}.{propName.SqlPropertyName(data.isMyShit)}";
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

            data.Joins += $" or {secondLetter}.{propNameOther.SqlPropertyName(data.isMyShit)} = {firstLetter}.{propName.SqlPropertyName(data.isMyShit)}";
            return data;
        }

        /// <summary>
        /// Casts a column of the second Entity as a column of the 
        /// output's DTO. !!Important!! => There are some restrictions
        /// to the types of the properties that can be casted. Read the Documentation
        /// for details. If a data type is not allowed to be converted to another type,
        /// then the cast will be ignored in the Execution and the DTO column will be null. 
        /// !!Tip!! => Cast has priority over normal mapping, For example the Column Id of the DTO is 
        /// by default mapped to the First Entity of all Joins. If you want to map a different
        /// Entity's Id into that column, use a Cast.
        /// </summary>
        /// <typeparam name="Dto">Class of the Output</typeparam>
        /// <typeparam name="Tsource">First Entity</typeparam>
        /// <typeparam name="TOther">Second Entity</typeparam>
        /// <typeparam name="Tkey">Type of their Joint Column</typeparam>
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
        /// Casts a column of the second Entity as a column of the 
        /// output's DTO. !!Important!! => There are some restrictions
        /// to the types of the properties that can be casted. Read the Documentation
        /// for details. If a data type is not allowed to be converted to another type,
        /// then the cast will be ignored in the Execution and the DTO column will be null. 
        /// !!Tip!! => Cast has priority over normal mapping, For example the Column Id of the DTO is 
        /// by default mapped to the First Entity of all Joins. If you want to map a different
        /// Entity's Id into that column, use a Cast.
        /// </summary>
        /// <typeparam name="Dto">Class of the Output</typeparam>
        /// <typeparam name="Tsource">First Entity</typeparam>
        /// <typeparam name="TOther">Second Entity</typeparam>
        /// <typeparam name="Tkey">Type of their Joint Column</typeparam>
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
        /// <typeparam name="Tkey">Type of their Joint Column</typeparam>
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
            ColumnsAndParameters colsAndParams = predicate.Body.SplitMembersExtension(data.isMyShit, letter, data.DynamicParams, data.ParamsCount);
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
        /// <typeparam name="Tkey">Type of their Joint Column</typeparam>
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
            ColumnsAndParameters colsAndParams = predicate.Body.SplitMembersExtension(data.isMyShit, letter, data.DynamicParams, data.ParamsCount);
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
        /// <param name="data"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Stores the Joins Data with the DTO Class as Identifier
        /// and then you can execute them as many times as you want
        /// using the 'IBlackHoleViewStorage' Interface.
        /// With this method, the program doesn't have to calculate the
        /// Joins Data multiple times and it executes the Joins faster.
        /// This method is recommended if the parameters in the current
        /// Joins Data are not depending on the user's inputs.
        /// Run your Joins Once in the StartUp of your program and store them
        /// as Views.
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="data"></param>
        /// <returns>The index of this Joins Data in the Stored Views List</returns>
        public static int StoreAsView<Dto>(this JoinsData<Dto> data) where Dto : BlackHoleDto
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();

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

        /// <summary>
        /// Executes the Joins Data from The View Storage
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="data">Joins Data</param>
        /// <returns>The Entries of the Joins mapped into DTO</returns>
        internal static List<Dto> ExecuteQuery<Dto>(this JoinsData data) where Dto : BlackHoleDto
        {
            if (data.DtoType == typeof(Dto))
            {
                string commandText = "";
                try
                {
                    IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
                    data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
                    TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                    commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {tL?.Table?.Name.SqlPropertyName(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
                    return connection.Query<Dto>(commandText, data.DynamicParams.Parameters);
                }
                catch (Exception ex)
                {
                    LoggerService logs = new LoggerService();
                    logs.CreateErrorLogs($"ViewExecution_{typeof(Dto).Name}", commandText, ex.Message, ex.ToString());
                }
            }

            return new List<Dto>();
        }

        /// <summary>
        /// Transaction. Executes the Joins Data from The View Storage
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="data">Joins Data</param>
        /// <returns>The Entries of the Joins mapped into DTO</returns>
        internal static List<Dto> ExecuteQuery<Dto>(this JoinsData data, BHTransaction bHTransaction) where Dto : BlackHoleDto
        {
            if (data.DtoType == typeof(Dto))
            {
                string commandText = "";
                try
                {
                    IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
                    data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
                    TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                    commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {tL?.Table?.Name.SqlPropertyName(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
                    return connection.Query<Dto>(commandText, data.DynamicParams.Parameters, bHTransaction.transaction);

                }
                catch (Exception ex)
                {
                    LoggerService logs = new LoggerService();
                    logs.CreateErrorLogs($"ViewExecution_{typeof(Dto).Name}", commandText, ex.Message, ex.ToString());
                }
            }
            return new List<Dto>();
        }

        /// <summary>
        /// Executes the Joins Data and returns the result
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="data">Joins Data</param>
        /// <returns>The Entries of the Joins mapped into DTO</returns>
        public static List<Dto> ExecuteQuery<Dto>(this JoinsData<Dto> data) where Dto : BlackHoleDto
        {
            string commandText = "";
            try
            {
                IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
                data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
                TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {tL?.Table?.Name.SqlPropertyName(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
                return connection.Query<Dto>(commandText, data.DynamicParams.Parameters);
            }
            catch (Exception ex)
            {
                LoggerService logs = new LoggerService();
                logs.CreateErrorLogs($"JoinExecution_{typeof(Dto).Name}", commandText, ex.Message, ex.ToString());
                return new List<Dto>();
            }
        }

        /// <summary>
        /// Transaction. Executes the Joins Data and returns the result
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="data">Joins Data</param>
        /// <returns>The Entries of the Joins mapped into DTO</returns>
        public static List<Dto> ExecuteQuery<Dto>(this JoinsData<Dto> data, BHTransaction bHTransaction) where Dto : BlackHoleDto
        {
            string commandText = "";
            try
            {
                IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
                data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
                TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {tL?.Table?.Name.SqlPropertyName(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
                return connection.Query<Dto>(commandText, data.DynamicParams.Parameters, bHTransaction.transaction);
            }
            catch (Exception ex)
            {
                LoggerService logs = new LoggerService();
                logs.CreateErrorLogs($"JoinExecution_{typeof(Dto).Name}", commandText, ex.Message, ex.ToString());
                return new List<Dto>();
            }
        }

        /// <summary>
        /// Asyncronous. Executes the Joins Data and returns the result
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="data">Joins Data</param>
        /// <returns>The Entries of the Joins mapped into DTO</returns>
        public static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinsData<Dto> data) where Dto : BlackHoleDto
        {
            string commandText = "";
            try
            {
                IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
                data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
                TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {tL?.Table?.Name.SqlPropertyName(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
                return await connection.QueryAsync<Dto>(commandText, data.DynamicParams.Parameters);
            }
            catch (Exception ex)
            {
                LoggerService logs = new LoggerService();
                logs.CreateErrorLogs($"JoinExecution_{typeof(Dto).Name}", commandText, ex.Message, ex.ToString());
                return new List<Dto>();
            }
        }

        /// <summary>
        /// Transaction.Asyncronous. Executes the Joins Data and returns the result
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="data">Joins Data</param>
        /// <returns>The Entries of the Joins mapped into DTO</returns>
        public static async Task<List<Dto>> ExecuteQueryAsync<Dto>(this JoinsData<Dto> data, BHTransaction bHTransaction) where Dto : BlackHoleDto
        {
            string commandText = "";
            try
            {
                IExecutionProvider connection = DatabaseStatics.DatabaseType.GetConnectionExtension();
                data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
                TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                commandText = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {tL?.Table?.Name.SqlPropertyName(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
                return await connection.QueryAsync<Dto>(commandText, data.DynamicParams.Parameters, bHTransaction.transaction);
            }
            catch (Exception ex)
            {
                LoggerService logs = new LoggerService();
                logs.CreateErrorLogs($"JoinExecution_{typeof(Dto).Name}", commandText, ex.Message, ex.ToString());
                return new List<Dto>();
            }
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
                if (command != string.Empty)
                {
                    anD = "and";
                }

                command += $" {anD} {table.Letter}.{inactiveColumn.SqlPropertyName(isMyShit)} = 0 ";
            }

            return whereCommand + command;
        }

        private static string BuildCommand(this List<PropertyOccupation> usedProperties, bool isMyShit)
        {
            string sqlCommand = "select ";

            foreach (PropertyOccupation prop in usedProperties.Where(x => x.Occupied))
            {
                switch (prop.WithCast)
                {
                    case 1:
                        sqlCommand += $" {prop.TableLetter}.{prop.TableProperty.SqlPropertyName(isMyShit)} as {prop.PropName.SqlPropertyName(isMyShit)},";
                        break;
                    case 2:
                        sqlCommand += $" cast({prop.TableLetter}.{prop.TableProperty.SqlPropertyName(isMyShit)} as {prop.PropType.SqlTypeFromType()}) as {prop.PropName.SqlPropertyName(isMyShit)},";
                        break;
                    default:
                        sqlCommand += $" {prop.TableLetter}.{prop.PropName.SqlPropertyName(isMyShit)},";
                        break;
                }
            }

            return sqlCommand.Substring(0, sqlCommand.Length - 1);
        }

        private static string SqlTypeFromType(this Type? type)
        {
            string[] SqlDatatypes = new string[5];

            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    SqlDatatypes = new[] { "nvarchar(500)", "int", "bigint", "decimal", "float" };
                    break;
                case BlackHoleSqlTypes.MySql:
                    SqlDatatypes = new[] { "char", "int", "bigint", "dec", "double" };
                    break;
                case BlackHoleSqlTypes.Postgres:
                    SqlDatatypes = new[] { "varchar(500)", "integer", "bigint", "numeric(10,5)", "numeric" };
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    SqlDatatypes = new[] { "varchar(500)", "integer", "bigint", "decimal(10,5)", "numeric" };
                    break;
                case BlackHoleSqlTypes.Oracle:
                    SqlDatatypes = new[] { "varchar2(500)", "Number(8,0)", "Number(16,0)", "Number(19,0)", "Number" };
                    break;
            }

            string result = string.Empty;

            switch (type?.Name)
            {
                case "Int32":
                    result = SqlDatatypes[1];
                    break;
                case "Int64":
                    result = SqlDatatypes[2];
                    break;
                case "Decimal":
                    result = SqlDatatypes[3];
                    break;
                case "Double":
                    result = SqlDatatypes[4];
                    break;
                default:
                    result = SqlDatatypes[0];
                    break;
            }

            return result;
        }

        private static IExecutionProvider GetConnectionExtension(this BlackHoleSqlTypes connectionType)
        {
            IExecutionProvider provider;
            switch (connectionType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    provider = new SqlServerExecutionProvider(DatabaseStatics.ConnectionString);
                    break;
                case BlackHoleSqlTypes.MySql:
                    provider = new MySqlExecutionProvider(DatabaseStatics.ConnectionString);
                    break;
                case BlackHoleSqlTypes.Postgres:
                    provider = new PostgresExecutionProvider(DatabaseStatics.ConnectionString);
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    provider = new SqLiteExecutionProvider(DatabaseStatics.ConnectionString);
                    break;
                default:
                    provider = new OracleExecutionProvider(DatabaseStatics.ConnectionString);
                    break;
            }
            return provider;
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

                data.TablesToLetters.Add(new TableLetters { Table = typeof(TOther), Letter = parameterOther });
            }
            else
            {
                parameterOther = secondTable.Letter;
            }

            data.Joins += $" {joinType} join {typeof(TOther).Name.SqlPropertyName(data.isMyShit)} {parameterOther} on {parameterOther}.{propNameOther.SqlPropertyName(data.isMyShit)} = {parameter}.{propName.SqlPropertyName(data.isMyShit)}";
            data.OccupiedDtoProps = data.OccupiedDtoProps.BindPropertiesToDtoExtension(typeof(Tsource), typeof(TOther), parameter, parameterOther);
            return data;
        }

        private static List<PropertyOccupation> BindPropertiesToDtoExtension(this List<PropertyOccupation> props, Type firstTable, Type secondTable, string? paramA, string? paramB)
        {
            List<string> PropNames = new List<string>();
            List<string> OtherPropNames = new List<string>();

            foreach (PropertyInfo prop in firstTable.GetProperties())
            {
                PropNames.Add(prop.Name);
            }

            foreach (PropertyInfo otherProp in secondTable.GetProperties())
            {
                OtherPropNames.Add(otherProp.Name);
            }

            foreach (PropertyOccupation property in props)
            {
                if (PropNames.Contains(property.PropName) && !property.Occupied)
                {
                    Type? TpropType = firstTable.GetProperty(property.PropName)?.PropertyType;

                    if (TpropType == property.PropType)
                    {
                        property.Occupied = true;
                        property.TableProperty = TpropType?.Name;
                        property.TablePropertyType = TpropType;
                        property.TableLetter = paramA;
                    }
                }

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

        private static string SqlPropertyName(this string? propName, bool isMyShit)
        {
            string result = propName ?? string.Empty;

            if (!isMyShit)
            {
                result = $@"""{propName}""";
            }

            return result;
        }

        private static int AllowCast(this Type firstType, Type secondType)
        {
            int allow = 2;
            string typeTotype = firstType.Name + secondType.Name;

            if (firstType.Name != secondType.Name)
            {
                switch (typeTotype)
                {
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

        private static ColumnsAndParameters SplitMembersExtension(this Expression expression, bool isMyShit, string? letter, BHParameters parameters, int index)
        {
            List<ExpressionsData> expressionTree = new List<ExpressionsData>();

            BinaryExpression? operation = expression as BinaryExpression;
            BinaryExpression? currentOperation = operation;
            MemberExpression? leftMember = currentOperation?.Left as MemberExpression;
            MemberExpression? rightMember = currentOperation?.Right as MemberExpression;

            int currentIndx = 0;
            bool startTranslate = false;

            if (operation != null)
            {
                startTranslate = true;

                expressionTree.Add(new ExpressionsData()
                {
                    operation = operation,
                    leftMember = operation?.Left as MemberExpression,
                    rightMember = operation?.Right as MemberExpression,
                    expressionType = operation != null ? operation.NodeType : ExpressionType.Default,
                    rightChecked = false,
                    leftChecked = false,
                    memberValue = null
                });
            }

            while (startTranslate)
            {
                if (expressionTree[currentIndx].operation != null)
                {
                    if (expressionTree[currentIndx].expressionType == ExpressionType.AndAlso || expressionTree[currentIndx].expressionType == ExpressionType.OrElse)
                    {
                        bool addTotree = false;

                        if (!expressionTree[currentIndx].leftChecked)
                        {
                            currentOperation = expressionTree[currentIndx].operation?.Left as BinaryExpression;
                            expressionTree[currentIndx].leftChecked = true;
                            addTotree = true;
                        }
                        else if (!expressionTree[currentIndx].rightChecked && expressionTree[currentIndx].leftChecked)
                        {
                            currentOperation = expressionTree[currentIndx].operation?.Right as BinaryExpression;
                            expressionTree[currentIndx].rightChecked = true;
                            addTotree = true;
                        }
                        else
                        {
                            currentIndx -= 1;
                        }

                        if (addTotree)
                        {
                            expressionTree.Add(new ExpressionsData()
                            {
                                operation = currentOperation,
                                leftMember = currentOperation?.Left as MemberExpression,
                                rightMember = currentOperation?.Right as MemberExpression,
                                expressionType = currentOperation != null ? currentOperation.NodeType : ExpressionType.Default,
                                rightChecked = false,
                                leftChecked = false,
                                memberValue = null,
                                parentIndex = currentIndx
                            });

                            currentIndx = expressionTree.Count - 1;
                        }
                    }
                    else
                    {
                        if (!expressionTree[currentIndx].leftChecked || !expressionTree[currentIndx].rightChecked)
                        {
                            rightMember = currentOperation?.Right as MemberExpression;
                            ConstantExpression? rightConstant = currentOperation?.Right as ConstantExpression;
                            BinaryExpression? rightBinary = currentOperation?.Right as BinaryExpression;

                            expressionTree[currentIndx].leftChecked = true;
                            expressionTree[currentIndx].rightChecked = true;

                            object? value = null;

                            if (rightMember != null)
                            {
                                value = Expression.Lambda(rightMember).Compile().DynamicInvoke();
                            }

                            if (rightConstant != null)
                            {
                                value = rightConstant?.Value;
                            }

                            if (rightBinary != null)
                            {
                                value = Expression.Lambda(rightBinary).Compile().DynamicInvoke();
                            }

                            expressionTree[currentIndx].memberValue = value;
                        }

                        currentIndx -= 1;
                    }
                }

                if (currentIndx < 0)
                {
                    startTranslate = false;
                }
            }

            return expressionTree.ExpressionTreeToSqlExtension(isMyShit, letter, parameters, index);
        }

        private static ColumnsAndParameters ExpressionTreeToSqlExtension(this List<ExpressionsData> data, bool isMyShit, string? letter, BHParameters parameters, int index)
        {
            string result = "";
            List<ExpressionsData> children = data.Where(x => x.memberValue != null).ToList();
            string[] translations = new string[children.Count];

            foreach (ExpressionsData child in children)
            {
                ExpressionsData parent = data[child.parentIndex];
                if (parent.leftChecked)
                {
                    ColumnAndParameter childParams = child.TranslateExpressionExtension(index, isMyShit, letter);

                    if (childParams.ParamName != string.Empty)
                    {
                        parameters.Add(childParams.ParamName, childParams.Value);
                    }

                    parent.sqlCommand = $"{childParams.Column}";
                    parent.leftChecked = false;
                    index++;
                }
                else
                {
                    ColumnAndParameter parentCols = parent.TranslateExpressionExtension(index, isMyShit, letter);

                    if (parentCols.ParamName != string.Empty)
                    {
                        parameters.Add(parentCols.ParamName, parentCols.Value);
                    }

                    index++;

                    ColumnAndParameter childCols = child.TranslateExpressionExtension(index, isMyShit, letter);

                    if (childCols.ParamName != string.Empty)
                    {
                        parameters.Add(childCols.ParamName, childCols.Value);
                    }

                    parent.sqlCommand = $"({parent.sqlCommand} {parentCols.Column} {childCols.Column})";

                    index++;
                }
            }

            List<ExpressionsData> parents = data.Where(x => x.memberValue == null).ToList();

            if (parents.Count > 1)
            {
                parents.RemoveAt(0);
                int parentsCount = parents.Count;

                for (int i = 0; i < parentsCount; i++)
                {
                    ExpressionsData parent = data[parents[parentsCount - 1 - i].parentIndex];

                    if (parent.leftChecked)
                    {
                        parent.sqlCommand = parents[parentsCount - 1 - i].sqlCommand;
                        parent.leftChecked = false;
                    }
                    else
                    {
                        ColumnAndParameter parentParams = parent.TranslateExpressionExtension(index, isMyShit, letter);
                        if (parentParams.ParamName != string.Empty)
                        {
                            parameters.Add(parentParams.ParamName, parentParams.Value);
                        }
                        parent.sqlCommand = $"({parent.sqlCommand} {parentParams.Column} {parents[parentsCount - 1 - i].sqlCommand})";
                        index++;
                    }
                }
            }

            result = data[0].sqlCommand;

            return new ColumnsAndParameters { Columns = result, Parameters = parameters, Count = index };
        }

        private static ColumnAndParameter TranslateExpressionExtension(this ExpressionsData expression, int index, bool isMyShit, string? letter)
        {
            string? column = string.Empty;
            string? parameter = string.Empty;
            object? value = new object();
            string[]? variable = new string[2];

            switch (expression.expressionType)
            {
                case ExpressionType.AndAlso:
                    column = " and ";
                    break;
                case ExpressionType.OrElse:
                    column = " or ";
                    break;
                case ExpressionType.Equal:
                    value = expression?.memberValue;
                    variable = expression?.leftMember?.ToString().Split(".");
                    column = $"{letter}.{variable?[1].SqlPropertyName(isMyShit)} = @{variable?[1]}{index}";
                    if (value == null)
                    {
                        column = $"{letter}.{variable?[1].SqlPropertyName(isMyShit)} is @{variable?[1]}{index}";
                    }
                    parameter = $"{variable?[1]}{index}";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    variable = expression?.leftMember?.ToString().Split(".");
                    column = $"{letter}.{variable?[1].SqlPropertyName(isMyShit)} >= @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.LessThanOrEqual:
                    variable = expression?.leftMember?.ToString().Split(".");
                    column = $"{letter}.{variable?[1].SqlPropertyName(isMyShit)} <= @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.LessThan:
                    variable = expression?.leftMember?.ToString().Split(".");
                    column = $"{letter}.{variable?[1].SqlPropertyName(isMyShit)} < @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.GreaterThan:
                    variable = expression?.leftMember?.ToString().Split(".");
                    column = $"{letter}.{variable?[1].SqlPropertyName(isMyShit)} > @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.NotEqual:
                    value = expression?.memberValue;
                    variable = expression?.leftMember?.ToString().Split(".");
                    column = $"{letter}.{variable?[1].SqlPropertyName(isMyShit)} != @{variable?[1]}{index}";
                    if (value == null)
                    {
                        column = $"{letter}.{variable?[1].SqlPropertyName(isMyShit)} is not @{variable?[1]}{index}";
                    }
                    parameter = $"{variable?[1]}{index}";
                    break;
            }

            return new ColumnAndParameter { Column = column, ParamName = parameter, Value = value };
        }
    }
}
