using BlackHole.Core;
using BlackHole.DataProviders;
using BlackHole.Entities;
using BlackHole.Enums;
using BlackHole.Identifiers;
using BlackHole.Statics;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace BlackHole.CoreSupport
{
    internal static class BHCore
    {
        private static IDataProvider? ExecProvider { get; set; }

        internal static IDataProvider GetDataProvider(this Type IdType, string tableName)
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => new SqlServerDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType), tableName, DatabaseStatics.IsQuotedDatabase),
                BlackHoleSqlTypes.MySql => new MySqlDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType), tableName),
                BlackHoleSqlTypes.Postgres => new PostgresDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType), tableName),
                BlackHoleSqlTypes.SqlLite => new SqLiteDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType), tableName, DatabaseStatics.IsQuotedDatabase),
                _ => new OracleDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType), tableName),
            };
        }

        internal static IDbConnection GetConnection()
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => new SqlConnection(DatabaseStatics.ConnectionString),
                BlackHoleSqlTypes.MySql => new MySqlConnection(DatabaseStatics.ConnectionString),
                BlackHoleSqlTypes.Postgres => new NpgsqlConnection(DatabaseStatics.ConnectionString),
                BlackHoleSqlTypes.SqlLite => new SqliteConnection(DatabaseStatics.ConnectionString),
                _ => new OracleConnection(DatabaseStatics.ConnectionString),
            };
        }

        internal static IDataProvider GetDataProvider()
        {
            if(ExecProvider == null)
            {
                return DatabaseStatics.DatabaseType switch
                {
                    BlackHoleSqlTypes.SqlServer => new SqlServerDataProvider(DatabaseStatics.ConnectionString, DatabaseStatics.IsQuotedDatabase),
                    BlackHoleSqlTypes.MySql => new MySqlDataProvider(DatabaseStatics.ConnectionString),
                    BlackHoleSqlTypes.Postgres => new PostgresDataProvider(DatabaseStatics.ConnectionString),
                    BlackHoleSqlTypes.SqlLite => new SqLiteDataProvider(DatabaseStatics.ConnectionString, DatabaseStatics.IsQuotedDatabase),
                    _ => new OracleDataProvider(DatabaseStatics.ConnectionString),
                };
            }

            return ExecProvider;
        }

        internal static string[] GetReturningPrimaryKey<T>(this EntitySettings<T> pkOptions, string MainColumn, string Tablename)
        {
            if (pkOptions.HasAutoIncrement)
            {
                return DatabaseStatics.DatabaseType switch
                {
                    BlackHoleSqlTypes.SqlServer => new string[] { $"output Inserted.{MainColumn}", "" },
                    BlackHoleSqlTypes.MySql => new string[] { "", ";SELECT LAST_INSERT_ID();" },
                    BlackHoleSqlTypes.Postgres => new string[] { "", $"returning {Tablename}.{MainColumn}" },
                    BlackHoleSqlTypes.SqlLite => new string[] { "", $"returning {MainColumn}" },
                    _ => new string[] { "", $"returning {MainColumn} into :OracleReturningValue" },
                };
            }

            return new string[2];
        }

        internal static string[] GetLimiter(this int rowsCount)
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => new string[] { $" TOP {rowsCount} ", "" },
                BlackHoleSqlTypes.MySql => new string[] { "", $" limit {rowsCount} " },
                BlackHoleSqlTypes.Postgres => new string[] { "", $" limit {rowsCount} " },
                BlackHoleSqlTypes.SqlLite => new string[] { "", $" limit {rowsCount} " },
                _ => new string[] { "", $" and rownum <= {rowsCount} " },
            };
        }

        internal static bool CheckActivator(this Type entity)
        {
            return entity.GetCustomAttributes(true).Any(x => x.GetType() == typeof(UseActivator));
        }

        internal static string GetDatabaseSchema()
        {
            if (DatabaseStatics.DatabaseSchema != string.Empty)
            {
                return $"{DatabaseStatics.DatabaseSchema}.";
            }
            return string.Empty;
        }

        private static BlackHoleIdTypes GetIdType(Type type)
        {
            if (type == typeof(int))
            {
                return BlackHoleIdTypes.IntId;
            }

            if (type == typeof(Guid))
            {
                return BlackHoleIdTypes.GuidId;
            }

            return BlackHoleIdTypes.StringId;
        }

        internal static ColumnsAndParameters SplitMembers<T>(this Expression expression, bool isMyShit, string? letter, List<BlackHoleParameter>? DynamicParams, int index)
        {
            List<ExpressionsData> expressionTree = new();

            BinaryExpression? currentOperation = expression as BinaryExpression;
            MethodCallExpression? methodCallOperation = expression as MethodCallExpression;

            int currentIndx = 0;
            bool startTranslate = false;

            if (currentOperation != null || methodCallOperation != null)
            {
                startTranslate = true;

                expressionTree.Add(new ExpressionsData()
                {
                    operation = currentOperation,
                    leftMethodMember = methodCallOperation,
                    leftMember = currentOperation?.Left as MemberExpression,
                    rightMember = currentOperation?.Right as MemberExpression,
                    expressionType = currentOperation != null ? currentOperation.NodeType : ExpressionType.Default,
                    rightChecked = false,
                    leftChecked = false,
                    memberValue = null
                });
            }

            while (startTranslate)
            {
                bool addTotree = false;

                if (expressionTree[currentIndx].operation != null)
                {
                    if (expressionTree[currentIndx].expressionType == ExpressionType.AndAlso || expressionTree[currentIndx].expressionType == ExpressionType.OrElse)
                    {

                        BinaryExpression? leftOperation = expressionTree[currentIndx].operation?.Left as BinaryExpression;
                        BinaryExpression? rightOperation = expressionTree[currentIndx].operation?.Right as BinaryExpression;
                        MethodCallExpression? leftCallOperation = expressionTree[currentIndx].operation?.Left as MethodCallExpression;
                        MethodCallExpression? rightCallOperation = expressionTree[currentIndx].operation?.Right as MethodCallExpression;

                        if (!expressionTree[currentIndx].leftChecked && (leftOperation != null || leftCallOperation != null))
                        {
                            expressionTree.Add(new ExpressionsData()
                            {
                                operation = leftOperation,
                                leftMethodMember = leftCallOperation,
                                expressionType = leftOperation != null ? leftOperation.NodeType : ExpressionType.Default,
                                rightChecked = false,
                                leftChecked = false,
                                memberValue = null,
                                parentIndex = currentIndx
                            });
                            expressionTree[currentIndx].leftChecked = true;
                            addTotree = true;
                        }

                        if (!expressionTree[currentIndx].rightChecked && (rightOperation != null || rightCallOperation != null))
                        {
                            expressionTree.Add(new ExpressionsData()
                            {
                                operation = rightOperation,
                                rightMethodMember = rightCallOperation,
                                expressionType = rightOperation != null ? rightOperation.NodeType : ExpressionType.Default,
                                rightChecked = false,
                                leftChecked = false,
                                memberValue = null,
                                parentIndex = currentIndx
                            });
                            expressionTree[currentIndx].rightChecked = true;
                            addTotree = true;
                        }

                        if (addTotree)
                        {
                            currentIndx = expressionTree.Count - 1;
                        }
                    }
                    else
                    {
                        if (!expressionTree[currentIndx].rightChecked)
                        {
                            if (expressionTree[currentIndx].operation?.Right is MemberExpression rightMember)
                            {
                                expressionTree[currentIndx].InvokeOrTake<T>(rightMember, true);
                            }

                            if (expressionTree[currentIndx].operation?.Right is ConstantExpression rightConstant)
                            {
                                expressionTree[currentIndx].memberValue = rightConstant?.Value;
                            }

                            if (expressionTree[currentIndx].operation?.Right is BinaryExpression rightBinary)
                            {
                                expressionTree[currentIndx].memberValue = Expression.Lambda(rightBinary).Compile().DynamicInvoke();
                            }

                            if (expressionTree[currentIndx].operation?.Right is MethodCallExpression rightmethodMember)
                            {
                                expressionTree[currentIndx].rightMethodMember = rightmethodMember;
                            }

                            expressionTree[currentIndx].rightChecked = true;
                        }

                        if (!expressionTree[currentIndx].leftChecked)
                        {
                            if (expressionTree[currentIndx].operation?.Left is MemberExpression leftMember)
                            {
                                expressionTree[currentIndx].InvokeOrTake<T>(leftMember, false);
                            }

                            if (expressionTree[currentIndx].operation?.Left is ConstantExpression leftConstant)
                            {
                                expressionTree[currentIndx].memberValue = leftConstant?.Value;
                            }

                            if (expressionTree[currentIndx].operation?.Left is BinaryExpression leftBinary)
                            {
                                expressionTree[currentIndx].memberValue = Expression.Lambda(leftBinary).Compile().DynamicInvoke();
                            }

                            if (expressionTree[currentIndx].operation?.Left is MethodCallExpression leftmethodMember)
                            {
                                expressionTree[currentIndx].leftMethodMember = leftmethodMember;
                            }

                            expressionTree[currentIndx].leftChecked = true;
                        }
                    }
                }

                if (expressionTree[currentIndx].methodData.Count == 0)
                {
                    MethodCallExpression? leftMethodMember = expressionTree[currentIndx].leftMethodMember;
                    MethodCallExpression? rightMethodMember = expressionTree[currentIndx].rightMethodMember;

                    if (leftMethodMember != null)
                    {
                        var func = leftMethodMember.Method;
                        var arguments = leftMethodMember.Arguments;
                        var obj = leftMethodMember.Object;
                        bool cleanOfMembers = true;

                        if (obj?.NodeType == ExpressionType.MemberAccess)
                        {
                            cleanOfMembers = false;
                        }

                        if (!expressionTree[currentIndx].methodChecked)
                        {
                            List<object?> MethodArguments = new();
                            object?[] parameters = new object[arguments.Count];

                            for (int i = 0; i < arguments.Count; i++)
                            {
                                if (arguments[i] is MemberExpression argMemmber)
                                {
                                    string? typeName = argMemmber.Member.ReflectedType?.FullName;

                                    if (typeName != null && (typeName == typeof(T).BaseType?.FullName || typeName == typeof(T).FullName))
                                    {
                                        cleanOfMembers = false;
                                        obj = argMemmber;
                                        MethodArguments.Add(argMemmber.Member);
                                    }
                                    else
                                    {
                                        parameters[i] = Expression.Lambda(argMemmber).Compile().DynamicInvoke();
                                        MethodArguments.Add(parameters[i]);
                                    }
                                }

                                if (arguments[i] is ConstantExpression argConstant)
                                {
                                    parameters[i] = argConstant.Value;
                                    MethodArguments.Add(argConstant.Value);
                                }

                                if (arguments[i] is BinaryExpression argBinary)
                                {
                                    parameters[i] = Expression.Lambda(argBinary).Compile().DynamicInvoke();
                                    MethodArguments.Add(parameters[i]);
                                }

                                if (arguments[i] is MethodCallExpression argMethod)
                                {
                                    foreach (var arg in argMethod.Arguments)
                                    {
                                        MemberExpression? SubargMemmber = arg as MemberExpression;

                                        if (SubargMemmber?.Member.ReflectedType?.FullName == typeof(T).FullName)
                                        {
                                            cleanOfMembers = false;
                                        }
                                    }

                                    if (cleanOfMembers)
                                    {
                                        parameters[i] = Expression.Lambda(argMethod).Compile().DynamicInvoke();
                                        MethodArguments.Add(parameters[i]);
                                    }
                                }

                                if(arguments[i] is LambdaExpression argLambda)
                                {
                                    expressionTree[currentIndx].rightMember = argLambda.Body as MemberExpression;
                                }
                            }

                            if (cleanOfMembers)
                            {
                                if (obj != null)
                                {
                                    object? skata = obj.Type != typeof(string) ? Activator.CreateInstance(obj.Type, null) : string.Empty;
                                    expressionTree[currentIndx].memberValue = func.Invoke(skata, parameters);
                                }
                            }
                            else
                            {
                                expressionTree[currentIndx].methodData.Add(new MethodExpressionData 
                                { 
                                    MethodName = func.Name, 
                                    MethodArguments = MethodArguments,
                                    CastedOn = obj ,
                                    ComparedValue = expressionTree[currentIndx].memberValue,
                                    CompareProperty = expressionTree[currentIndx].rightMember,
                                    OperatorType = expressionTree[currentIndx].expressionType,
                                    ReverseOperator = true,
                                    TableName = typeof(T).Name
                                });
                            }
                        }
                    }

                    if (rightMethodMember != null)
                    {
                        var func = rightMethodMember.Method;
                        var arguments = rightMethodMember.Arguments;
                        var obj = rightMethodMember.Object;
                        bool cleanOfMembers = true;

                        if (obj?.NodeType == ExpressionType.MemberAccess)
                        {
                            cleanOfMembers = false;
                        }

                        if (!expressionTree[currentIndx].methodChecked)
                        {
                            List<object?> MethodArguments = new();
                            object?[] parameters = new object[arguments.Count];

                            for (int i = 0; i < arguments.Count; i++)
                            {
                                if (arguments[i] is MemberExpression argMemmber)
                                {
                                    string? typeName = argMemmber.Member.ReflectedType?.FullName;

                                    if (typeName != null && (typeName == typeof(T).BaseType?.FullName || typeName == typeof(T).FullName))
                                    {
                                        cleanOfMembers = false;
                                        obj = argMemmber;
                                        MethodArguments.Add(argMemmber.Member);
                                    }
                                    else
                                    {
                                        parameters[i] = Expression.Lambda(argMemmber).Compile().DynamicInvoke();
                                        MethodArguments.Add(parameters[i]);
                                    }
                                }

                                if (arguments[i] is ConstantExpression argConstant)
                                {
                                    parameters[i] = argConstant.Value;
                                    MethodArguments.Add(argConstant.Value);
                                }

                                if (arguments[i] is BinaryExpression argBinary)
                                {
                                    parameters[i] = Expression.Lambda(argBinary).Compile().DynamicInvoke();
                                    MethodArguments.Add(parameters[i]);
                                }

                                if (arguments[i] is MethodCallExpression argMethod)
                                {
                                    foreach (var arg in argMethod.Arguments)
                                    {
                                        MemberExpression? SubargMemmber = arg as MemberExpression;

                                        if (SubargMemmber?.Member.ReflectedType?.FullName == typeof(T).FullName)
                                        {
                                            cleanOfMembers = false;
                                        }
                                    }

                                    if (cleanOfMembers)
                                    {
                                        parameters[i] = Expression.Lambda(argMethod).Compile().DynamicInvoke();
                                        MethodArguments.Add(parameters[i]);
                                    }
                                }

                                if (arguments[i] is LambdaExpression argLambda)
                                {
                                    expressionTree[currentIndx].leftMember = argLambda.Body as MemberExpression;
                                }
                            }

                            if (cleanOfMembers)
                            {
                                if (obj != null)
                                {
                                    object? skata = obj.Type != typeof(string) ? Activator.CreateInstance(obj.Type, null) : string.Empty;
                                    expressionTree[currentIndx].memberValue = func.Invoke(skata, parameters);
                                }
                            }
                            else
                            {
                                expressionTree[currentIndx].methodData.Add(new MethodExpressionData
                                {
                                    MethodName = func.Name,
                                    MethodArguments = MethodArguments,
                                    CastedOn = obj,
                                    ComparedValue = expressionTree[currentIndx].memberValue,
                                    CompareProperty = expressionTree[currentIndx].leftMember,
                                    OperatorType = expressionTree[currentIndx].expressionType,
                                    ReverseOperator = false,
                                    TableName = typeof(T).Name
                                });
                            }
                        }
                    }
                }
                
                if (!addTotree)
                {
                    currentIndx -= 1;
                }

                if (currentIndx < 0)
                {
                    startTranslate = false;
                }
            }

            return expressionTree.ExpressionTreeToSql(isMyShit, letter, DynamicParams, index);
        }
        
        private static void InvokeOrTake<T>(this ExpressionsData thisBranch, MemberExpression memberExp, bool isRight)
        {
            string? typeName = memberExp.Member.ReflectedType?.FullName;

            if (typeName != null && (typeName == typeof(T).BaseType?.FullName || typeName == typeof(T).FullName))
            {
                try
                {
                    thisBranch.memberValue = Expression.Lambda(memberExp).Compile().DynamicInvoke();
                }
                catch
                {
                    if (isRight)
                    {
                        thisBranch.rightMember = memberExp;
                    }
                    else
                    {
                        thisBranch.leftMember = memberExp;
                    }
                }
            }
            else
            {
                thisBranch.memberValue = Expression.Lambda(memberExp).Compile().DynamicInvoke();
            }
        }

        internal static ColumnsAndParameters ExpressionTreeToSql(this List<ExpressionsData> data, bool isMyShit, string? letter, List<BlackHoleParameter>? parameters, int index)
        {
            if(parameters == null)
            {
                parameters = new List<BlackHoleParameter>();
            }

            List<ExpressionsData> children = data.Where(x => x.memberValue != null || x.methodData.Count > 0).ToList();
            string[] translations = new string[children.Count];

            foreach (ExpressionsData child in children)
            {
                ExpressionsData parent = data[child.parentIndex];

                if (child.methodData.Count > 0)
                {
                    if(child.memberValue != null && child.methodData[0].ComparedValue == null  && child.methodData[0].CompareProperty == null)
                    {
                        child.methodData[0].ComparedValue = child.memberValue;
                    }

                    SqlFunctionsReader sqlFunctionResult = new(child.methodData[0], index, letter, isMyShit);

                    if (sqlFunctionResult.ParamName != string.Empty)
                    {
                        parameters.Add(new BlackHoleParameter { Name = sqlFunctionResult.ParamName, Value = sqlFunctionResult.Value });
                        index++;
                    }

                    if(parent.sqlCommand == string.Empty)
                    {
                        parent.sqlCommand = $"{sqlFunctionResult.SqlCommand}";
                    }
                    else
                    {
                        parent.sqlCommand += $" and {sqlFunctionResult.SqlCommand}";
                    }

                    parent.leftChecked = false;
                }
                else
                {
                    if (parent.leftChecked)
                    {
                        ColumnAndParameter childParams = child.TranslateExpression(index, isMyShit, letter);

                        if (childParams.ParamName != string.Empty)
                        {
                            parameters.Add(new BlackHoleParameter { Name = childParams.ParamName, Value = childParams.Value });
                        }

                        parent.sqlCommand = $"{childParams.Column}";

                        parent.leftChecked = false;
                        index++;
                    }
                    else
                    {
                        ColumnAndParameter parentCols = parent.TranslateExpression(index, isMyShit, letter);

                        if (parentCols.ParamName != string.Empty)
                        {
                            parameters.Add(new BlackHoleParameter { Name = parentCols.ParamName, Value = parentCols.Value });
                        }

                        index++;

                        ColumnAndParameter childCols = child.TranslateExpression(index, isMyShit, letter);

                        if (childCols.ParamName != string.Empty)
                        {
                            parameters.Add(new BlackHoleParameter { Name = childCols.ParamName, Value = childCols.Value });
                        }

                        parent.sqlCommand = $"({parent.sqlCommand} {parentCols.Column} {childCols.Column})";
                        index++;
                    }
                }
            }

            List<ExpressionsData> parents = data.Where(x => x.memberValue == null && x.methodData.Count == 0 && x.expressionType != ExpressionType.Default).ToList();

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
                        ColumnAndParameter parentParams = parent.TranslateExpression(index, isMyShit, letter);

                        if (parentParams.ParamName != string.Empty)
                        {
                            parameters.Add(new BlackHoleParameter { Name = parentParams.ParamName, Value = parentParams.Value });
                        }

                        parent.sqlCommand = $"({parent.sqlCommand} {parentParams.Column} {parents[parentsCount - 1 - i].sqlCommand})";

                        index++;
                    }
                }
            }

            return new ColumnsAndParameters { Columns = data[0].sqlCommand, Parameters = parameters, Count = index};
        }

        private static ColumnAndParameter TranslateExpression(this ExpressionsData expression, int index, bool isMyShit, string? letter)
        {
            string? column = string.Empty;
            string? parameter = string.Empty;
            object? value = new();
            string[]? variable = new string[2];
            string subLetter = letter != string.Empty ? $"{letter}." : string.Empty;
            string sqlOperator = "=";

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
                    variable = expression?.leftMember != null ? expression?.leftMember.ToString().Split(".") : expression?.rightMember?.ToString().Split(".");
                    column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} = @{variable?[1]}{index}";
                    if (value == null)
                    {
                        column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} is @{variable?[1]}{index}";
                    }
                    parameter = $"{variable?[1]}{index}";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    if(expression?.leftMember != null)
                    {
                        variable = expression?.leftMember.ToString().Split(".");
                        sqlOperator = ">=";
                    }
                    else
                    {
                        variable = expression?.rightMember?.ToString().Split(".");
                        sqlOperator = "<=";
                    }
                    column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} {sqlOperator} @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.LessThanOrEqual:
                    if (expression?.leftMember != null)
                    {
                        variable = expression?.leftMember.ToString().Split(".");
                        sqlOperator = "<=";
                    }
                    else
                    {
                        variable = expression?.rightMember?.ToString().Split(".");
                        sqlOperator = ">=";
                    }
                    column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} {sqlOperator} @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.LessThan:
                    if (expression?.leftMember != null)
                    {
                        variable = expression?.leftMember.ToString().Split(".");
                        sqlOperator = "<";
                    }
                    else
                    {
                        variable = expression?.rightMember?.ToString().Split(".");
                        sqlOperator = ">";
                    }
                    column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} {sqlOperator} @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.GreaterThan:
                    if (expression?.leftMember != null)
                    {
                        variable = expression?.leftMember.ToString().Split(".");
                        sqlOperator = ">";
                    }
                    else
                    {
                        variable = expression?.rightMember?.ToString().Split(".");
                        sqlOperator = "<";
                    }
                    column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} {sqlOperator} @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.NotEqual:
                    value = expression?.memberValue;
                    variable = expression?.leftMember != null ? expression?.leftMember.ToString().Split(".") : expression?.rightMember?.ToString().Split(".");
                    column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} != @{variable?[1]}{index}";
                    if (value == null)
                    {
                        column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} is not @{variable?[1]}{index}";
                    }
                    parameter = $"{variable?[1]}{index}";
                    break;
                case ExpressionType.Default:
                    column = "1 != 1";
                    break;
            }

            return new ColumnAndParameter { Column = column, ParamName = parameter, Value = value};
        }

        internal static void CreateJoin<Dto, TSource, TOther>(this JoinsData data, LambdaExpression key, LambdaExpression otherKey, string joinType, bool first)
        {
            if (first)
            {
                string? parameterOne = key.Parameters[0].Name;

                data.isMyShit = true;
                data.BaseTable = typeof(TSource);
                data.Ignore = false;
                bool OpenEntity = typeof(TSource).BaseType?.GetGenericTypeDefinition() == typeof(BHOpenEntity<>);

                data.TablesToLetters.Add(new TableLetters { Table = typeof(TSource), Letter = parameterOne, IsOpenEntity = OpenEntity });
                data.Letters.Add(parameterOne);
                data.BindPropertiesToDtoExtension(typeof(TSource), parameterOne);
            }

            string? parameter = string.Empty;

            TableLetters? firstType = data.TablesToLetters.Where(x => x.Table == typeof(TSource)).FirstOrDefault();

            if (firstType == null)
            {
                data.Ignore = true;
            }
            else
            {
                parameter = firstType.Letter;
            }

            if (!data.Ignore)
            {
                MemberExpression? member = key.Body as MemberExpression;
                string? propName = member?.Member.Name;

                string? parameterOther = otherKey.Parameters[0].Name;
                MemberExpression? memberOther = otherKey.Body as MemberExpression;
                string? propNameOther = memberOther?.Member.Name;

                TableLetters? secondTable = data.TablesToLetters.FirstOrDefault(x => x.Table == typeof(TOther));

                if (secondTable == null)
                {
                    if (data.Letters.Contains(parameterOther))
                    {
                        parameterOther += data.HelperIndex.ToString();
                        data.HelperIndex++;
                    }

                    data.Letters.Add(parameterOther);

                    bool isOpen = typeof(TOther).BaseType?.GetGenericTypeDefinition() == typeof(BHOpenEntity<>);
                    data.TablesToLetters.Add(new TableLetters { Table = typeof(TOther), Letter = parameterOther, IsOpenEntity = isOpen });
                }
                else
                {
                    parameterOther = secondTable.Letter;
                }

                string schemaName = GetDatabaseSchema();

                data.Joins += $" {joinType} join {schemaName}{typeof(TOther).Name.SkipNameQuotes(data.isMyShit)} {parameterOther} on {parameterOther}.{propNameOther.SkipNameQuotes(data.isMyShit)} = {parameter}.{propName.SkipNameQuotes(data.isMyShit)}";
                data.BindPropertiesToDtoExtension(typeof(TOther), parameterOther);
            }
        }

        internal static void InitializeOccupiedProperties(this JoinsData data)
        {
            foreach(PropertyInfo dtoProperty in data.DtoType.GetProperties())
            {
                data.OccupiedDtoProps.Add(new PropertyOccupation
                {
                    PropName = dtoProperty.Name,
                    PropType = dtoProperty.PropertyType,
                    Occupied = false,
                    WithCast = 0
                });
            }
        }

        private static void BindPropertiesToDtoExtension(this JoinsData data, Type secondTable, string? paramB)
        {
            List<string> OtherPropNames = secondTable.GetProperties().Select(x=>x.Name).ToList();

            for(int i = 0 ; i < data.OccupiedDtoProps.Count; i++)
            {
                PropertyOccupation property = data.OccupiedDtoProps[i];

                if (OtherPropNames.Contains(property.PropName) && !property.Occupied)
                {
                    Type? TOtherPropType = secondTable.GetProperty(property.PropName)?.PropertyType;

                    if (TOtherPropType == property.PropType)
                    {
                        data.OccupiedDtoProps[i].Occupied = true;
                        data.OccupiedDtoProps[i].TableProperty = TOtherPropType?.Name;
                        data.OccupiedDtoProps[i].TablePropertyType = TOtherPropType;
                        data.OccupiedDtoProps[i].TableLetter = paramB;
                    }
                }
            }
        }

        internal static void CastColumn<TSource>(this JoinsData data, LambdaExpression predicate, LambdaExpression castOnDto)
        {
            if (!data.Ignore)
            {
                Type propertyType = predicate.Body.Type;
                MemberExpression? member = predicate.Body as MemberExpression;
                string? propName = member?.Member.Name;

                Type dtoPropType = castOnDto.Body.Type;
                MemberExpression? memberOther = castOnDto.Body as MemberExpression;
                string? propNameOther = memberOther?.Member.Name;
                int allow = propertyType.AllowCast(dtoPropType);

                if (allow != 0)
                {
                    var oDp = data.OccupiedDtoProps.First(x => x.PropName == propNameOther);
                    int index = data.OccupiedDtoProps.IndexOf(oDp);
                    data.OccupiedDtoProps[index].Occupied = true;
                    data.OccupiedDtoProps[index].TableLetter = data.TablesToLetters.First(x => x.Table == typeof(TSource)).Letter;
                    data.OccupiedDtoProps[index].TableProperty = propName;
                    data.OccupiedDtoProps[index].TablePropertyType = propertyType;
                    data.OccupiedDtoProps[index].WithCast = allow;
                }
            }
        }

        internal static void Additional<TSource, TOther>(this JoinsData data, LambdaExpression key, LambdaExpression otherKey, string additionalType)
        {
            if (!data.Ignore)
            {
                string? firstLetter = data.TablesToLetters.First(t => t.Table == typeof(TSource)).Letter;
                string? secondLetter = data.TablesToLetters.First(t => t.Table == typeof(TOther)).Letter;

                MemberExpression? member = key.Body as MemberExpression;
                string? propName = member?.Member.Name;
                MemberExpression? memberOther = otherKey.Body as MemberExpression;
                string? propNameOther = memberOther?.Member.Name;

                data.Joins += $" {additionalType} {secondLetter}.{propNameOther.SkipNameQuotes(data.isMyShit)} = {firstLetter}.{propName.SkipNameQuotes(data.isMyShit)}";
            }
        }

        internal static void WhereJoin<TSource>(this JoinsData data, Expression<Func<TSource, bool>> predicate)
        {
            if (!data.Ignore)
            {
                string? letter = data.TablesToLetters.First(x => x.Table == typeof(TSource)).Letter;
                ColumnsAndParameters colsAndParams = predicate.Body.SplitMembers<TSource>(data.isMyShit, letter, data.DynamicParams, data.ParamsCount);
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
            }
        }

        internal static ColumnsAndParameters? TranslateJoin<Dto>(this JoinsData data) where Dto : IBHDtoIdentifier
        {
            if (data.DtoType == typeof(Dto))
            {
                data.RejectInactiveEntities();
                TableLetters? tL = data.TablesToLetters.FirstOrDefault(x => x.Table == data.BaseTable);
                string schemaName = GetDatabaseSchema();
                string commandText = $"{data.BuildCommand()} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.isMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates} {data.OrderByOptions}";
                return new ColumnsAndParameters { Columns = commandText, Parameters = data.DynamicParams };
            }
            return null;
        }

        internal static string OrderByToSql<T>(this BHOrderBy<T> orderByConfig, bool isMyShit)
        {
            if (orderByConfig.orderBy.LockedByError)
            {
                return string.Empty;
            }

            StringBuilder orderby = new();
            string limiter = string.Empty;

            foreach (OrderByPair pair in orderByConfig.orderBy.OrderProperties)
            {
                orderby.Append($", {pair.PropertyName.SkipNameQuotes(isMyShit)} {pair.Oriantation}");
            }

            if (orderByConfig.orderBy.TakeSpecificRange)
            {
                limiter = orderByConfig.orderBy.RowsLimiter();
            }

            return $"order by{orderby.ToString().Remove(0, 1)}{limiter}";
        }

        internal static void OrderByToSqlJoins<T>(this JoinsData data, BHOrderBy<T> orderByConfig)
        {
            if (orderByConfig.orderBy.LockedByError)
            {
                data.OrderByOptions = string.Empty;
            }
            else
            {
                StringBuilder orderby = new();
                string limiter = string.Empty;
                int counter = 0;

                foreach (OrderByPair pair in orderByConfig.orderBy.OrderProperties)
                {
                    if (data.OccupiedDtoProps.FirstOrDefault(x => x.PropName == pair.PropertyName) is PropertyOccupation occupation)
                    {
                        counter++;
                        orderby.Append($", {occupation.TableLetter}.{occupation.TableProperty.SkipNameQuotes(data.isMyShit)} {pair.Oriantation}");
                    }
                }

                if (orderByConfig.orderBy.TakeSpecificRange)
                {
                    limiter = orderByConfig.orderBy.RowsLimiter();
                }

                if (counter > 0)
                {
                    data.OrderByOptions = $"order by{orderby.ToString().Remove(0, 1)}{limiter}";
                }
            }
        }

        private static string RowsLimiter<T>(this BlackHoleOrderBy<T> limiter)
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

        private static void RejectInactiveEntities(this JoinsData data)
        {
            string command = string.Empty;
            string inactiveColumn = "Inactive";
            string anD = "and";

            if (data.WherePredicates == string.Empty)
            {
                anD = "where";
            }

            foreach (TableLetters table in data.TablesToLetters)
            {
                if (!table.IsOpenEntity)
                {
                    if (command != string.Empty)
                    {
                        anD = "and";
                    }

                    command += $" {anD} {table.Letter}.{inactiveColumn.SkipNameQuotes(data.isMyShit)} = 0 ";
                }
            }

            data.WherePredicates += command;
        }

        private static string BuildCommand(this JoinsData data)
        {
            string sqlCommand = "select ";

            foreach (PropertyOccupation prop in data.OccupiedDtoProps.Where(x => x.Occupied))
            {
                sqlCommand += prop.WithCast switch
                {
                    1 => $" {prop.TableLetter}.{prop.TableProperty.SkipNameQuotes(data.isMyShit)} as {prop.PropName.SkipNameQuotes(data.isMyShit)},",
                    2 => $" cast({prop.TableLetter}.{prop.TableProperty.SkipNameQuotes(data.isMyShit)} as {prop.PropType.SqlTypeFromType()}) as {prop.PropName.SkipNameQuotes(data.isMyShit)},",
                    _ => $" {prop.TableLetter}.{prop.PropName.SkipNameQuotes(data.isMyShit)},",
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

            if (type != null && !string.IsNullOrEmpty(TypeName))
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

        private static int AllowCast(this Type firstType, Type secondType)
        {
            int allow = 2;
            string typeTotype = firstType.Name + secondType.Name;

            if (firstType == typeof(Guid))
            {
                BlackHoleSqlTypes sqlType = DatabaseStatics.DatabaseType;
                if (sqlType != BlackHoleSqlTypes.Postgres && sqlType != BlackHoleSqlTypes.SqlServer)
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

        private static List<PropertyOccupation> BindPropertiesToDto<T>( this List<string> Columns, Type otherTable, Type dto, string? paramA, string? paramB)
        {
            List<PropertyOccupation> result = new();
            List<string> OtherPropNames = new();

            foreach (PropertyInfo otherProp in otherTable.GetProperties())
            {
                OtherPropNames.Add(otherProp.Name);
            }

            foreach (PropertyInfo property in dto.GetProperties())
            {
                PropertyOccupation occupation = new();

                if (Columns.Contains(property.Name))
                {
                    Type? TpropType = typeof(T).GetProperty(property.Name)?.PropertyType;

                    if (TpropType == property.PropertyType)
                    {
                        occupation = new PropertyOccupation
                        {
                            PropName = property.Name,
                            PropType = property.PropertyType,
                            Occupied = true,
                            TableLetter = paramA,
                            TableProperty = property.Name,
                            TablePropertyType = TpropType,
                            WithCast = 0
                        };
                    }
                }

                if (OtherPropNames.Contains(property.Name) && !occupation.Occupied)
                {
                    Type? TOtherPropType = otherTable.GetProperty(property.Name)?.PropertyType;

                    if (TOtherPropType == property.PropertyType)
                    {
                        occupation = new PropertyOccupation
                        {
                            PropName = property.Name,
                            PropType = property.PropertyType,
                            Occupied = true,
                            TableLetter = paramB,
                            TableProperty = property.Name,
                            TablePropertyType = TOtherPropType,
                            WithCast = 0
                        };
                    }
                }

                if (!occupation.Occupied)
                {
                    occupation = new PropertyOccupation
                    {
                        PropName = property.Name,
                        PropType = property.PropertyType,
                        Occupied = false,
                        WithCast = 0
                    };
                }

                result.Add(occupation);
            }

            return result;
        }

        internal static string SkipNameQuotes(this string? propName, bool isMyShit)
        {
            string result = propName ?? string.Empty;

            if (!isMyShit)
            {
                result = $@"""{propName}""";
            }

            return result;
        }

        internal static void AdditionalParameters(this ColumnsAndParameters colsAndParams, object item)
        {
            Type type = item.GetType();
            PropertyInfo[] props = type.GetProperties();

            foreach (var prop in props)
            {
                colsAndParams.Parameters.Add(new BlackHoleParameter { Name = prop.Name, Value = prop.GetValue(item) });
            }
        }
    }
}
