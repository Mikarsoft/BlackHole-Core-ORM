using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore.Connector.Tools
{
    internal class ExpressionsData
    {
        internal BinaryExpression? Operation { get; set; }
        internal MethodCallExpression? LeftMethodMember { get; set; }
        internal MethodCallExpression? RightMethodMember { get; set; }
        public MemberExpression? LeftMember { get; set; }
        internal MemberExpression? RightMember { get; set; }
        internal List<MethodExpressionData> MethodData { get; set; } = new List<MethodExpressionData>();
        internal ExpressionType OperationType { get; set; }
        internal object? MemberValue { get; set; }
        internal bool MethodChecked { get; set; }
        internal bool RightChecked { get; set; }
        internal bool LeftChecked { get; set; }
        internal int ParentIndex { get; set; } = -1;
        internal int FinalPosition { get; set; } = -1;
        internal bool OpenParenthesis { get; set; }
        internal bool CloseParenthesis { get; set; }
        internal bool IsNullValue { get; set; }
    }

    public class BHExpressionLayer
    {
        public string[] Letters { get; set; } = new string[2];
        public string[] Columns { get; set; } = new string[2];
        public string[] Schemas { get; set; } = new string[2];
        public string[] Methods { get; set; } = new string[2];
        public ExpressionType ExpType { get; set; }
        public short ParentOrderId { get; set; }
    }

    public class BHExpressionPart
    {
        public short OrderId { get; set; }

        public short ParentOrderId { get; set; }

        public string? LeftName { get; set; }

        public string? RightName { get; set; }

        public object? Value { get; set; }

        //public BHMethod? CustomMethod { get; set; }

        public ExpressionType ExpType { get; set; }
    }

    internal class MethodExpressionData
    {
        internal List<object?> MethodArguments { get; set; } = new List<object?>();
        internal Expression? CastedOn { get; set; }
        internal string MethodName { get; set; } = string.Empty;
        internal ExpressionType OperatorType { get; set; }
        internal object? ComparedValue { get; set; }
        internal MemberExpression? CompareProperty { get; set; }
        internal bool ReverseOperator { get; set; }
        internal string? TableName { get; set; } = string.Empty;
    }

    public static class BHExpressionParser
    {
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

        private static BHExpressionPart[] OrderExpressionTree<T>(this List<ExpressionsData> data)
        {
            var res = data.OrderBy(x => x.FinalPosition).ToList();
            BHExpressionPart[] parts = new BHExpressionPart[res.Count];

            for (int i = 0; i < data.Count; i++)
            {
                if (res[i].IsEdge())
                {
                    parts[i] = res[i].MapEdge();
                    continue;
                }

                if (res[i].IsBranch())
                {
                    parts[i] = res[i].MapBranch();
                    continue;
                }

                if (res[i].IsDoubleEdge())
                {
                    parts[i] = res[i].MapDoubleEdge();
                }
            }

            return parts;
        }

        private static BHExpressionPart MapEdge(this ExpressionsData item)
        {
            return new BHExpressionPart
            {
                LeftName = item.LeftMember != null ? item.LeftMember.ToString().Split(".")[1] : item.RightMember?.ToString().Split(".")[1],
                Value = item?.MemberValue,
                ExpType = item?.OperationType ?? ExpressionType.Default
            };
        }

        private static BHExpressionPart MapBranch(this ExpressionsData item)
        {
            return new BHExpressionPart
            {
                ExpType = item?.OperationType ?? ExpressionType.Default
            };
        }

        private static BHExpressionPart MapDoubleEdge(this ExpressionsData item)
        {
            return new BHExpressionPart
            {
                LeftName = item.LeftMember?.ToString().Split('.')[1],
                RightName = item.RightMember?.ToString().Split(".")[1],
                ExpType = item?.OperationType ?? ExpressionType.Default
            };
        }

        private static bool IsBranch(this ExpressionsData item) => item.MemberValue == null && item.MethodData.Count == 0 && item.OperationType != ExpressionType.Default;

        private static bool IsEdge(this ExpressionsData item) => item.MemberValue != null || item.MethodData.Count > 0 || (item.MemberValue == null && item.IsNullValue);

        private static bool IsDoubleEdge(this ExpressionsData item) => item.MemberValue == null && item.LeftMember != null && item.RightMember != null && item.MethodData.Count == 0;

        private static bool IsNotConnector(this ExpressionsData item) => item.OperationType != ExpressionType.AndAlso && item.OperationType != ExpressionType.OrElse;

        public static BHExpressionPart[] ParseExpression<T>(this Expression<Func<T, bool>> completeExpression)
        {
            List<ExpressionsData> expressionTree = new();

            BinaryExpression? currentOperation = null;
            MethodCallExpression? methodCallOperation = null;

            if (completeExpression.Body is BinaryExpression bExp)
            {
                currentOperation = bExp;
            }

            if (completeExpression.Body is MethodCallExpression mcExp)
            {
                methodCallOperation = mcExp;
            }

            int currentIndx = 0;
            int positionIndex = 0;
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
            else if (completeExpression.Body is UnaryExpression unExpr)
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
                        LeftMember = Exp,
                        FinalPosition = 0
                    });
                }
            }
            else if (completeExpression.Body is MemberExpression memberExpr)
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
                    LeftMember = memberExpr,
                    FinalPosition = 0
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
                                    ExpressionType leftExpType = ExpressionType.NotEqual;

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
                            else if (expressionTree[currentIndx].Operation?.Left is MemberExpression leftExp)
                            {
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
                            else if (expressionTree[currentIndx].Operation?.Right is MemberExpression rightExp)
                            {
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
                    int parentSpot = expressionTree[currentIndx].ParentIndex;

                    if (expressionTree[currentIndx].FinalPosition < 0)
                    {
                        expressionTree[currentIndx].FinalPosition = positionIndex;
                        positionIndex++;

                        expressionTree[currentIndx].OpenParenthesis = true;
                    }

                    if (parentSpot != -1 && expressionTree[parentSpot].FinalPosition < 0)
                    {
                        expressionTree[parentSpot].FinalPosition = positionIndex;
                        positionIndex++;

                        if (expressionTree[parentSpot].RightChecked)
                        {
                            expressionTree[currentIndx].CloseParenthesis = true;
                        }
                    }

                    currentIndx -= 1;
                }

                if (currentIndx < 0)
                {
                    startTranslate = false;
                }
            }

            return expressionTree.OrderExpressionTree<T>();
        }
    }
}
