using BlackHole.Core;
using BlackHole.DataProviders;
using BlackHole.Entities;
using BlackHole.Enums;
using BlackHole.Identifiers;
using BlackHole.Logger;
using BlackHole.Statics;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace BlackHole.Engine
{
    internal static class BlackHoleEngine
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
                    Operation = currentOperation,
                    LeftMethodMember = methodCallOperation,
                    LeftMember = currentOperation?.Left as MemberExpression,
                    RightMember = currentOperation?.Right as MemberExpression,
                    OperationType = currentOperation != null ? currentOperation.NodeType : ExpressionType.Default,
                    RightChecked = false,
                    LeftChecked = false,
                    MemberValue = null
                });
            }

            while (startTranslate)
            {
                bool addTotree = false;

                if (expressionTree[currentIndx].Operation != null)
                {
                    if (expressionTree[currentIndx].OperationType == ExpressionType.AndAlso || expressionTree[currentIndx].OperationType == ExpressionType.OrElse)
                    {

                        BinaryExpression? leftOperation = expressionTree[currentIndx].Operation?.Left as BinaryExpression;
                        BinaryExpression? rightOperation = expressionTree[currentIndx].Operation?.Right as BinaryExpression;
                        MethodCallExpression? leftCallOperation = expressionTree[currentIndx].Operation?.Left as MethodCallExpression;
                        MethodCallExpression? rightCallOperation = expressionTree[currentIndx].Operation?.Right as MethodCallExpression;

                        if (!expressionTree[currentIndx].LeftChecked && (leftOperation != null || leftCallOperation != null))
                        {
                            expressionTree.Add(new ExpressionsData()
                            {
                                Operation = leftOperation,
                                LeftMethodMember = leftCallOperation,
                                OperationType = leftOperation != null ? leftOperation.NodeType : ExpressionType.Default,
                                RightChecked = false,
                                LeftChecked = false,
                                MemberValue = null,
                                ParentIndex = currentIndx
                            });
                            expressionTree[currentIndx].LeftChecked = true;
                            addTotree = true;
                        }

                        if (!expressionTree[currentIndx].RightChecked && (rightOperation != null || rightCallOperation != null))
                        {
                            expressionTree.Add(new ExpressionsData()
                            {
                                Operation = rightOperation,
                                RightMethodMember = rightCallOperation,
                                OperationType = rightOperation != null ? rightOperation.NodeType : ExpressionType.Default,
                                RightChecked = false,
                                LeftChecked = false,
                                MemberValue = null,
                                ParentIndex = currentIndx
                            });
                            expressionTree[currentIndx].RightChecked = true;
                            addTotree = true;
                        }

                        if (addTotree)
                        {
                            currentIndx = expressionTree.Count - 1;
                        }
                    }
                    else
                    {
                        if (!expressionTree[currentIndx].RightChecked)
                        {
                            if (expressionTree[currentIndx].Operation?.Right is MemberExpression rightMember)
                            {
                                expressionTree[currentIndx].InvokeOrTake<T>(rightMember, true);
                            }

                            if (expressionTree[currentIndx].Operation?.Right is ConstantExpression rightConstant)
                            {
                                expressionTree[currentIndx].MemberValue = rightConstant?.Value;
                            }

                            if (expressionTree[currentIndx].Operation?.Right is BinaryExpression rightBinary)
                            {
                                expressionTree[currentIndx].MemberValue = Expression.Lambda(rightBinary).Compile().DynamicInvoke();
                            }

                            if (expressionTree[currentIndx].Operation?.Right is MethodCallExpression rightMethodMember)
                            {
                                expressionTree[currentIndx].RightMethodMember = rightMethodMember;
                            }

                            expressionTree[currentIndx].RightChecked = true;
                        }

                        if (!expressionTree[currentIndx].LeftChecked)
                        {
                            if (expressionTree[currentIndx].Operation?.Left is MemberExpression leftMember)
                            {
                                expressionTree[currentIndx].InvokeOrTake<T>(leftMember, false);
                            }

                            if (expressionTree[currentIndx].Operation?.Left is ConstantExpression leftConstant)
                            {
                                expressionTree[currentIndx].MemberValue = leftConstant?.Value;
                            }

                            if (expressionTree[currentIndx].Operation?.Left is BinaryExpression leftBinary)
                            {
                                expressionTree[currentIndx].MemberValue = Expression.Lambda(leftBinary).Compile().DynamicInvoke();
                            }

                            if (expressionTree[currentIndx].Operation?.Left is MethodCallExpression leftmethodMember)
                            {
                                expressionTree[currentIndx].LeftMethodMember = leftmethodMember;
                            }

                            expressionTree[currentIndx].LeftChecked = true;
                        }
                    }
                }

                if (expressionTree[currentIndx].MethodData.Count == 0)
                {
                    MethodCallExpression? leftMethodMember = expressionTree[currentIndx].LeftMethodMember;
                    MethodCallExpression? rightMethodMember = expressionTree[currentIndx].RightMethodMember;

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

                        if (!expressionTree[currentIndx].MethodChecked)
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
                                    expressionTree[currentIndx].RightMember = argLambda.Body as MemberExpression;
                                }
                            }

                            if (cleanOfMembers)
                            {
                                if (obj != null)
                                {
                                    object? skata = obj.Type != typeof(string) ? Activator.CreateInstance(obj.Type, null) : string.Empty;
                                    expressionTree[currentIndx].MemberValue = func.Invoke(skata, parameters);
                                }
                            }
                            else
                            {
                                expressionTree[currentIndx].MethodData.Add(new MethodExpressionData 
                                { 
                                    MethodName = func.Name, 
                                    MethodArguments = MethodArguments,
                                    CastedOn = obj ,
                                    ComparedValue = expressionTree[currentIndx].MemberValue,
                                    CompareProperty = expressionTree[currentIndx].RightMember,
                                    OperatorType = expressionTree[currentIndx].OperationType,
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

                        if (!expressionTree[currentIndx].MethodChecked)
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
                                    expressionTree[currentIndx].LeftMember = argLambda.Body as MemberExpression;
                                }
                            }

                            if (cleanOfMembers)
                            {
                                if (obj != null)
                                {
                                    object? skata = obj.Type != typeof(string) ? Activator.CreateInstance(obj.Type, null) : string.Empty;
                                    expressionTree[currentIndx].MemberValue = func.Invoke(skata, parameters);
                                }
                            }
                            else
                            {
                                expressionTree[currentIndx].MethodData.Add(new MethodExpressionData
                                {
                                    MethodName = func.Name,
                                    MethodArguments = MethodArguments,
                                    CastedOn = obj,
                                    ComparedValue = expressionTree[currentIndx].MemberValue,
                                    CompareProperty = expressionTree[currentIndx].LeftMember,
                                    OperatorType = expressionTree[currentIndx].OperationType,
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
                    thisBranch.MemberValue = Expression.Lambda(memberExp).Compile().DynamicInvoke();
                }
                catch
                {
                    if (isRight)
                    {
                        thisBranch.RightMember = memberExp;
                    }
                    else
                    {
                        thisBranch.LeftMember = memberExp;
                    }
                }
            }
            else
            {
                thisBranch.MemberValue = Expression.Lambda(memberExp).Compile().DynamicInvoke();
            }
        }

        internal static ColumnsAndParameters ExpressionTreeToSql(this List<ExpressionsData> data, bool isMyShit, string? letter, List<BlackHoleParameter>? parameters, int index)
        {
            parameters ??= new List<BlackHoleParameter>();

            List<ExpressionsData> children = data.Where(x => x.MemberValue != null || x.MethodData.Count > 0).ToList();
            string[] translations = new string[children.Count];

            foreach (ExpressionsData child in children)
            {
                ExpressionsData parent = data[child.ParentIndex];

                if (child.MethodData.Count > 0)
                {
                    if(child.MemberValue != null && child.MethodData[0].ComparedValue == null  && child.MethodData[0].CompareProperty == null)
                    {
                        child.MethodData[0].ComparedValue = child.MemberValue;
                    }

                    SqlFunctionResult sqlFunctionResult = child.MethodData[0].TranslateBHMethod(index, letter, isMyShit, string.Empty);

                    if (sqlFunctionResult.ParamName != string.Empty)
                    {
                        parameters.Add(new BlackHoleParameter { Name = sqlFunctionResult.ParamName, Value = sqlFunctionResult.Value });
                        index++;
                    }

                    if(parent.SqlCommand == string.Empty)
                    {
                        parent.SqlCommand = $"{sqlFunctionResult.SqlCommand}";
                    }
                    else
                    {
                        parent.SqlCommand += $" and {sqlFunctionResult.SqlCommand}";
                    }

                    parent.LeftChecked = false;
                }
                else
                {
                    if (parent.LeftChecked)
                    {
                        ColumnAndParameter childParams = child.TranslateExpression(index, isMyShit, letter);

                        if (childParams.ParamName != string.Empty)
                        {
                            parameters.Add(new BlackHoleParameter { Name = childParams.ParamName, Value = childParams.Value });
                        }

                        parent.SqlCommand = $"{childParams.Column}";

                        parent.LeftChecked = false;
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

                        parent.SqlCommand = $"({parent.SqlCommand} {parentCols.Column} {childCols.Column})";
                        index++;
                    }
                }
            }

            List<ExpressionsData> parents = data.Where(x => x.MemberValue == null && x.MethodData.Count == 0 && x.OperationType != ExpressionType.Default).ToList();

            if (parents.Count > 1)
            {
                parents.RemoveAt(0);
                int parentsCount = parents.Count;

                for (int i = 0; i < parentsCount; i++)
                {
                    ExpressionsData parent = data[parents[parentsCount - 1 - i].ParentIndex];

                    if (parent.LeftChecked)
                    {
                        parent.SqlCommand = parents[parentsCount - 1 - i].SqlCommand;
                        parent.LeftChecked = false;
                    }
                    else
                    {
                        ColumnAndParameter parentParams = parent.TranslateExpression(index, isMyShit, letter);

                        if (parentParams.ParamName != string.Empty)
                        {
                            parameters.Add(new BlackHoleParameter { Name = parentParams.ParamName, Value = parentParams.Value });
                        }

                        parent.SqlCommand = $"({parent.SqlCommand} {parentParams.Column} {parents[parentsCount - 1 - i].SqlCommand})";

                        index++;
                    }
                }
            }

            return new ColumnsAndParameters { Columns = data[0].SqlCommand, Parameters = parameters, Count = index};
        }

        private static SqlFunctionResult TranslateBHMethod(this MethodExpressionData MethodData, int index, string? letter, bool isMyShit, string schemaName)
        {
            SqlFunctionResult result = new();

            if(MethodData.CastedOn != null)
            {
                int methodType = 2;

                Type CastedOnPropType = MethodData.CastedOn.Type;

                if (MethodData.CastedOn.Type.Name.Contains("Nullable"))
                {
                    if (MethodData.CastedOn.Type.GenericTypeArguments != null && MethodData.CastedOn.Type.GenericTypeArguments.Length > 0)
                    {
                        CastedOnPropType = MethodData.CastedOn.Type.GenericTypeArguments[0];
                    }
                }

                if (CastedOnPropType == typeof(string))
                {
                    methodType = 0;
                }

                if (CastedOnPropType == typeof(DateTime))
                {
                    methodType = 1;
                }

                string[] compareProperty = MethodData.CastedOn.ToString().Split(".");

                if(compareProperty.Length > 1)
                {
                    string selectCommand;
                    string operationType;

                    switch (MethodData.MethodName)
                    {
                        case "SqlMax":
                            selectCommand = $"( Select MAX({compareProperty[1].SkipNameQuotes(isMyShit)}) from {schemaName}{MethodData.TableName.SkipNameQuotes(isMyShit)} )";
                            result.SqlCommand = $" {letter}{compareProperty[1].SkipNameQuotes(isMyShit)} = {selectCommand} ";
                            result.WasTranslated = true;
                            return result;
                        case "SqlMin":
                            selectCommand = $"( Select MIN({compareProperty[1].SkipNameQuotes(isMyShit)}) from {schemaName}{MethodData.TableName.SkipNameQuotes(isMyShit)} )";
                            result.SqlCommand = $" {letter}{compareProperty[1].SkipNameQuotes(isMyShit)} = {selectCommand} ";
                            result.WasTranslated = true;
                            return result;
                    }

                    if(methodType == 0)
                    {
                        if(MethodData.ComparedValue != null)
                        {
                            if (MethodData.MethodArguments.Count == 1 && MethodData.MethodName == "Contains")
                            {
                                result.ParamName = $"{compareProperty[1]}{index}";
                                result.Value = $"%{MethodData.MethodArguments[0]}%";
                                result.SqlCommand = $" {letter}{compareProperty[1].SkipNameQuotes(isMyShit)} Like @{result.ParamName}";
                                result.WasTranslated = true;
                                return result;
                            }

                            if (MethodData.MethodArguments.Count == 2)
                            {
                                switch (MethodData.MethodName)
                                {
                                    case "Replace":
                                        object? first = MethodData.MethodArguments[0];
                                        object? second = MethodData.MethodArguments[1];
                                        operationType = ExpressionTypeToSql(MethodData.OperatorType, MethodData.ReverseOperator, false);
                                        result.ParamName = $"{compareProperty[1]}{index}";
                                        result.Value = MethodData.ComparedValue;
                                        result.SqlCommand = $" REPLACE({letter}{compareProperty[1].SkipNameQuotes(isMyShit)},'{first}','{second}') {operationType} @{result.ParamName} ";
                                        result.WasTranslated = true;
                                        return result;
                                    case "SqlLike":
                                        result.ParamName = $"{compareProperty[1]}{index}";
                                        result.Value = MethodData.MethodArguments[1];
                                        result.SqlCommand = $" {letter}{compareProperty[1].SkipNameQuotes(isMyShit)} Like @{result.ParamName}";
                                        result.WasTranslated = true;
                                        return result;
                                }
                            }

                            switch (MethodData.MethodName)
                            {
                                case "ToUpper":
                                    operationType = ExpressionTypeToSql(MethodData.OperatorType, MethodData.ReverseOperator, false);
                                    result.ParamName = $"{compareProperty[1]}{index}";
                                    result.Value = MethodData.ComparedValue;
                                    result.SqlCommand = $" UPPER({letter}{compareProperty[1].SkipNameQuotes(isMyShit)}) {operationType} @{result.ParamName} ";
                                    result.WasTranslated = true;
                                    return result;
                                case "ToLower":
                                    operationType = ExpressionTypeToSql(MethodData.OperatorType, MethodData.ReverseOperator, false);
                                    result.ParamName = $"{compareProperty[1]}{index}";
                                    result.Value = MethodData.ComparedValue;
                                    result.SqlCommand = $" LOWER({letter}{compareProperty[1].SkipNameQuotes(isMyShit)}) {operationType} @{result.ParamName} ";
                                    result.WasTranslated = true;
                                    return result;
                                case "SqlLength":
                                    operationType = ExpressionTypeToSql(MethodData.OperatorType, !MethodData.ReverseOperator, false);
                                    result.ParamName = $"{compareProperty[1]}{index}";
                                    result.Value = MethodData.ComparedValue;
                                    result.SqlCommand = $" LENGTH({letter}{compareProperty[1].SkipNameQuotes(isMyShit)}) {operationType} @{result.ParamName} ";
                                    result.WasTranslated = true;
                                    return result;
                            }
                        }
                    }

                    if(methodType == 1 && MethodData.MethodArguments.Count == 2)
                    {
                        switch (MethodData.MethodName)
                        {
                            case "SqlDateAfter":
                                result.ParamName = $"DateProp{index}";
                                result.Value = MethodData.MethodArguments[1];
                                result.SqlCommand = $" {letter}{compareProperty[1].SkipNameQuotes(isMyShit)} >= @{result.ParamName} ";
                                result.WasTranslated = true;
                                return result;
                            case "SqlDateBefore":
                                result.ParamName = $"DateProp{index}";
                                result.Value = MethodData.MethodArguments[1];
                                result.SqlCommand = $" {letter}{compareProperty[1].SkipNameQuotes(isMyShit)} < @{result.ParamName} ";
                                result.WasTranslated = true;
                                return result;
                        }
                    }

                    if(methodType == 2)
                    {
                        string decimalPoints = string.Empty;
                        PropertyInfo? thePlusValue = null;

                        if (MethodData.MethodArguments.Count > 1)
                        {
                            decimalPoints = $",{MethodData.MethodArguments[1]}";
                            thePlusValue = MethodData.MethodArguments[1] as PropertyInfo;
                        }

                        if (MethodData.ComparedValue != null)
                        {
                            switch (MethodData.MethodName)
                            {
                                case "SqlAbsolut":
                                    operationType = ExpressionTypeToSql(MethodData.OperatorType, !MethodData.ReverseOperator, false);
                                    result.ParamName = $"{compareProperty[1]}{index}";
                                    result.Value = MethodData.ComparedValue;
                                    result.SqlCommand = $" ABS({letter}{compareProperty[1].SkipNameQuotes(isMyShit)}) {operationType} @{result.ParamName} ";
                                    result.WasTranslated = true;
                                    return result;
                                case "SqlRound":
                                    operationType = ExpressionTypeToSql(MethodData.OperatorType, !MethodData.ReverseOperator, false);
                                    result.ParamName = $"{compareProperty[1]}{index}";
                                    result.Value = MethodData.ComparedValue;
                                    result.SqlCommand = $" ROUND({letter}{compareProperty[1].SkipNameQuotes(isMyShit)}{decimalPoints}) {operationType} @{result.ParamName} ";
                                    result.WasTranslated = true;
                                    return result;
                                case "SqlPlus":
                                    operationType = ExpressionTypeToSql(MethodData.OperatorType, !MethodData.ReverseOperator, false);
                                    if (thePlusValue != null)
                                    {
                                        result.ParamName = $"{compareProperty[1]}{index}";
                                        result.Value = MethodData.ComparedValue;
                                        result.SqlCommand = $" ({letter}{compareProperty[1].SkipNameQuotes(isMyShit)} + {letter}{thePlusValue.Name.SkipNameQuotes(isMyShit)}) {operationType} @{result.ParamName} ";
                                    }
                                    else
                                    {
                                        result.ParamName = $"{compareProperty[1]}{index}";
                                        result.Value = MethodData.ComparedValue;
                                        result.SqlCommand = $" ({letter}{compareProperty[1].SkipNameQuotes(isMyShit)} + {MethodData.MethodArguments[1]}) {operationType} @{result.ParamName} ";
                                    }
                                    result.WasTranslated = true;
                                    return result;
                                case "SqlMinus":
                                    operationType = ExpressionTypeToSql(MethodData.OperatorType, !MethodData.ReverseOperator, false);
                                    if (thePlusValue != null)
                                    {
                                        result.ParamName = $"{compareProperty[1]}{index}";
                                        result.Value = MethodData.ComparedValue;
                                        result.SqlCommand = $" ({letter}{compareProperty[1].SkipNameQuotes(isMyShit)} - {letter}{thePlusValue.Name.SkipNameQuotes(isMyShit)}) {operationType} @{result.ParamName} ";
                                    }
                                    else
                                    {
                                        result.ParamName = $"{compareProperty[1]}{index}";
                                        result.Value = MethodData.ComparedValue;
                                        result.SqlCommand = $" ({letter}{compareProperty[1].SkipNameQuotes(isMyShit)} - {MethodData.MethodArguments[1]}) {operationType} @{result.ParamName} ";
                                    }
                                    result.WasTranslated = true;
                                    break;
                            }
                        }

                        if(MethodData.CompareProperty != null)
                        {
                            string[] otherPropName = MethodData.CompareProperty.ToString().Split(".");

                            if(otherPropName.Length > 1)
                            {
                                switch (MethodData.MethodName)
                                {
                                    case "SqlAverage":
                                        operationType = ExpressionTypeToSql(MethodData.OperatorType, MethodData.ReverseOperator, false);
                                        string SelectAverage = $"( Select AVG({compareProperty[1].SkipNameQuotes(isMyShit)}) from {schemaName}{MethodData.TableName.SkipNameQuotes(isMyShit)} )";
                                        result.SqlCommand = $" {letter}{otherPropName[1].SkipNameQuotes(isMyShit)} {operationType} {SelectAverage} ";
                                        result.WasTranslated = true;
                                        return result;
                                    case "SqlAbsolut":
                                        operationType = ExpressionTypeToSql(MethodData.OperatorType, !MethodData.ReverseOperator, false);
                                        result.SqlCommand = $" ABS({letter}{compareProperty[1].SkipNameQuotes(isMyShit)}) {operationType} {letter}{otherPropName[1].SkipNameQuotes(isMyShit)} ";
                                        result.WasTranslated = true;
                                        return result;
                                    case "SqlRound":
                                        operationType = ExpressionTypeToSql(MethodData.OperatorType, !MethodData.ReverseOperator, false);
                                        result.SqlCommand = $" ROUND({letter}{compareProperty[1].SkipNameQuotes(isMyShit)}{decimalPoints}) {operationType} {letter}{otherPropName[1].SkipNameQuotes(isMyShit)} ";
                                        result.WasTranslated = true;
                                        break;
                                    case "SqlPlus":
                                        operationType = ExpressionTypeToSql(MethodData.OperatorType, !MethodData.ReverseOperator, false);
                                        if (thePlusValue != null)
                                        {
                                            result.SqlCommand = $" ({letter}{compareProperty[1].SkipNameQuotes(isMyShit)} + {letter}{thePlusValue.Name.SkipNameQuotes(isMyShit)}) {operationType} {letter}{otherPropName[1].SkipNameQuotes(isMyShit)} ";
                                        }
                                        else
                                        {
                                            result.SqlCommand = $" ({letter}{compareProperty[1].SkipNameQuotes(isMyShit)} + {MethodData.MethodArguments[1]}) {operationType} {letter}{otherPropName[1].SkipNameQuotes(isMyShit)} ";
                                        }
                                        result.WasTranslated = true;
                                        return result;
                                    case "SqlMinus":
                                        operationType = ExpressionTypeToSql(MethodData.OperatorType, !MethodData.ReverseOperator, false);
                                        if (thePlusValue != null)
                                        {
                                            result.SqlCommand = $" ({letter}{compareProperty[1].SkipNameQuotes(isMyShit)} - {letter}{thePlusValue.Name.SkipNameQuotes(isMyShit)}) {operationType} {letter}{otherPropName[1].SkipNameQuotes(isMyShit)} ";
                                        }
                                        else
                                        {
                                            result.SqlCommand = $" ({letter}{compareProperty[1].SkipNameQuotes(isMyShit)} - {MethodData.MethodArguments[1]}) {operationType} {letter}{otherPropName[1].SkipNameQuotes(isMyShit)} ";
                                        }
                                        result.WasTranslated = true;
                                        break;
                                }
                            }
                        }
                    }

                    if (MethodData.MethodArguments.Count == 2 && MethodData.CompareProperty != null)
                    {
                        string? aTable = MethodData.CompareProperty.Member?.ReflectedType?.Name;
                        string? PropertyName = MethodData.CompareProperty.Member?.Name;

                        if (aTable != null && PropertyName != null)
                        {
                            switch (MethodData.MethodName)
                            {
                                case "SqlEqualTo":
                                    result.ParamName = $"OtherId{index}";
                                    result.Value = MethodData.MethodArguments[1];
                                    selectCommand = $"( Select {PropertyName.SkipNameQuotes(isMyShit)} from {schemaName}{aTable.SkipNameQuotes(isMyShit)} where {"Id".SkipNameQuotes(isMyShit)}= @{result.ParamName} )";
                                    result.SqlCommand = $" {letter}{compareProperty[1].SkipNameQuotes(isMyShit)} = {selectCommand} ";
                                    result.WasTranslated = true;
                                    return result;
                                case "SqlGreaterThan":
                                    result.ParamName = $"OtherId{index}";
                                    result.Value = MethodData.MethodArguments[1];
                                    selectCommand = $"( Select {PropertyName.SkipNameQuotes(isMyShit)} from {schemaName}{aTable.SkipNameQuotes(isMyShit)} where {"Id".SkipNameQuotes(isMyShit)}= @{result.ParamName} )";
                                    result.SqlCommand = $" {letter}{compareProperty[1].SkipNameQuotes(isMyShit)} > {selectCommand} ";
                                    result.WasTranslated = true;
                                    return result;
                                case "SqlLessThan":
                                    result.ParamName = $"OtherId{index}";
                                    result.Value = MethodData.MethodArguments[1];
                                    selectCommand = $"( Select {PropertyName.SkipNameQuotes(isMyShit)} from {schemaName}{aTable.SkipNameQuotes(isMyShit)} where {"Id".SkipNameQuotes(isMyShit)}= @{result.ParamName} )";
                                    result.SqlCommand = $" {letter}{compareProperty[1].SkipNameQuotes(isMyShit)} < {selectCommand} ";
                                    result.WasTranslated = true;
                                    return result;
                            }
                        }
                    }
                }
            }

            if (!result.WasTranslated)
            {
                Task.Factory.StartNew(() => MethodData.MethodName.CreateErrorLogs($"SqlFunctionsReader_{MethodData.MethodName}",
                    $"Unknown Method: {MethodData.MethodName}. It was translated into '1 != 1' to avoid data corruption.",
                    "Please read the documentation to find the supported Sql functions and the correct usage of them."));
            }

            return result;
        }

        private static string ExpressionTypeToSql(this ExpressionType ExpType, bool IsReversed, bool IsNullValue)
        {
            return ExpType switch
            {
                ExpressionType.Equal => IsNullValue ? "is" : "=",
                ExpressionType.NotEqual => IsNullValue ? "is not" : "!=",
                ExpressionType.GreaterThan => IsReversed ? "<" : ">",
                ExpressionType.LessThan => IsReversed ? ">" : "<",
                ExpressionType.GreaterThanOrEqual => IsReversed ? "<=" : ">=",
                ExpressionType.LessThanOrEqual => IsReversed ? ">=" : "<=",
                _ => string.Empty,
            };
        }

        private static ColumnAndParameter TranslateExpression(this ExpressionsData expression, int index, bool isMyShit, string? letter)
        {
            string? column = string.Empty;
            string? parameter = string.Empty;
            object? value = new();
            string[]? variable = new string[2];
            string subLetter = letter != string.Empty ? $"{letter}." : string.Empty;
            string sqlOperator = "=";

            switch (expression.OperationType)
            {
                case ExpressionType.AndAlso:
                    column = " and ";
                    break;
                case ExpressionType.OrElse:
                    column = " or ";
                    break;
                case ExpressionType.Equal:
                    value = expression?.MemberValue;
                    variable = expression?.LeftMember != null ? expression?.LeftMember.ToString().Split(".") : expression?.RightMember?.ToString().Split(".");
                    column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} = @{variable?[1]}{index}";
                    if (value == null)
                    {
                        column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} is @{variable?[1]}{index}";
                    }
                    parameter = $"{variable?[1]}{index}";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    if(expression?.LeftMember != null)
                    {
                        variable = expression?.LeftMember.ToString().Split(".");
                        sqlOperator = ">=";
                    }
                    else
                    {
                        variable = expression?.RightMember?.ToString().Split(".");
                        sqlOperator = "<=";
                    }
                    column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} {sqlOperator} @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.MemberValue;
                    break;
                case ExpressionType.LessThanOrEqual:
                    if (expression?.LeftMember != null)
                    {
                        variable = expression?.LeftMember.ToString().Split(".");
                        sqlOperator = "<=";
                    }
                    else
                    {
                        variable = expression?.RightMember?.ToString().Split(".");
                        sqlOperator = ">=";
                    }
                    column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} {sqlOperator} @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.MemberValue;
                    break;
                case ExpressionType.LessThan:
                    if (expression?.LeftMember != null)
                    {
                        variable = expression?.LeftMember.ToString().Split(".");
                        sqlOperator = "<";
                    }
                    else
                    {
                        variable = expression?.RightMember?.ToString().Split(".");
                        sqlOperator = ">";
                    }
                    column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} {sqlOperator} @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.MemberValue;
                    break;
                case ExpressionType.GreaterThan:
                    if (expression?.LeftMember != null)
                    {
                        variable = expression?.LeftMember.ToString().Split(".");
                        sqlOperator = ">";
                    }
                    else
                    {
                        variable = expression?.RightMember?.ToString().Split(".");
                        sqlOperator = "<";
                    }
                    column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} {sqlOperator} @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.MemberValue;
                    break;
                case ExpressionType.NotEqual:
                    value = expression?.MemberValue;
                    variable = expression?.LeftMember != null ? expression?.LeftMember.ToString().Split(".") : expression?.RightMember?.ToString().Split(".");
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

                data.IsMyShit = true;
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

                data.Joins += $" {joinType} join {schemaName}{typeof(TOther).Name.SkipNameQuotes(data.IsMyShit)} {parameterOther} on {parameterOther}.{propNameOther.SkipNameQuotes(data.IsMyShit)} = {parameter}.{propName.SkipNameQuotes(data.IsMyShit)}";
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

        private static void BindPropertiesToDtoExtension(this JoinsData data, Type tableType, string? paramB)
        {
            List<string> OtherPropNames = tableType.GetProperties().Select(x => x.Name).ToList();

            for(int i = 0 ; i < data.OccupiedDtoProps.Count; i++)
            {
                PropertyOccupation property = data.OccupiedDtoProps[i];

                if (OtherPropNames.Contains(property.PropName) && !property.Occupied)
                {
                    Type? TOtherPropType = tableType.GetProperty(property.PropName)?.PropertyType;

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

                data.Joins += $" {additionalType} {secondLetter}.{propNameOther.SkipNameQuotes(data.IsMyShit)} = {firstLetter}.{propName.SkipNameQuotes(data.IsMyShit)}";
            }
        }

        internal static void WhereJoin<TSource>(this JoinsData data, Expression<Func<TSource, bool>> predicate)
        {
            if (!data.Ignore)
            {
                string? letter = data.TablesToLetters.First(x => x.Table == typeof(TSource)).Letter;
                ColumnsAndParameters colsAndParams = predicate.Body.SplitMembers<TSource>(data.IsMyShit, letter, data.DynamicParams, data.ParamsCount);
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

        internal static ColumnsAndParameters? TranslateJoin<Dto>(this JoinsData data) where Dto : BHDtoIdentifier
        {
            if (data.DtoType == typeof(Dto))
            {
                data.RejectInactiveEntities();
                TableLetters? tL = data.TablesToLetters.FirstOrDefault(x => x.Table == data.BaseTable);
                string schemaName = GetDatabaseSchema();
                string commandText = $"{data.BuildCommand()} from {schemaName}{tL?.Table?.Name.SkipNameQuotes(data.IsMyShit)} {tL?.Letter} {data.Joins} {data.WherePredicates} {data.OrderByOptions}";
                return new ColumnsAndParameters { Columns = commandText, Parameters = data.DynamicParams };
            }
            return null;
        }

        internal static string OrderByToSql<T>(this BHOrderBy<T> orderByConfig, bool isMyShit)
        {
            if (orderByConfig.OrderBy.LockedByError)
            {
                return string.Empty;
            }

            StringBuilder orderby = new();
            string limiter = string.Empty;

            foreach (OrderByPair pair in orderByConfig.OrderBy.OrderProperties)
            {
                orderby.Append($", {pair.PropertyName.SkipNameQuotes(isMyShit)} {pair.Orientation}");
            }

            if (orderByConfig.OrderBy.TakeSpecificRange)
            {
                limiter = orderByConfig.OrderBy.RowsLimiter();
            }

            return $"order by{orderby.ToString().Remove(0, 1)}{limiter}";
        }

        internal static void OrderByToSqlJoins<T>(this JoinsData data, BHOrderBy<T> orderByConfig)
        {
            if (orderByConfig.OrderBy.LockedByError)
            {
                data.OrderByOptions = string.Empty;
            }
            else
            {
                StringBuilder orderby = new();
                string limiter = string.Empty;
                int counter = 0;

                foreach (OrderByPair pair in orderByConfig.OrderBy.OrderProperties)
                {
                    if (data.OccupiedDtoProps.FirstOrDefault(x => x.PropName == pair.PropertyName) is PropertyOccupation occupation)
                    {
                        counter++;
                        orderby.Append($", {occupation.TableLetter}.{occupation.TableProperty.SkipNameQuotes(data.IsMyShit)} {pair.Orientation}");
                    }
                }

                if (orderByConfig.OrderBy.TakeSpecificRange)
                {
                    limiter = orderByConfig.OrderBy.RowsLimiter();
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

                    command += $" {anD} {table.Letter}.{inactiveColumn.SkipNameQuotes(data.IsMyShit)} = 0 ";
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
                    1 => $" {prop.TableLetter}.{prop.TableProperty.SkipNameQuotes(data.IsMyShit)} as {prop.PropName.SkipNameQuotes(data.IsMyShit)},",
                    2 => $" cast({prop.TableLetter}.{prop.TableProperty.SkipNameQuotes(data.IsMyShit)} as {prop.PropType.SqlTypeFromType()}) as {prop.PropName.SkipNameQuotes(data.IsMyShit)},",
                    _ => $" {prop.TableLetter}.{prop.PropName.SkipNameQuotes(data.IsMyShit)},",
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
            int allow;
            int defAllow = 2;

            string typeToType = firstType.Name + secondType.Name;

            if (firstType == typeof(Guid))
            {
                BlackHoleSqlTypes sqlType = DatabaseStatics.DatabaseType;
                if (sqlType != BlackHoleSqlTypes.Postgres && sqlType != BlackHoleSqlTypes.SqlServer)
                {
                    defAllow = 1;
                }
            }

            if (firstType.Name != secondType.Name)
            {
                allow = typeToType switch
                {
                    "Int16Int32" or "Int16String" or "Int16Int64" or "Int32String" or "Int32Int64" or
                    "Int64String" or "DecimalString" or "DecimalDouble" or "SingleString" or "SingleDouble" or
                    "SingleDecimal" or "DoubleString" or "Int32Decimal" or "GuidString" or "Int32Double" or
                    "BooleanString" or "BooleanInt32" or "Byte[]String" or "DateTimeString" => defAllow,
                    _ => 0,
                };
            }
            else
            {
                allow = 1;
            }

            return allow;
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
