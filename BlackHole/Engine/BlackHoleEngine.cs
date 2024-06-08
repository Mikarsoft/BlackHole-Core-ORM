using BlackHole.Core;
using BlackHole.DataProviders;
using BlackHole.Entities;
using BlackHole.Enums;
using BlackHole.Identifiers;
using BlackHole.Statics;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace BlackHole.Engine
{
    internal static class BlackHoleEngine
    {
        private static IDataProvider[] DataProviders { get; set; } = new IDataProvider[0];
        private static SHA1? Sh { get; set; }

        internal static void InitializeEngine(this int pvCount)
        {
            DataProviders = new IDataProvider[pvCount];
            Sh = SHA1.Create();
        }

        internal static bool SetId<G>(this BHEntityAI<G> entity, G? id) where G : IComparable<G>
        {
            if (id != null)
            {
                entity.Id = id;

                if (typeof(G) == typeof(int))
                {
                    object value = id;
                    return (int)value != 0;
                }

                if (typeof(G) == typeof(Guid))
                {
                    object value = id;
                    return (Guid)value != Guid.Empty;
                }

                if (typeof(G) == typeof(string))
                {
                    object value = id;
                    return (string)value != string.Empty;
                }
            }
            return false;
        }

        internal static int SetIndex(this int providerIndex, int providerNewIndex)
        {
            if(providerNewIndex > -1 && providerNewIndex < DataProviders.Length)
            {
                return providerNewIndex;
            }

            return providerIndex;
        }

        internal static int GetConnectionIndexByIdentity(this string dbIdentity)
        {
            return WormHoleData.DatabaseIdentities.IndexOf(dbIdentity);
        }

        internal static IDataProvider GetDataProvider(this int providerIndex)
        {
            if (DataProviders[providerIndex] != null)
            {
                return DataProviders[providerIndex];
            }

            DataProviders[providerIndex] = WormHoleData.DbTypes[providerIndex] switch
            {
                BlackHoleSqlTypes.SqlServer => new SqlServerDataProvider(WormHoleData.ConnectionStrings[providerIndex], WormHoleData.IsQuotedDb[providerIndex]),
                BlackHoleSqlTypes.MySql => new MySqlDataProvider(WormHoleData.ConnectionStrings[providerIndex]),
                BlackHoleSqlTypes.Postgres => new PostgresDataProvider(WormHoleData.ConnectionStrings[providerIndex]),
                BlackHoleSqlTypes.SqlLite => new SqLiteDataProvider(WormHoleData.ConnectionStrings[providerIndex], WormHoleData.IsQuotedDb[providerIndex]),
                _ => new OracleDataProvider(WormHoleData.ConnectionStrings[providerIndex]),
            };

            return DataProviders[providerIndex];
        }

        internal static IDbConnection GetConnection(this int providerIndex)
        {
            if (DataProviders[providerIndex] != null)
            {
                return DataProviders[providerIndex].GetConnection();
            }

            DataProviders[providerIndex] = WormHoleData.DbTypes[providerIndex] switch
            {
                BlackHoleSqlTypes.SqlServer => new SqlServerDataProvider(WormHoleData.ConnectionStrings[providerIndex], WormHoleData.IsQuotedDb[providerIndex]),
                BlackHoleSqlTypes.MySql => new MySqlDataProvider(WormHoleData.ConnectionStrings[providerIndex]),
                BlackHoleSqlTypes.Postgres => new PostgresDataProvider(WormHoleData.ConnectionStrings[providerIndex]),
                BlackHoleSqlTypes.SqlLite => new SqLiteDataProvider(WormHoleData.ConnectionStrings[providerIndex], WormHoleData.IsQuotedDb[providerIndex]),
                _ => new OracleDataProvider(WormHoleData.ConnectionStrings[providerIndex]),
            };

            return DataProviders[providerIndex].GetConnection();
        }

        internal static int GetAvailableConnections()
        {
            return WormHoleData.ConnectionStrings.Length;
        }

        internal static EntityInfo SwitchBlackHoleMode(this Type entityType, DatabaseRole dbRole)
        {
            EntityInfo entityConfig = new();
            int entityInfoIndex;
            byte[] entityCode;

            switch (WormHoleData.BlackHoleMode)
            {
                case BHMode.MultiSchema:
                    entityConfig.EntitySchema = entityType.GetEntitySchema(0);
                    entityConfig.DBTIndex = 0;
                    return entityConfig;

                case BHMode.HighAvailability:
                    int IndexOfMainDb = Array.IndexOf(WormHoleData.DatabaseRoles, dbRole);
                    entityConfig.EntitySchema = string.Empty;
                    entityConfig.DBTIndex = IndexOfMainDb > -1? IndexOfMainDb : 0;
                    return entityConfig;

                case BHMode.Multiple:
                    entityCode = ComputeByteHash($"{entityType.Name}_{entityType.Namespace}_{entityType.Assembly.FullName}");
                    entityInfoIndex = Array.IndexOf(WormHoleData.EntitiesCodes, entityCode);
                    entityConfig.DBTIndex = entityInfoIndex > -1 ? WormHoleData.EntityTargetDb[entityInfoIndex] : 0;
                    entityConfig.EntitySchema = entityType.GetEntitySchema(entityConfig.DBTIndex);
                    return entityConfig;

                default:
                    entityConfig.EntitySchema = WormHoleData.DbSchemas[0];
                    entityConfig.DBTIndex = 0;
                    return entityConfig;
            }
        }

        private static byte[] ComputeByteHash(this string text)
        {
            if(Sh == null)
            {
                Sh = SHA1.Create();
            }

            byte[] bytes = Encoding.ASCII.GetBytes(text);
            return Sh.ComputeHash(bytes);
        }

        internal static string GetEntitySchema(this Type entityType, int connectionIndex)
        {
            if (WormHoleData.IsSchemaFromNamespace)
            {
                if(entityType.Namespace?.Split(".") is string[] NmsParts)
                {
                    return NmsParts[NmsParts.Length - 1];
                }
            }

            if (WormHoleData.IsSchemaFromAssembly)
            {
                if (entityType.Namespace?.Split(".") is string[] AsmParts)
                {
                    return AsmParts[AsmParts.Length - 1];
                }
            }

            return WormHoleData.DbSchemas[connectionIndex];
        }

        internal static EntityContext GetEntityContext<G>(this Type entityType)
        {
            EntityInfo entityInfo = entityType.SwitchBlackHoleMode(DatabaseRole.Master);

            EntityContext entityContext = new EntityContext
            {
                ConnectionIndex = entityInfo.DBTIndex,
                DatabaseType = WormHoleData.DbTypes[entityInfo.DBTIndex],
                IsQuotedDb = WormHoleData.IsQuotedDb[entityInfo.DBTIndex],
                ThisSchema = entityInfo.EntitySchema
            };

            entityContext.MapEntityContext<G>(entityType);
            return entityContext;
        }

        internal static void MapEntityContext<G>(this EntityContext entityContext, Type entityType)
        {
            string entityId = "Id";
            string inactiveColumn = "Inactive";
            string? TableName = entityType.GetTableDisplayName(entityContext.IsQuotedDb);

            entityContext.ReturningId = entityContext.DatabaseType.GetReturningIdCommand(TableName);
            entityContext.ThisTable = $"{entityContext.ThisSchema}{TableName}";
            entityContext.ThisId = entityId.UseNameQuotes(entityContext.IsQuotedDb);
            entityContext.ThisInactive = inactiveColumn.UseNameQuotes(entityContext.IsQuotedDb);
            entityContext.WithActivator = entityType.CheckActivator();
            entityContext.UseIdGenerator = entityContext.DatabaseType.DetermineUseGenerator<G>();

            using (TripleStringBuilder sb = new())
            {
                foreach (PropertyInfo prop in entityType.GetProperties())
                {
                    if (prop.Name != inactiveColumn)
                    {
                        if (prop.Name != "Id")
                        {
                            string property = prop.Name.UseNameQuotes(entityContext.IsQuotedDb);
                            sb.PNSb.Append($", {property}");
                            sb.PPSb.Append($", @{prop.Name}");
                            sb.UPSb.Append($",{property} = @{prop.Name}");
                        }
                        entityContext.Columns.Add(prop.Name);
                    }
                }
                entityContext.PropertyNames = $"{sb.PNSb.ToString().Remove(0, 1)} ";
                entityContext.PropertyParams = $"{sb.PPSb.ToString().Remove(0, 1)} ";
                entityContext.UpdateParams = $"{sb.UPSb.ToString().Remove(0, 1)} ";
            }
        }

        internal static OpenEntityContext GetEntityContext<T>(this EntitySettings<T> entitySettings)
        {
            Type entityType = typeof(T);

            EntityInfo entityInfo = entityType.SwitchBlackHoleMode(DatabaseRole.Master);

            OpenEntityContext entityContext = new OpenEntityContext
            {
                ConnectionIndex = entityInfo.DBTIndex,
                DatabaseType = WormHoleData.DbTypes[entityInfo.DBTIndex],
                IsQuotedDb = WormHoleData.IsQuotedDb[entityInfo.DBTIndex],
                ThisSchema = entityInfo.EntitySchema,
                MainPrimaryKey = entitySettings.MainPrimaryKey,
                PKPropertyNames = entitySettings.PKPropertyNames,
                AutoGeneratedColumns = entitySettings.AutoGeneratedColumns,
                HasAutoIncrement = entitySettings.HasAutoIncrement
            };

            entityContext.MapOpenEntityContext<T>();
            return entityContext;
        }

        internal static void MapOpenEntityContext<T>(this OpenEntityContext entityContext)
        {
            Type entityType = typeof(T);
            entityContext.Tprops = entityType.GetProperties();

            string? TableName = entityType.GetTableDisplayName(entityContext.IsQuotedDb);
            entityContext.ThisTable = $"{entityContext.ThisSchema}{TableName}";

            if (entityContext.HasAutoIncrement)
            {
                entityContext.MainPK = entityContext.MainPrimaryKey.UseNameQuotes(entityContext.IsQuotedDb);
            }

            using (TripleStringBuilder sb = new())
            {
                foreach (PropertyInfo prop in entityContext.Tprops)
                {
                    string property = prop.Name.UseNameQuotes(entityContext.IsQuotedDb);

                    if (entityContext.HasAutoIncrement)
                    {
                        if (prop.Name != entityContext.MainPrimaryKey)
                        {
                            sb.PNSb.Append($", {property}");
                            sb.PPSb.Append($", @{prop.Name}");
                        }
                        else
                        {
                            entityContext.GetReturningPrimaryKey(property);
                        }
                    }
                    else
                    {
                        sb.PNSb.Append($", {property}");
                        sb.PPSb.Append($", @{prop.Name}");
                    }

                    if (!entityContext.PKPropertyNames.Contains(prop.Name))
                    {
                        sb.UPSb.Append($",{property} = @{prop.Name}");
                    }

                    entityContext.Columns.Add(prop.Name);
                }
                entityContext.PropertyNames = $"{sb.PNSb.ToString().Remove(0, 1)} ";
                entityContext.PropertyParams = $"{sb.PPSb.ToString().Remove(0, 1)} ";
                entityContext.UpdateParams = $"{sb.UPSb.ToString().Remove(0, 1)} ";
            }
        }

        private static bool DetermineUseGenerator<G>(this BlackHoleSqlTypes sqlType)
        {
            if(typeof(G) == typeof(string))
            {
                return true;
            }

            if(typeof(G) == typeof(Guid) && sqlType != BlackHoleSqlTypes.SqlServer && sqlType != BlackHoleSqlTypes.Postgres)
            {
                return true;
            }

            return false;
        }

        private static string GetReturningIdCommand(this BlackHoleSqlTypes sqlType, string? tableName)
        {
            return sqlType switch
            {
                BlackHoleSqlTypes.SqlServer => "output Inserted.Id",
                BlackHoleSqlTypes.MySql => "SELECT LAST_INSERT_ID();",
                BlackHoleSqlTypes.Oracle => @"returning ""Id"" into :Id",
                BlackHoleSqlTypes.Postgres => $@"returning ""{tableName}"".""Id""",
                _ => "returning Id",
            };
        }

        internal static void GetReturningPrimaryKey(this OpenEntityContext entityContext, string MainColumn)
        {
            if (entityContext.HasAutoIncrement)
            {
                entityContext.ReturningCase = entityContext.DatabaseType switch
                {
                    BlackHoleSqlTypes.SqlServer => new string[] { $"output Inserted.{MainColumn}", "" },
                    BlackHoleSqlTypes.MySql => new string[] { "", ";SELECT LAST_INSERT_ID();" },
                    BlackHoleSqlTypes.Postgres => new string[] { "", $"returning {entityContext.ThisTable}.{MainColumn}" },
                    BlackHoleSqlTypes.SqlLite => new string[] { "", $"returning {MainColumn}" },
                    _ => new string[] { "", $"returning {MainColumn} into :OracleReturningValue" },
                };
            }
            else
            {
                entityContext.ReturningCase = new string[2];
            }
        }

        internal static string[] GetLimiter(this int rowsCount)
        {
            return BHStaticSettings.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => new string[] { $" TOP {rowsCount} ", "" },
                BlackHoleSqlTypes.MySql => new string[] { "", $" limit {rowsCount} " },
                BlackHoleSqlTypes.Postgres => new string[] { "", $" limit {rowsCount} " },
                BlackHoleSqlTypes.SqlLite => new string[] { "", $" limit {rowsCount} " },
                _ => new string[] { "", $" and rownum <= {rowsCount} " },
            };
        }

        private static bool CheckActivator(this Type entity)
        {
            return entity.GetCustomAttributes(true).Any(x => x.GetType() == typeof(UseActivator));
        }

        private static string? GetTableDisplayName(this Type entity, bool isQuotedDb)
        {
            Type tableDisplayAttr = typeof(TableDisplayName);
            if(entity.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == tableDisplayAttr) is TableDisplayName displayName)
            {
                object? tableName = tableDisplayAttr.GetProperty("TableName")?.GetValue(displayName, null);

                if(tableName is string tbN)
                {
                    return tbN.UseNameQuotes(isQuotedDb);
                }
            }
            return entity.Name.UseNameQuotes(isQuotedDb);
        }

        internal static ColumnsAndParameters SplitMembers<T>(this Expression<Func<T,bool>> fullexpression, bool isQuotedDb, string? letter,
            List<BlackHoleParameter>? DynamicParams, int index, string schemaName, int connectionIndex)
        {
            List<ExpressionsData> expressionTree = new();

            BinaryExpression? currentOperation = null;
            MethodCallExpression? methodCallOperation = null;

            if (fullexpression.Body is BinaryExpression bExp)
            {
                currentOperation = bExp;
            }

            if (fullexpression.Body is MethodCallExpression mcExp)
            {
                methodCallOperation = mcExp;
            }

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
            else if (fullexpression.Body is UnaryExpression unExpr)
            {
                if (unExpr.Operand is MemberExpression mExpr)
                {
                    MemberExpression Exp = mExpr;
                    ExpressionType ExpType = ExpressionType.NotEqual;

                    expressionTree.Add(new ExpressionsData()
                    {
                        Operation = null,
                        LeftMethodMember = null,
                        OperationType = ExpType,
                        RightChecked = false,
                        LeftChecked = false,
                        MemberValue = true,
                        ParentIndex = currentIndx,
                        LeftMember = Exp,
                    });
                }
            }
            else if (fullexpression.Body is MemberExpression memberExpr)
            {
                ExpressionType ExpType = ExpressionType.Equal;

                expressionTree.Add(new ExpressionsData()
                {
                    Operation = null,
                    LeftMethodMember = null,
                    OperationType = ExpType,
                    RightChecked = false,
                    LeftChecked = false,
                    MemberValue = true,
                    ParentIndex = currentIndx,
                    LeftMember = memberExpr
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

                        if (!expressionTree[currentIndx].LeftChecked)
                        {
                            if (leftOperation != null || leftCallOperation != null)
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
                            else if (expressionTree[currentIndx].Operation?.Left is UnaryExpression uExp)
                            {
                                if (uExp.Operand is MemberExpression mExp)
                                {
                                    MemberExpression leftExp = mExp;
                                    ExpressionType leftExpType = ExpressionType.Equal;

                                    expressionTree.Add(new ExpressionsData()
                                    {
                                        Operation = leftOperation,
                                        LeftMethodMember = leftCallOperation,
                                        OperationType = leftExpType,
                                        RightChecked = false,
                                        LeftChecked = false,
                                        MemberValue = true,
                                        ParentIndex = currentIndx,
                                        LeftMember = leftExp,
                                    });
                                    expressionTree[currentIndx].LeftChecked = true;
                                    addTotree = true;
                                }
                            }
                            else if (expressionTree[currentIndx].LeftMember != null)
                            {
                                MemberExpression? leftExp = expressionTree[currentIndx].LeftMember;
                                ExpressionType leftExpType = ExpressionType.Equal;

                                expressionTree.Add(new ExpressionsData()
                                {
                                    Operation = leftOperation,
                                    LeftMethodMember = leftCallOperation,
                                    OperationType = leftExpType,
                                    RightChecked = false,
                                    LeftChecked = false,
                                    MemberValue = true,
                                    ParentIndex = currentIndx,
                                    LeftMember = leftExp,
                                });
                                expressionTree[currentIndx].LeftChecked = true;
                                addTotree = true;
                            }
                        }

                        if (!expressionTree[currentIndx].RightChecked)
                        {
                            if (rightOperation != null || rightCallOperation != null)
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
                            else if (expressionTree[currentIndx].Operation?.Right is UnaryExpression uExp)
                            {
                                if (uExp.Operand is MemberExpression mExp)
                                {
                                    MemberExpression rightExp = mExp;
                                    ExpressionType rightExpType = ExpressionType.NotEqual;

                                    expressionTree.Add(new ExpressionsData()
                                    {
                                        Operation = rightOperation,
                                        LeftMethodMember = rightCallOperation,
                                        OperationType = rightExpType,
                                        RightChecked = false,
                                        LeftChecked = false,
                                        MemberValue = true,
                                        ParentIndex = currentIndx,
                                        LeftMember = rightExp
                                    });
                                    expressionTree[currentIndx].RightChecked = true;
                                    addTotree = true;
                                }
                            }
                            else if (expressionTree[currentIndx].RightMember != null)
                            {
                                MemberExpression? rightExp = expressionTree[currentIndx].RightMember;
                                ExpressionType rightExpType = ExpressionType.Equal;

                                expressionTree.Add(new ExpressionsData()
                                {
                                    Operation = rightOperation,
                                    LeftMethodMember = rightCallOperation,
                                    OperationType = rightExpType,
                                    RightChecked = false,
                                    LeftChecked = false,
                                    MemberValue = true,
                                    ParentIndex = currentIndx,
                                    LeftMember = rightExp
                                });
                                expressionTree[currentIndx].RightChecked = true;
                                addTotree = true;
                            }
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
                                expressionTree[currentIndx].IsNullValue = rightConstant?.Value == null;
                            }

                            if (expressionTree[currentIndx].Operation?.Right is BinaryExpression rightBinary)
                            {
                                expressionTree[currentIndx].MemberValue = Expression.Lambda(rightBinary).Compile().DynamicInvoke();
                                expressionTree[currentIndx].IsNullValue = expressionTree[currentIndx].MemberValue == null;
                            }

                            if (expressionTree[currentIndx].Operation?.Right is MethodCallExpression rightmethodMember)
                            {
                                expressionTree[currentIndx].RightMethodMember = rightmethodMember;
                            }

                            if (expressionTree[currentIndx].Operation?.Right is UnaryExpression unaryMember)
                            {
                                expressionTree[currentIndx].MemberValue = Expression.Lambda(unaryMember).Compile().DynamicInvoke();
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
                                expressionTree[currentIndx].IsNullValue = leftConstant?.Value == null;
                            }

                            if (expressionTree[currentIndx].Operation?.Left is BinaryExpression leftBinary)
                            {
                                expressionTree[currentIndx].MemberValue = Expression.Lambda(leftBinary).Compile().DynamicInvoke();
                                expressionTree[currentIndx].IsNullValue = expressionTree[currentIndx].MemberValue == null;
                            }

                            if (expressionTree[currentIndx].Operation?.Left is MethodCallExpression leftmethodMember)
                            {
                                expressionTree[currentIndx].LeftMethodMember = leftmethodMember;
                            }

                            if (expressionTree[currentIndx].Operation?.Left is UnaryExpression unaryMember)
                            {
                                expressionTree[currentIndx].MemberValue = Expression.Lambda(unaryMember).Compile().DynamicInvoke();
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

                                if (arguments[i] is LambdaExpression argLambda)
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
                                    CastedOn = obj,
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

            return expressionTree.ExpressionTreeToSql<T>(isQuotedDb, letter, DynamicParams, index, schemaName, connectionIndex);
        }

        private static void InvokeOrTake<T>(this ExpressionsData thisBranch, MemberExpression memberExp, bool isRight)
        {
            string? typeName = memberExp.Member.ReflectedType?.FullName;

            if (typeName != null && (typeName == typeof(T).BaseType?.FullName || typeName == typeof(T).FullName))
            {
                try
                {
                    var lExp = Expression.Lambda(memberExp);

                    if (lExp.Parameters.Count > 0)
                    {
                        thisBranch.MemberValue = lExp.Compile().DynamicInvoke();
                        thisBranch.IsNullValue = thisBranch.MemberValue == null;
                    }
                    else
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
                thisBranch.IsNullValue = thisBranch.MemberValue == null;
            }
        }

        internal static ColumnsAndParameters ExpressionTreeToSql<T>(this List<ExpressionsData> data, bool isQuotedDb, string? letter,
            List<BlackHoleParameter>? parameters, int index, string schemaName, int connectionIndex)
        {
            parameters ??= new List<BlackHoleParameter>();

            bool HAMode = false;

            if (WormHoleData.BlackHoleMode == BHMode.HighAvailability)
            {
                HAMode = true;
            }

            List<ExpressionsData> children = data.Where(x => x.MemberValue != null || x.MethodData.Count > 0 || (x.MemberValue == null && x.IsNullValue)).ToList();

            foreach (ExpressionsData child in children)
            {
                ExpressionsData parent = data[child.ParentIndex];

                if (child.MethodData.Count > 0)
                {
                    if(child.MemberValue != null && child.MethodData[0].ComparedValue == null  && child.MethodData[0].CompareProperty == null)
                    {
                        child.MethodData[0].ComparedValue = child.MemberValue;
                    }

                    SqlFunctionResult sqlFunctionResult = child.MethodData[0].TranslateBHMethod<T>(index, letter, isQuotedDb, schemaName, connectionIndex);

                    if (sqlFunctionResult.ParamName != string.Empty)
                    {
                        parameters.Add(new BlackHoleParameter { Name = sqlFunctionResult.ParamName, Value = sqlFunctionResult.Value });
                        index++;
                    }

                    if(parent.SqlCommand == string.Empty)
                    {
                        parent.SqlCommand = $"{sqlFunctionResult.SqlCommand}";

                        if (HAMode) 
                        { 
                            parent.SqlCommandReverseQuotes = $"{sqlFunctionResult.SqlCommandReverseQuotes}";
                        }
                    }
                    else
                    {
                        parent.SqlCommand += $" and {sqlFunctionResult.SqlCommand}";

                        if (HAMode)
                        {
                            parent.SqlCommandReverseQuotes += $" and {sqlFunctionResult.SqlCommandReverseQuotes}";
                        }
                    }

                    parent.LeftChecked = false;
                }
                else
                {
                    if (parent.LeftChecked)
                    {
                        ColumnAndParameter childParams = child.TranslateExpression(index, isQuotedDb, letter);

                        if (childParams.ParamName != string.Empty)
                        {
                            parameters.Add(new BlackHoleParameter { Name = childParams.ParamName, Value = childParams.Value });
                        }

                        parent.SqlCommand = $"{childParams.Column}";

                        if (HAMode)
                        {
                            parent.SqlCommandReverseQuotes = $"{childParams.ColumnReverseQuotes}";
                        }

                        parent.LeftChecked = false;
                        index++;
                    }
                    else
                    {
                        ColumnAndParameter parentCols = parent.TranslateExpression(index, isQuotedDb, letter);

                        if (parentCols.ParamName != string.Empty)
                        {
                            parameters.Add(new BlackHoleParameter { Name = parentCols.ParamName, Value = parentCols.Value });
                        }

                        index++;

                        if (child != parent)
                        {
                            ColumnAndParameter childCols = child.TranslateExpression(index, isQuotedDb, letter);

                            if (childCols.ParamName != string.Empty)
                            {
                                parameters.Add(new BlackHoleParameter { Name = childCols.ParamName, Value = childCols.Value });
                            }

                            parent.SqlCommand = $"({parent.SqlCommand} {parentCols.Column} {childCols.Column})";

                            if (HAMode)
                            {
                                parent.SqlCommandReverseQuotes = $"({parent.SqlCommandReverseQuotes} {parentCols.ColumnReverseQuotes} {childCols.ColumnReverseQuotes})";
                            }
                        }
                        else
                        {
                            parent.SqlCommand = $"({parentCols.Column})";

                            if (HAMode)
                            {
                                parent.SqlCommandReverseQuotes = $"({parentCols.ColumnReverseQuotes})";
                            }
                        }

                        index++;
                    }
                }
            }

            List<ExpressionsData> selfCompair = data.Where(x => x.MemberValue == null && x.LeftMember != null && x.RightMember != null && x.MethodData.Count == 0).ToList();

            foreach (ExpressionsData self in selfCompair)
            {
                ExpressionsData parent = data[self.ParentIndex];
                ColumnAndParameter selfCompairCols = self.TranslateSelfCompairExpression(letter, isQuotedDb);

                parent.SqlCommand = $"({selfCompairCols.Column})";
                parent.LeftChecked = false;
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
                        if (HAMode) { parent.SqlCommandReverseQuotes = parents[parentsCount - 1 - i].SqlCommandReverseQuotes; }
                        parent.LeftChecked = false;
                    }
                    else
                    {
                        ColumnAndParameter parentParams = parent.TranslateExpression(index, isQuotedDb, letter);

                        if (parentParams.ParamName != string.Empty)
                        {
                            parameters.Add(new BlackHoleParameter { Name = parentParams.ParamName, Value = parentParams.Value });
                        }

                        parent.SqlCommand = $"({parent.SqlCommand} {parentParams.Column} {parents[parentsCount - 1 - i].SqlCommand})";

                        if (HAMode)
                        {
                            parent.SqlCommandReverseQuotes = $"({parent.SqlCommandReverseQuotes} {parentParams.ColumnReverseQuotes} {parents[parentsCount - 1 - i].SqlCommandReverseQuotes})";
                        }

                        index++;
                    }
                }
            }

            return new ColumnsAndParameters { Columns = data[0].SqlCommand, ColumnsReverseQuotes = data[0].SqlCommandReverseQuotes, Parameters = parameters, Count = index};
        }

        private static ColumnAndParameter TranslateSelfCompairExpression(this ExpressionsData expression, string? letter, bool isQuotedDb)
        {
            string? leftPart = expression.LeftMember?.ToString().Split('.')[1].UseNameQuotes(isQuotedDb);
            string? rightPart = expression.RightMember?.ToString().Split(".")[1].UseNameQuotes(isQuotedDb);
            string subLetter = letter != string.Empty ? $"{letter}." : string.Empty;

            string column = expression.OperationType switch
            {
                ExpressionType.Equal => $"{subLetter}{leftPart} = {subLetter}{rightPart}",
                ExpressionType.GreaterThanOrEqual => $"{subLetter}{leftPart} >= {subLetter}{rightPart}",
                ExpressionType.LessThanOrEqual => $"{leftPart} <= {rightPart}",
                ExpressionType.LessThan => $"{subLetter} {leftPart} < {subLetter}{rightPart}",
                ExpressionType.GreaterThan => $"{subLetter} {leftPart} > {subLetter}{rightPart}",
                ExpressionType.NotEqual => $"{subLetter} {leftPart} != {subLetter}{rightPart}",
                _ => string.Empty
            };

            return new ColumnAndParameter { Column = column };
        }

        private static SqlFunctionResult TranslateBHMethod<T>(this MethodExpressionData MethodData, int index, string? letter, bool isQuotedDb, string schemaName, int connectionIndex)
        {
            SqlFunctionResult result = new();

            bool connectionMismatch = false;

            if(MethodData.CastedOn != null)
            {
                bool NotHAMode = true;

                if (WormHoleData.BlackHoleMode == BHMode.HighAvailability)
                {
                    NotHAMode = false;
                }

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
                    string selectCommandReverse;
                    string operationType;

                    if (methodType == 0)
                    {
                        if(MethodData.ComparedValue != null)
                        {
                            if (MethodData.MethodArguments.Count == 1 && MethodData.MethodName == "Contains")
                            {
                                result.ParamName = $"{compareProperty[1]}{index}";
                                result.Value = $"%{MethodData.MethodArguments[0]}%";
                                result.SqlCommand = $" {letter}{compareProperty[1].UseNameQuotes(isQuotedDb)} Like @{result.ParamName}";
                                result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" {letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)} Like @{result.ParamName}";
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
                                        result.SqlCommand = $" REPLACE({letter}{compareProperty[1].UseNameQuotes(isQuotedDb)},'{first}','{second}') {operationType} @{result.ParamName} ";
                                        result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" REPLACE({letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)},'{first}','{second}') {operationType} @{result.ParamName} ";
                                        result.WasTranslated = true;
                                        return result;
                                    case "SqlLike":
                                        result.ParamName = $"{compareProperty[1]}{index}";
                                        result.Value = MethodData.MethodArguments[1];
                                        result.SqlCommand = $" {letter}{compareProperty[1].UseNameQuotes(isQuotedDb)} Like @{result.ParamName}";
                                        result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" {letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)} Like @{result.ParamName}";
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
                                    result.SqlCommand = $" UPPER({letter}{compareProperty[1].UseNameQuotes(isQuotedDb)}) {operationType} @{result.ParamName} ";
                                    result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" UPPER({letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)}) {operationType} @{result.ParamName} ";
                                    result.WasTranslated = true;
                                    return result;
                                case "ToLower":
                                    operationType = ExpressionTypeToSql(MethodData.OperatorType, MethodData.ReverseOperator, false);
                                    result.ParamName = $"{compareProperty[1]}{index}";
                                    result.Value = MethodData.ComparedValue;
                                    result.SqlCommand = $" LOWER({letter}{compareProperty[1].UseNameQuotes(isQuotedDb)}) {operationType} @{result.ParamName} ";
                                    result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" LOWER({letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)}) {operationType} @{result.ParamName} ";
                                    result.WasTranslated = true;
                                    return result;
                                case "SqlLength":
                                    operationType = ExpressionTypeToSql(MethodData.OperatorType, !MethodData.ReverseOperator, false);
                                    result.ParamName = $"{compareProperty[1]}{index}";
                                    result.Value = MethodData.ComparedValue;
                                    result.SqlCommand = $" LENGTH({letter}{compareProperty[1].UseNameQuotes(isQuotedDb)}) {operationType} @{result.ParamName} ";
                                    result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" LENGTH({letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)}) {operationType} @{result.ParamName} ";
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
                                result.SqlCommand = $" {letter}{compareProperty[1].UseNameQuotes(isQuotedDb)} >= @{result.ParamName} ";
                                result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" {letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)} >= @{result.ParamName} ";
                                result.WasTranslated = true;
                                return result;
                            case "SqlDateBefore":
                                result.ParamName = $"DateProp{index}";
                                result.Value = MethodData.MethodArguments[1];
                                result.SqlCommand = $" {letter}{compareProperty[1].UseNameQuotes(isQuotedDb)} < @{result.ParamName} ";
                                result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" {letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)} < @{result.ParamName} ";
                                result.WasTranslated = true;
                                return result;
                        }
                    }

                    if (methodType == 1 || methodType == 2)
                    {
                        if (MethodData.MethodArguments.Count == 2 && MethodData.CompareProperty != null)
                        {
                            Type? aTableType = MethodData.CompareProperty.Member?.ReflectedType;
                            string? PropertyName = MethodData.CompareProperty.Member?.Name;
                            string? aTable = aTableType?.GetTableDisplayName(false);

                            if (aTableType != null && PropertyName != null)
                            {
                                string? specialSchema;
                                EntityInfo otherInfo = aTableType.SwitchBlackHoleMode(DatabaseRole.Master);

                                switch (MethodData.MethodName)
                                {
                                    case "SqlEqualTo":
                                        connectionMismatch = connectionIndex != otherInfo.DBTIndex;
                                        specialSchema = otherInfo.EntitySchema;
                                        result.ParamName = $"OtherId{index}";
                                        result.Value = MethodData.MethodArguments[1];
                                        selectCommandReverse = NotHAMode ? string.Empty : $"( Select {PropertyName.UseNameQuotes(!isQuotedDb)} from {specialSchema}{aTable.UseNameQuotes(!isQuotedDb)} where {"Id".UseNameQuotes(!isQuotedDb)} = @{result.ParamName} )";
                                        selectCommand = $"( Select {PropertyName.UseNameQuotes(isQuotedDb)} from {specialSchema}{aTable.UseNameQuotes(isQuotedDb)} where {"Id".UseNameQuotes(isQuotedDb)} = @{result.ParamName} )";
                                        result.SqlCommand = $" {letter}{compareProperty[1].UseNameQuotes(isQuotedDb)} = {selectCommand} ";
                                        result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" {letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)} = {selectCommandReverse} ";
                                        result.WasTranslated = true;
                                        return result;
                                    case "SqlGreaterThan":
                                        connectionMismatch = connectionIndex != otherInfo.DBTIndex;
                                        specialSchema = otherInfo.EntitySchema;
                                        result.ParamName = $"OtherId{index}";
                                        result.Value = MethodData.MethodArguments[1];
                                        selectCommandReverse = NotHAMode ? string.Empty : $"( Select {PropertyName.UseNameQuotes(!isQuotedDb)} from {specialSchema}{aTable.UseNameQuotes(!isQuotedDb)} where {"Id".UseNameQuotes(!isQuotedDb)} = @{result.ParamName} )";
                                        selectCommand = $"( Select {PropertyName.UseNameQuotes(isQuotedDb)} from {specialSchema}{aTable.UseNameQuotes(isQuotedDb)} where {"Id".UseNameQuotes(isQuotedDb)} = @{result.ParamName} )";
                                        result.SqlCommand = $" {letter}{compareProperty[1].UseNameQuotes(isQuotedDb)} > {selectCommand} ";
                                        result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" {letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)} > {selectCommandReverse} ";
                                        result.WasTranslated = true;
                                        return result;
                                    case "SqlLessThan":
                                        connectionMismatch = connectionIndex != otherInfo.DBTIndex;
                                        specialSchema = otherInfo.EntitySchema;
                                        result.ParamName = $"OtherId{index}";
                                        result.Value = MethodData.MethodArguments[1];
                                        selectCommandReverse = NotHAMode ? string.Empty : $"( Select {PropertyName.UseNameQuotes(!isQuotedDb)} from {specialSchema}{aTable.UseNameQuotes(!isQuotedDb)} where {"Id".UseNameQuotes(!isQuotedDb)} = @{result.ParamName} )";
                                        selectCommand = $"( Select {PropertyName.UseNameQuotes(isQuotedDb)} from {specialSchema}{aTable.UseNameQuotes(isQuotedDb)} where {"Id".UseNameQuotes(isQuotedDb)} = @{result.ParamName} )";
                                        result.SqlCommand = $" {letter}{compareProperty[1].UseNameQuotes(isQuotedDb)} < {selectCommand} ";
                                        result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" {letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)} < {selectCommandReverse} ";
                                        result.WasTranslated = true;
                                        return result;
                                }
                            }
                        }

                        switch (MethodData.MethodName)
                        {
                            case "SqlMax":
                                selectCommandReverse = NotHAMode ? string.Empty : $"( Select MAX({compareProperty[1].UseNameQuotes(!isQuotedDb)}) from {schemaName}{MethodData.TableName.UseNameQuotes(!isQuotedDb)} )";
                                selectCommand = $"( Select MAX({compareProperty[1].UseNameQuotes(isQuotedDb)}) from {schemaName}{MethodData.TableName.UseNameQuotes(isQuotedDb)} )";
                                result.SqlCommand = $" {letter}{compareProperty[1].UseNameQuotes(isQuotedDb)} = {selectCommand} ";
                                result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" {letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)} = {selectCommandReverse} ";
                                result.WasTranslated = true;
                                return result;
                            case "SqlMin":
                                selectCommandReverse = NotHAMode ? string.Empty : $"( Select MIN({compareProperty[1].UseNameQuotes(!isQuotedDb)}) from {schemaName}{MethodData.TableName.UseNameQuotes(!isQuotedDb)} )";
                                selectCommand = $"( Select MIN({compareProperty[1].UseNameQuotes(isQuotedDb)}) from {schemaName}{MethodData.TableName.UseNameQuotes(isQuotedDb)} )";
                                result.SqlCommand = $" {letter}{compareProperty[1].UseNameQuotes(isQuotedDb)} = {selectCommand} ";
                                result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" {letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)} = {selectCommandReverse} ";
                                result.WasTranslated = true;
                                return result;
                        }
                    }

                    if (methodType == 2)
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
                                    result.SqlCommand = $" ABS({letter}{compareProperty[1].UseNameQuotes(isQuotedDb)}) {operationType} @{result.ParamName} ";
                                    result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" ABS({letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)}) {operationType} @{result.ParamName} ";
                                    result.WasTranslated = true;
                                    return result;
                                case "SqlRound":
                                    operationType = ExpressionTypeToSql(MethodData.OperatorType, !MethodData.ReverseOperator, false);
                                    result.ParamName = $"{compareProperty[1]}{index}";
                                    result.Value = MethodData.ComparedValue;
                                    result.SqlCommand = $" ROUND({letter}{compareProperty[1].UseNameQuotes(isQuotedDb)}{decimalPoints}) {operationType} @{result.ParamName} ";
                                    result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" ROUND({letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)}{decimalPoints}) {operationType} @{result.ParamName} ";
                                    result.WasTranslated = true;
                                    return result;
                                case "SqlPlus":
                                    operationType = ExpressionTypeToSql(MethodData.OperatorType, !MethodData.ReverseOperator, false);
                                    if (thePlusValue != null)
                                    {
                                        result.ParamName = $"{compareProperty[1]}{index}";
                                        result.Value = MethodData.ComparedValue;
                                        result.SqlCommand = $" ({letter}{compareProperty[1].UseNameQuotes(isQuotedDb)} + {letter}{thePlusValue.Name.UseNameQuotes(isQuotedDb)}) {operationType} @{result.ParamName} ";
                                        result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" ({letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)} + {letter}{thePlusValue.Name.UseNameQuotes(!isQuotedDb)}) {operationType} @{result.ParamName} ";
                                    }
                                    else
                                    {
                                        result.ParamName = $"{compareProperty[1]}{index}";
                                        result.Value = MethodData.ComparedValue;
                                        result.SqlCommand = $" ({letter}{compareProperty[1].UseNameQuotes(isQuotedDb)} + {MethodData.MethodArguments[1]}) {operationType} @{result.ParamName} ";
                                        result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" ({letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)} + {MethodData.MethodArguments[1]}) {operationType} @{result.ParamName} ";
                                    }
                                    result.WasTranslated = true;
                                    return result;
                                case "SqlMinus":
                                    operationType = ExpressionTypeToSql(MethodData.OperatorType, !MethodData.ReverseOperator, false);
                                    if (thePlusValue != null)
                                    {
                                        result.ParamName = $"{compareProperty[1]}{index}";
                                        result.Value = MethodData.ComparedValue;
                                        result.SqlCommand = $" ({letter}{compareProperty[1].UseNameQuotes(isQuotedDb)} - {letter}{thePlusValue.Name.UseNameQuotes(isQuotedDb)}) {operationType} @{result.ParamName} ";
                                        result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" ({letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)} - {letter}{thePlusValue.Name.UseNameQuotes(!isQuotedDb)}) {operationType} @{result.ParamName} ";
                                    }
                                    else
                                    {
                                        result.ParamName = $"{compareProperty[1]}{index}";
                                        result.Value = MethodData.ComparedValue;
                                        result.SqlCommand = $" ({letter}{compareProperty[1].UseNameQuotes(isQuotedDb)} - {MethodData.MethodArguments[1]}) {operationType} @{result.ParamName} ";
                                        result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" ({letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)} - {MethodData.MethodArguments[1]}) {operationType} @{result.ParamName} ";
                                    }
                                    result.WasTranslated = true;
                                    return result;
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
                                        string SelectAverage = $"( Select AVG({compareProperty[1].UseNameQuotes(isQuotedDb)}) from {schemaName}{MethodData.TableName.UseNameQuotes(isQuotedDb)} )";
                                        string SelectAverageReverse = NotHAMode ? string.Empty : $"( Select AVG({compareProperty[1].UseNameQuotes(!isQuotedDb)}) from {schemaName}{MethodData.TableName.UseNameQuotes(!isQuotedDb)} )";
                                        result.SqlCommand = $" {letter}{otherPropName[1].UseNameQuotes(isQuotedDb)} {operationType} {SelectAverage} ";
                                        result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" {letter}{otherPropName[1].UseNameQuotes(!isQuotedDb)} {operationType} {SelectAverageReverse} ";
                                        result.WasTranslated = true;
                                        return result;
                                    case "SqlAbsolut":
                                        operationType = ExpressionTypeToSql(MethodData.OperatorType, !MethodData.ReverseOperator, false);
                                        result.SqlCommand = $" ABS({letter}{compareProperty[1].UseNameQuotes(isQuotedDb)}) {operationType} {letter}{otherPropName[1].UseNameQuotes(isQuotedDb)} ";
                                        result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" ABS({letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)}) {operationType} {letter}{otherPropName[1].UseNameQuotes(!isQuotedDb)} ";
                                        result.WasTranslated = true;
                                        return result;
                                    case "SqlRound":
                                        operationType = ExpressionTypeToSql(MethodData.OperatorType, !MethodData.ReverseOperator, false);
                                        result.SqlCommand = $" ROUND({letter}{compareProperty[1].UseNameQuotes(isQuotedDb)}{decimalPoints}) {operationType} {letter}{otherPropName[1].UseNameQuotes(isQuotedDb)} ";
                                        result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" ROUND({letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)}{decimalPoints}) {operationType} {letter}{otherPropName[1].UseNameQuotes(!isQuotedDb)} ";
                                        result.WasTranslated = true;
                                        return result;
                                    case "SqlPlus":
                                        operationType = ExpressionTypeToSql(MethodData.OperatorType, !MethodData.ReverseOperator, false);
                                        if (thePlusValue != null)
                                        {
                                            result.SqlCommand = $" ({letter}{compareProperty[1].UseNameQuotes(isQuotedDb)} + {letter}{thePlusValue.Name.UseNameQuotes(isQuotedDb)}) {operationType} {letter}{otherPropName[1].UseNameQuotes(isQuotedDb)} ";
                                            result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" ({letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)} + {letter}{thePlusValue.Name.UseNameQuotes(!isQuotedDb)}) {operationType} {letter}{otherPropName[1].UseNameQuotes(!isQuotedDb)} ";
                                        }
                                        else
                                        {
                                            result.SqlCommand = $" ({letter}{compareProperty[1].UseNameQuotes(isQuotedDb)} + {MethodData.MethodArguments[1]}) {operationType} {letter}{otherPropName[1].UseNameQuotes(isQuotedDb)} ";
                                            result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" ({letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)} + {MethodData.MethodArguments[1]}) {operationType} {letter}{otherPropName[1].UseNameQuotes(!isQuotedDb)} ";
                                        }
                                        result.WasTranslated = true;
                                        return result;
                                    case "SqlMinus":
                                        operationType = ExpressionTypeToSql(MethodData.OperatorType, !MethodData.ReverseOperator, false);
                                        if (thePlusValue != null)
                                        {
                                            result.SqlCommand = $" ({letter}{compareProperty[1].UseNameQuotes(isQuotedDb)} - {letter}{thePlusValue.Name.UseNameQuotes(isQuotedDb)}) {operationType} {letter}{otherPropName[1].UseNameQuotes(isQuotedDb)} ";
                                            result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" ({letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)} - {letter}{thePlusValue.Name.UseNameQuotes(!isQuotedDb)}) {operationType} {letter}{otherPropName[1].UseNameQuotes(!isQuotedDb)} ";
                                        }
                                        else
                                        {
                                            result.SqlCommand = $" ({letter}{compareProperty[1].UseNameQuotes(isQuotedDb)} - {MethodData.MethodArguments[1]}) {operationType} {letter}{otherPropName[1].UseNameQuotes(isQuotedDb)} ";
                                            result.SqlCommandReverseQuotes = NotHAMode ? string.Empty : $" ({letter}{compareProperty[1].UseNameQuotes(!isQuotedDb)} - {MethodData.MethodArguments[1]}) {operationType} {letter}{otherPropName[1].UseNameQuotes(!isQuotedDb)} ";
                                        }
                                        result.WasTranslated = true;
                                        return result;
                                }
                            }
                        }
                    }
                }
            }

            if (!result.WasTranslated)
            {
                throw new Exception($"Black Hole Method Translator : The '{MethodData.MethodName}' Method is unsupported , or the usage is incorrect.");
            }

            if (connectionMismatch)
            {
                throw new Exception($"Black Hole Method Translator : Connection Mismatch at method '{MethodData.MethodName}'. The entities belong to different databases.");
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


        private static ColumnAndParameter TranslateExpression(this ExpressionsData expression, int index, bool isQuotedDb, string? letter)
        {
            string? column = string.Empty;
            string? reverseColumn = string.Empty;
            string? parameter = string.Empty;
            object? value = new();
            string[]? variable;
            string subLetter = letter != string.Empty ? $"{letter}." : string.Empty;
            string sqlOperator;

            switch (expression.OperationType)
            {
                case ExpressionType.AndAlso:
                    column = " and ";
                    reverseColumn = " and ";
                    break;
                case ExpressionType.OrElse:
                    column = " or ";
                    reverseColumn = " or ";
                    break;
                case ExpressionType.Equal:
                    value = expression?.MemberValue;
                    variable = expression?.LeftMember != null ? expression?.LeftMember.ToString().Split(".") : expression?.RightMember?.ToString().Split(".");
                    column = $"{subLetter}{variable?[1].UseNameQuotes(isQuotedDb)} = @{variable?[1]}{index}";
                    reverseColumn = $"{subLetter}{variable?[1].UseNameQuotes(!isQuotedDb)} = @{variable?[1]}{index}";
                    if (value == null)
                    {
                        column = $"{subLetter}{variable?[1].UseNameQuotes(isQuotedDb)} is @{variable?[1]}{index}";
                        reverseColumn = $"{subLetter}{variable?[1].UseNameQuotes(!isQuotedDb)} is @{variable?[1]}{index}";
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
                    column = $"{subLetter}{variable?[1].UseNameQuotes(isQuotedDb)} {sqlOperator} @{variable?[1]}{index}";
                    reverseColumn = $"{subLetter}{variable?[1].UseNameQuotes(!isQuotedDb)} {sqlOperator} @{variable?[1]}{index}";
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
                    column = $"{subLetter}{variable?[1].UseNameQuotes(isQuotedDb)} {sqlOperator} @{variable?[1]}{index}";
                    reverseColumn = $"{subLetter}{variable?[1].UseNameQuotes(!isQuotedDb)} {sqlOperator} @{variable?[1]}{index}";
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
                    column = $"{subLetter}{variable?[1].UseNameQuotes(isQuotedDb)} {sqlOperator} @{variable?[1]}{index}";
                    reverseColumn = $"{subLetter}{variable?[1].UseNameQuotes(!isQuotedDb)} {sqlOperator} @{variable?[1]}{index}";
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
                    column = $"{subLetter}{variable?[1].UseNameQuotes(isQuotedDb)} {sqlOperator} @{variable?[1]}{index}";
                    reverseColumn = $"{subLetter}{variable?[1].UseNameQuotes(!isQuotedDb)} {sqlOperator} @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.MemberValue;
                    break;
                case ExpressionType.NotEqual:
                    value = expression?.MemberValue;
                    variable = expression?.LeftMember != null ? expression?.LeftMember.ToString().Split(".") : expression?.RightMember?.ToString().Split(".");
                    column = $"{subLetter}{variable?[1].UseNameQuotes(isQuotedDb)} != @{variable?[1]}{index}";
                    reverseColumn = $"{subLetter}{variable?[1].UseNameQuotes(!isQuotedDb)} != @{variable?[1]}{index}";
                    if (value == null)
                    {
                        column = $"{subLetter}{variable?[1].UseNameQuotes(isQuotedDb)} is not @{variable?[1]}{index}";
                        reverseColumn = $"{subLetter}{variable?[1].UseNameQuotes(!isQuotedDb)} is not @{variable?[1]}{index}";
                    }
                    parameter = $"{variable?[1]}{index}";
                    break;
                case ExpressionType.Default:
                    column = "1 != 1";
                    reverseColumn = "1 != 1";
                    break;
            }

            return new ColumnAndParameter { Column = column, ColumnReverseQuotes = reverseColumn, ParamName = parameter, Value = value};
        }

        internal static void CreateJoin<Dto, TSource, TOther>(this JoinsData data, LambdaExpression key, LambdaExpression otherKey, string joinType, bool first)
        {
            TableLetters? firstType;
            string? parameter = string.Empty;

            if (first)
            {
                if (WormHoleData.BlackHoleMode == BHMode.HighAvailability)
                {
                    data.HAMode = true;
                }

                string? parameterOne = key.Parameters[0].Name;

                data.BaseTable = typeof(TSource);

                bool OpenEntity = typeof(TSource).BaseType?.GetGenericTypeDefinition() == typeof(BHEntity<>);
                EntityInfo basicTableInfo = data.BaseTable.SwitchBlackHoleMode(DatabaseRole.Master);

                data.TablesToLetters.Add(new TableLetters { Table = typeof(TSource),
                    Schema = basicTableInfo.EntitySchema,
                    Letter = parameterOne, IsOpenEntity = OpenEntity });

                data.ConnectionIndex = basicTableInfo.DBTIndex;
                data.IsQuotedDb = WormHoleData.IsQuotedDb[basicTableInfo.DBTIndex];
                data.Letters.Add(parameterOne);
                data.BindPropertiesToDtoExtension(typeof(TSource), parameterOne);
                firstType = data.TablesToLetters[0];
            }
            else
            {
                firstType = data.TablesToLetters.FirstOrDefault(x => x.Table == typeof(TSource));

                if (firstType == null)
                {
                    throw new Exception($"Joins Error : At the join between '{typeof(TSource).Name}' and  '{typeof(TOther).Name}'," +
                        $" the first table '{typeof(TSource).Name}' has not been used in previous joins.");
                }
                else
                {
                    parameter = firstType.Letter;
                }
            }

            MemberExpression? member = key.Body as MemberExpression;
            string? propName = member?.Member.Name;

            string? parameterOther = otherKey.Parameters[0].Name;
            MemberExpression? memberOther = otherKey.Body as MemberExpression;
            string? propNameOther = memberOther?.Member.Name;

            Type otherTableType = typeof(TOther);

            TableLetters? secondTable = data.TablesToLetters.FirstOrDefault(x => x.Table == otherTableType);

            string schemaName = string.Empty;

            if (secondTable == null)
            {
                bool isOpen = otherTableType.BaseType?.GetGenericTypeDefinition() == typeof(BHEntity<>);
                EntityInfo otherTableInfo = otherTableType.SwitchBlackHoleMode(DatabaseRole.Master);

                if (data.ConnectionIndex == otherTableInfo.DBTIndex)
                {
                    if (data.Letters.Contains(parameterOther))
                    {
                        parameterOther += data.HelperIndex.ToString();
                        data.HelperIndex++;
                    }

                    data.TablesToLetters.Add(new TableLetters
                    {
                        Table = otherTableType,
                        Schema = otherTableInfo.EntitySchema,
                        Letter = parameterOther,
                        IsOpenEntity = isOpen
                    });

                    data.Letters.Add(parameterOther);
                    data.BindPropertiesToDtoExtension(otherTableType, parameterOther);
                }
                else
                {
                    throw new Exception($"Joins Error : At the join between '{typeof(TSource).Name}' and  '{typeof(TOther).Name}'," +
                        $" there is a connection mismatch. The Tables belong to different databases.");
                }
            }
            else
            {
                parameterOther = secondTable.Letter;
                schemaName = secondTable.Schema;
            }

            data.Joins += $" {joinType} join {schemaName}{otherTableType.GetTableDisplayName(data.IsQuotedDb)} {parameterOther} on {parameterOther}.{propNameOther.UseNameQuotes(data.IsQuotedDb)} = {parameter}.{propName.UseNameQuotes(data.IsQuotedDb)}";

            if (data.HAMode)
            {
                data.JoinsReverseQuotes += $" {joinType} join {schemaName}{otherTableType.GetTableDisplayName(!data.IsQuotedDb)} {parameterOther} on {parameterOther}.{propNameOther.UseNameQuotes(!data.IsQuotedDb)} = {parameter}.{propName.UseNameQuotes(!data.IsQuotedDb)}";
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

        internal static void Additional<TSource, TOther>(this JoinsData data, LambdaExpression key, LambdaExpression otherKey, string additionalType)
        {
            string? firstLetter = data.TablesToLetters.First(t => t.Table == typeof(TSource)).Letter;
            string? secondLetter = data.TablesToLetters.First(t => t.Table == typeof(TOther)).Letter;

            MemberExpression? member = key.Body as MemberExpression;
            string? propName = member?.Member.Name;
            MemberExpression? memberOther = otherKey.Body as MemberExpression;
            string? propNameOther = memberOther?.Member.Name;

            data.Joins += $" {additionalType} {secondLetter}.{propNameOther.UseNameQuotes(data.IsQuotedDb)} = {firstLetter}.{propName.UseNameQuotes(data.IsQuotedDb)}";

            if (data.HAMode)
            {
                data.JoinsReverseQuotes += $" {additionalType} {secondLetter}.{propNameOther.UseNameQuotes(!data.IsQuotedDb)} = {firstLetter}.{propName.UseNameQuotes(!data.IsQuotedDb)}";
            }
        }

        internal static void WhereJoin<TSource>(this JoinsData data, Expression<Func<TSource, bool>> predicate)
        {
            TableLetters tb = data.TablesToLetters.First(x => x.Table == typeof(TSource));

            ColumnsAndParameters colsAndParams = predicate.SplitMembers<TSource>(data.IsQuotedDb, tb.Letter,
                data.DynamicParams, data.ParamsCount, tb.Schema, data.ConnectionIndex);

            data.DynamicParams = colsAndParams.Parameters;
            data.ParamsCount = colsAndParams.Count;

            if (data.WherePredicates == string.Empty)
            {
                data.WherePredicates = $" where {colsAndParams.Columns}";

                if (data.HAMode)
                {
                    data.WhereReverseQuotes = $" where {colsAndParams.ColumnsReverseQuotes}";
                }
            }
            else
            {
                data.WherePredicates += $" and {colsAndParams.Columns}";

                if (data.HAMode)
                {
                    data.WhereReverseQuotes += $" and {colsAndParams.ColumnsReverseQuotes}";
                }
            }
        }

        internal static ColumnsAndParameters? TranslateJoin<Dto>(this JoinsData data) where Dto : BHDtoIdentifier
        {
            if (data.DtoType == typeof(Dto))
            {
                data.RejectInactiveEntities();
                TableLetters tL = data.TablesToLetters.First(x => x.Table == data.BaseTable);
                string commandTextReverse = string.Empty;
                string commandText = $"{data.BuildCommand(false)} from {tL.Schema}{tL.Table?.GetTableDisplayName(data.IsQuotedDb)} {tL.Letter} {data.Joins} {data.WherePredicates} {data.OrderByOptions}";

                if (data.HAMode)
                {
                    commandTextReverse = $"{data.BuildCommand(true)} from {tL.Schema}{tL.Table?.GetTableDisplayName(!data.IsQuotedDb)} {tL.Letter} {data.JoinsReverseQuotes} {data.WhereReverseQuotes} {data.OrderByReverseQuotes}";
                }

                return new ColumnsAndParameters { Columns = commandText, ColumnsReverseQuotes = commandTextReverse, Parameters = data.DynamicParams };
            }
            return null;
        }

        internal static string OrderByToSql<T>(this BHOrderBy<T> orderByConfig, bool IsQuotedDb)
        {
            if (orderByConfig.OrderBy.LockedByError)
            {
                return string.Empty;
            }

            StringBuilder orderby = new();
            string limiter = string.Empty;

            foreach (OrderByPair pair in orderByConfig.OrderBy.OrderProperties)
            {
                orderby.Append($", {pair.PropertyName.UseNameQuotes(IsQuotedDb)} {pair.Orientation}");
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
                string orderByReverse = string.Empty;

                string limiter = string.Empty;
                int counter = 0;

                foreach (OrderByPair pair in orderByConfig.OrderBy.OrderProperties)
                {
                    if (data.OccupiedDtoProps.FirstOrDefault(x => x.PropName == pair.PropertyName) is PropertyOccupation occupation)
                    {
                        counter++;
                        orderby.Append($", {occupation.TableLetter}.{occupation.TableProperty.UseNameQuotes(data.IsQuotedDb)} {pair.Orientation}");

                        if (data.HAMode)
                        {
                            orderByReverse += $", {occupation.TableLetter}.{occupation.TableProperty.UseNameQuotes(!data.IsQuotedDb)} {pair.Orientation}";
                        }
                    }
                }

                if (orderByConfig.OrderBy.TakeSpecificRange)
                {
                    limiter = orderByConfig.OrderBy.RowsLimiter();
                }

                if (counter > 0)
                {
                    data.OrderByOptions = $"order by{orderby.ToString().Remove(0, 1)}{limiter}";

                    if (data.HAMode)
                    {
                        data.OrderByReverseQuotes = $"order by{orderByReverse.Remove(0, 1)}{limiter}";
                    }
                }
            }
        }

        private static string RowsLimiter<T>(this BlackHoleOrderBy<T> limiter)
        {
            return BHStaticSettings.DatabaseType switch
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
            string reverseCommand = string.Empty;
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

                    command += $" {anD} {table.Letter}.{inactiveColumn.UseNameQuotes(data.IsQuotedDb)} = 0 ";

                    if (data.HAMode)
                    {
                        reverseCommand += $" {anD} {table.Letter}.{inactiveColumn.UseNameQuotes(!data.IsQuotedDb)} = 0 ";
                    }
                }
            }

            data.WherePredicates += command;

            if (data.HAMode)
            {
                data.WhereReverseQuotes += reverseCommand;
            }
        }

        private static string BuildCommand(this JoinsData data, bool reverseQuotes)
        {
            string sqlCommand = "select ";

            foreach (PropertyOccupation prop in data.OccupiedDtoProps.Where(x => x.Occupied))
            {
                if (reverseQuotes)
                {
                    sqlCommand += prop.WithCast switch
                    {
                        1 => $" {prop.TableLetter}.{prop.TableProperty.UseNameQuotes(!data.IsQuotedDb)} as {prop.PropName.UseNameQuotes(!data.IsQuotedDb)},",
                        2 => $" cast({prop.TableLetter}.{prop.TableProperty.UseNameQuotes(!data.IsQuotedDb)} as {prop.PropType.SqlTypeFromType()}) as {prop.PropName.UseNameQuotes(!data.IsQuotedDb)},",
                        _ => $" {prop.TableLetter}.{prop.PropName.UseNameQuotes(!data.IsQuotedDb)},",
                    };
                }
                else
                {
                    sqlCommand += prop.WithCast switch
                    {
                        1 => $" {prop.TableLetter}.{prop.TableProperty.UseNameQuotes(data.IsQuotedDb)} as {prop.PropName.UseNameQuotes(data.IsQuotedDb)},",
                        2 => $" cast({prop.TableLetter}.{prop.TableProperty.UseNameQuotes(data.IsQuotedDb)} as {prop.PropType.SqlTypeFromType()}) as {prop.PropName.UseNameQuotes(data.IsQuotedDb)},",
                        _ => $" {prop.TableLetter}.{prop.PropName.UseNameQuotes(data.IsQuotedDb)},",
                    };
                }
            }

            return sqlCommand[..^1];
        }

        private static string SqlTypeFromType(this Type? type)
        {
            string[] SqlDatatypes = BHStaticSettings.DatabaseType switch
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
                BlackHoleSqlTypes sqlType = BHStaticSettings.DatabaseType;
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

        internal static string UseNameQuotes(this string? propName, bool isQuotedDb)
        {
            if (isQuotedDb)
            {
                return $@"""{propName}""";
            }

            return propName ?? string.Empty;
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
