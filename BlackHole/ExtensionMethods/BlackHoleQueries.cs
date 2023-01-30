using BlackHole.Entities;
using BlackHole.Enums;
using BlackHole.Statics;
using Dapper;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection;

namespace BlackHole.ExtensionMethods
{
    public static class BlackHoleQueries
    {
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

            return newJoin.CreateJoin(key,otherkey,"right");
        }

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

        public static JoinsData<Dto, Tsource, TOther> InnerJoinOn<Tsource, TOther, Tkey, Dto>(this JoinsData<Dto> data,
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
                Ignore = data.Ignore,
            };

            return newJoin.CreateJoin(key, otherkey, "inner");
        }

        public static JoinsData<Dto, Tsource, TOther> And<Dto,Tsource, TOther, Tkey>(this JoinsData<Dto, Tsource, TOther> data,
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

        public static JoinsData<Dto, Tsource, TOther> CastColumnOfSecondAs<Dto, Tsource, TOther, Tkey , TotherKey>(this JoinsData<Dto, Tsource, TOther> data,
            Expression<Func<TOther, Tkey>> predicate, Expression<Func<Dto,TotherKey>> castOnDto) where Tkey : IComparable where TotherKey : IComparable
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

            if (propertyType.AllowCast(dtoPropType))
            {
                var oDp = data.OccupiedDtoProps.Where(x => x.PropName == propNameOther).First();
                int index = data.OccupiedDtoProps.IndexOf(oDp);
                data.OccupiedDtoProps[index].Occupied = true;
                data.OccupiedDtoProps[index].TableLetter = data.TablesToLetters.Where(x=>x.Table == typeof(TOther)).First().Letter;
                data.OccupiedDtoProps[index].TableProperty = propName;
                data.OccupiedDtoProps[index].TablePropertyType = propertyType;
                data.OccupiedDtoProps[index].WithCast = true;
            }

            return data;
        }

        public static JoinsData<Dto, Tsource, TOther> CastColumnOfFirstAs<Dto, Tsource, TOther, Tkey ,TotherKey>(this JoinsData<Dto, Tsource, TOther> data,
            Expression<Func<Tsource, Tkey>> predicate, Expression<Func<Dto,TotherKey>> castOnDto) where Tkey : IComparable where TotherKey : IComparable
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

            if (propertyType.AllowCast(dtoPropType))
            {
                var oDp = data.OccupiedDtoProps.Where(x => x.PropName == propNameOther).First();
                int index = data.OccupiedDtoProps.IndexOf(oDp);
                data.OccupiedDtoProps[index].Occupied = true;
                data.OccupiedDtoProps[index].TableLetter = data.TablesToLetters.Where(x => x.Table == typeof(Tsource)).First().Letter;
                data.OccupiedDtoProps[index].TableProperty = propName;
                data.OccupiedDtoProps[index].TablePropertyType = propertyType;
                data.OccupiedDtoProps[index].WithCast = true;
            }

            return data;
        }

        public static JoinsData<Dto, Tsource, TOther> WhereFirst<Dto,Tsource,TOther>(this JoinsData<Dto,Tsource,TOther> data,
            Expression<Func<Tsource,bool>> predicate)
        {
            if (data.Ignore)
            {
                return data;
            }

            string? letter = data.TablesToLetters.Where(x => x.Table == typeof(Tsource)).First().Letter;
            ColumnsAndParameters colsAndParams = predicate.Body.SplitMembersExtension(data.isMyShit, letter, data.DynamicParams,data.ParamsCount);
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

        public static JoinsData<Dto,Tsource, TOther> WhereSecond<Dto, Tsource, TOther>(this JoinsData<Dto, Tsource, TOther> data,
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

        public static JoinsData<Dto> Then<Dto,Tsource,TOther>(this JoinsData<Dto,Tsource,TOther> data)
        {
            return new JoinsData<Dto>
            {
                BaseTable = data.BaseTable,
                OccupiedDtoProps = data.OccupiedDtoProps,
                TablesToLetters = data.TablesToLetters,
                Joins = data.Joins,
                Letters= data.Letters,
                WherePredicates = data.WherePredicates,
                DynamicParams = data.DynamicParams,
                HelperIndex = data.HelperIndex,
                isMyShit = data.isMyShit,
                ParamsCount = data.ParamsCount,
                Ignore = false
            };
        }

        public static int StoreAsView<Dto>(this JoinsData<Dto> data) where Dto:BlackHoleDto
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();

            if(existingJoin != null)
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

        public static int StoreAsGView<Dto>(this JoinsData<Dto> data) where Dto : BlackHoleGDto
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

        public static IList<Dto> ExecuteQuery<Dto>(this JoinsData data) where Dto : BlackHoleDto
        {
            IList<Dto> joinResult = new List<Dto>();

            if(data.DtoType == typeof(Dto))
            {
                try
                {
                    using (IDbConnection connection = DatabaseStatics.ConnectionString.GetConnectionExtension())
                    {
                        TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                        string command = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {tL?.Table?.Name.SqlPropertyName(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
                        joinResult = connection.Query<Dto>(command, data.DynamicParams).ToList();
                    }
                }
                catch
                {}
            }

            return joinResult;
        }

        public static IList<Dto> ExecuteGQuery<Dto>(this JoinsData data) where Dto : BlackHoleGDto
        {
            IList<Dto> joinResult = new List<Dto>();

            if (data.DtoType == typeof(Dto))
            {
                try
                {
                    using (IDbConnection connection = DatabaseStatics.ConnectionString.GetConnectionExtension())
                    {
                        TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                        string command = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {tL?.Table?.Name.SqlPropertyName(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
                        joinResult = connection.Query<Dto>(command, data.DynamicParams).ToList();
                    }
                }
                catch
                { }
            }

            return joinResult;
        }

        public static IList<Dto> ExecuteQuery<Dto>(this JoinsData<Dto> data) where Dto : class
        {
            IList<Dto> joinResult = new List<Dto>();

            try
            {
                using (IDbConnection connection = DatabaseStatics.ConnectionString.GetConnectionExtension())
                {
                    TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                    string command = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {tL?.Table?.Name.SqlPropertyName(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
                    joinResult = connection.Query<Dto>(command, data.DynamicParams).ToList();
                }
            }
            catch
            {}

            return joinResult;
        }

        public static async Task<IList<Dto>> ExecuteQueryAsync<Dto>(this JoinsData<Dto> data) where Dto : class
        {
            IList<Dto> joinResult = new List<Dto>();
            try
            {
                using (IDbConnection connection = DatabaseStatics.ConnectionString.GetConnectionExtension())
                {
                    data.WherePredicates = data.TablesToLetters.RejectInactiveEntities(data.WherePredicates, data.isMyShit);
                    TableLetters? tL = data.TablesToLetters.Where(x => x.Table == data.BaseTable).FirstOrDefault();
                    string command = $"{data.OccupiedDtoProps.BuildCommand(data.isMyShit)} from {tL?.Table?.Name.SqlPropertyName(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates}";
                    var join = await connection.QueryAsync<Dto>(command, data.DynamicParams);
                    joinResult = join.ToList();
                }
            }
            catch { }

            return joinResult;
        }

        private static string RejectInactiveEntities(this List<TableLetters> involvedTables, string whereCommand, bool isMyShit)
        {
            string command = string.Empty;
            string inactiveColumn = "Inactive";
            string anD = "and";

            if(whereCommand == string.Empty)
            {
                anD = "where";
            }

            foreach(TableLetters table in involvedTables)
            {
                if(command != string.Empty)
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

            foreach(PropertyOccupation prop in usedProperties.Where(x => x.Occupied))
            {
                if (prop.WithCast)
                {
                    sqlCommand += $" cast({prop.TableLetter}.{prop.TableProperty.SqlPropertyName(isMyShit)} as {prop.PropType.SqlTypeFromType()}) as {prop.PropName.SqlPropertyName(isMyShit)},";
                }
                else
                {
                    sqlCommand += $" {prop.TableLetter}.{prop.PropName.SqlPropertyName(isMyShit)},";
                }
            }

            return sqlCommand.Substring(0, sqlCommand.Length - 1);
        }

        private static string SqlTypeFromType(this Type? type)
        {
            string[] SqlDatatypes = new string[5];

            switch (DatabaseStatics.DatabaseType)
            {
                case BHSqlTypes.MsSql:
                    SqlDatatypes = new[] { "nvarchar(500)", "int", "bigint", "decimal","float"};
                    break;
                case BHSqlTypes.MySql:
                    SqlDatatypes = new[] { "char", "int", "bigint", "dec","double"};
                    break;
                case BHSqlTypes.Postgres:
                    SqlDatatypes = new[] { "varchar(500)", "integer", "bigint", "numeric(10,5)", "numeric"};
                    break;
                case BHSqlTypes.SqlLite:
                    SqlDatatypes = new[] { "varchar(500)", "integer", "bigint", "decimal(10,5)","numeric"};
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
                default: result = SqlDatatypes[0];
                    break;
            }

            return result;
        }

        private static IDbConnection GetConnectionExtension(this string _connectionString)
        {
            IDbConnection _Sconnection = new SqlConnection();

            switch (DatabaseStatics.DatabaseType)
            {
                case BHSqlTypes.MsSql:
                    _Sconnection = new SqlConnection(_connectionString);
                    break;
                case BHSqlTypes.MySql:
                    _Sconnection = new MySqlConnection(_connectionString);
                    break;
                case BHSqlTypes.Postgres:
                    _Sconnection = new NpgsqlConnection(_connectionString);
                    break;
                case BHSqlTypes.SqlLite:
                    _Sconnection = new SqliteConnection(_connectionString);
                    break;
            }

            return _Sconnection;
        }

        private static JoinsData<Dto, Tsource, TOther> CreateJoin<Dto,Tsource, TOther>(this JoinsData<Dto,Tsource,TOther> data,LambdaExpression key, LambdaExpression otherKey, string joinType)
        {
            string? parameter = string.Empty;

            TableLetters? firstType = data.TablesToLetters.Where(x => x.Table == typeof(Tsource)).FirstOrDefault();

            if(firstType == null)
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

            if(secondTable == null)
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

        private static List<PropertyOccupation> BindPropertiesToDtoExtension(this List<PropertyOccupation> props,Type firstTable, Type secondTable, string? paramA, string? paramB)
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

        private static bool AllowCast(this Type firstType , Type secondType)
        {
            bool allow = true;
            string typeTotype = firstType.Name + secondType.Name;

            if(firstType.Name != secondType.Name)
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
                        allow = false;
                        break;
                }
            }

            return allow;
        }

        private static ColumnsAndParameters SplitMembersExtension(this Expression expression, bool isMyShit, string? letter, DynamicParameters parameters, int index)
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

        private static ColumnsAndParameters ExpressionTreeToSqlExtension(this List<ExpressionsData> data, bool isMyShit, string? letter, DynamicParameters parameters, int index)
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
                        parameters.Add(@childParams.ParamName, childParams.Value);
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
                        parameters.Add(@parentCols.ParamName, parentCols.Value);
                    }

                    index++;

                    ColumnAndParameter childCols = child.TranslateExpressionExtension(index, isMyShit, letter);

                    if (childCols.ParamName != string.Empty)
                    {
                        parameters.Add(@childCols.ParamName, childCols.Value);
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
                            parameters.Add(@parentParams.ParamName, parentParams.Value);
                        }
                        parent.sqlCommand = $"({parent.sqlCommand} {parentParams.Column} {parents[parentsCount - 1 - i].sqlCommand})";
                        index++;
                    }
                }
            }

            result = data[0].sqlCommand;

            return new ColumnsAndParameters { Columns = result, Parameters = parameters, Count = index };
        }

        private static ColumnAndParameter TranslateExpressionExtension( this ExpressionsData expression, int index, bool isMyShit, string? letter)
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
                    variable = expression?.leftMember?.ToString().Split(".");
                    column = $"{letter}.{variable?[1].SqlPropertyName(isMyShit)} = @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
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
                    variable = expression?.leftMember?.ToString().Split(".");
                    column = $"{letter}.{variable?[1].SqlPropertyName(isMyShit)} != @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
            }

            return new ColumnAndParameter { Column = column, ParamName = parameter, Value = value };
        }
    }
}
