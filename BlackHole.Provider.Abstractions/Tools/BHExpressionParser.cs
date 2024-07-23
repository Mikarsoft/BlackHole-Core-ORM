using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore.Connector
{
    internal class ExpressionsData
    {
        internal BinaryExpression? Operation { get; set; }
        internal MethodCallExpression? LeftMethodMember { get; set; }
        internal MethodCallExpression? RightMethodMember { get; set; }
        public MemberExpression? LeftMember { get; set; }
        internal MemberExpression? RightMember { get; set; }
        internal List<MethodExpressionData> MethodData { get; set; } = [];
        internal ExpressionType OperationType { get; set; }
        internal object? MemberValue { get; set; }
        internal bool MethodChecked { get; set; }
        internal bool RightChecked { get; set; }
        internal bool LeftChecked { get; set; }
        internal int ParentIndex { get; set; } = -1;
        internal int FinalPosition { get; set; } = -1;
        internal int OpenParenthesis { get; set; }
        internal int CloseParenthesis { get; set; }
        internal bool IsNullValue { get; set; }
    }

    public class BHExpressionPart
    {
        public string? LeftName { get; set; }
        public string? RightName { get; set; }
        public object? Value { get; set; }
        public int OpenParenthesis { get; set; }
        public int CLoseParenthesis { get; set; }
        public ExpressionType ExpType { get; set; }
    }

    internal class MethodExpressionData
    {
        internal List<object?> MethodArguments { get; set; } = [];
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

        public static string BuildSqlCommand(this BHExpressionPart[] parts)
        {
            string res = string.Empty;

            foreach (var part in parts)
            {
                if (part.OpenParenthesis > 0)
                {
                    res += new string('(', part.OpenParenthesis);
                }

                if (part.LeftName != null)
                {
                    res += $" {part.LeftName} ";
                }

                res += part.ExpType switch
                {
                    ExpressionType.AndAlso => " and ",
                    ExpressionType.OrElse => " or ",
                    ExpressionType.Equal => " = ",
                    ExpressionType.NotEqual => " != ",
                    ExpressionType.GreaterThan => " > ",
                    ExpressionType.LessThan => " < ",
                    ExpressionType.GreaterThanOrEqual => " >= ",
                    ExpressionType.LessThanOrEqual => " <= ",
                    _ => string.Empty
                };

                if (!part.ExpType.IsConnector())
                {
                    if (part.RightName != null)
                    {
                        res += $" {part.RightName} ";
                    }
                    else
                    {
                        res += $" {part.Value ?? "null"} ";
                    }
                }

                if (part.CLoseParenthesis > 0)
                {
                    res += new string(')', part.CLoseParenthesis);
                }
            }

            return res;
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
                Value = item.MemberValue,
                ExpType = item.OperationType,
                OpenParenthesis = item.OpenParenthesis,
                CLoseParenthesis = item.CloseParenthesis
            };
        }

        private static BHExpressionPart MapBranch(this ExpressionsData item)
        {
            return new BHExpressionPart
            {
                ExpType = item.OperationType,
                OpenParenthesis = item.OpenParenthesis,
                CLoseParenthesis = item.CloseParenthesis
            };
        }

        private static BHExpressionPart MapDoubleEdge(this ExpressionsData item)
        {
            return new BHExpressionPart
            {
                LeftName = item.LeftMember?.ToString().Split('.')[1],
                RightName = item.RightMember?.ToString().Split(".")[1],
                ExpType = item.OperationType,
                OpenParenthesis = item.OpenParenthesis,
                CLoseParenthesis = item.CloseParenthesis
            };
        }

        private static bool IsBranch(this ExpressionsData item) => item.MemberValue == null && item.MethodData.Count == 0 && item.OperationType != ExpressionType.Default;

        private static bool IsEdge(this ExpressionsData item) => item.MemberValue != null || item.MethodData.Count > 0 || (item.MemberValue == null && item.IsNullValue);

        private static bool IsDoubleEdge(this ExpressionsData item) => item.MemberValue == null && item.LeftMember != null && item.RightMember != null && item.MethodData.Count == 0;

        private static bool IsConnector(this ExpressionType item) => item == ExpressionType.AndAlso || item == ExpressionType.OrElse;

        public static BHExpressionPart[] ParseExpression<T>(this Expression<Func<T, bool>> completeExpression)
        {
            List<ExpressionsData> expressionTree = [];

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

            int currentIndex = 0;
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
                bool addToTree = false;

                if (expressionTree[currentIndex].Operation != null)
                {
                    if (expressionTree[currentIndex].OperationType.IsConnector())
                    {
                        BinaryExpression? leftOperation = expressionTree[currentIndex].Operation?.Left as BinaryExpression;
                        BinaryExpression? rightOperation = expressionTree[currentIndex].Operation?.Right as BinaryExpression;
                        MethodCallExpression? leftCallOperation = expressionTree[currentIndex].Operation?.Left as MethodCallExpression;
                        MethodCallExpression? rightCallOperation = expressionTree[currentIndex].Operation?.Right as MethodCallExpression;

                        if (!expressionTree[currentIndex].LeftChecked)
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
                                    ParentIndex = currentIndex,
                                    CloseParenthesis = expressionTree[currentIndex].CloseParenthesis + 1
                                });
                                expressionTree[currentIndex].LeftChecked = true;
                                expressionTree[currentIndex].CloseParenthesis = 0;
                                addToTree = true;
                            }
                            else if (expressionTree[currentIndex].Operation?.Left is UnaryExpression uExp)
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
                                        ParentIndex = currentIndex,
                                        LeftMember = leftExp,
                                        CloseParenthesis = expressionTree[currentIndex].CloseParenthesis + 1
                                    });
                                    expressionTree[currentIndex].LeftChecked = true;
                                    expressionTree[currentIndex].CloseParenthesis = 0;
                                    addToTree = true;
                                }
                            }
                            else if (expressionTree[currentIndex].Operation?.Left is MemberExpression leftExp)
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
                                    ParentIndex = currentIndex,
                                    LeftMember = leftExp,
                                    CloseParenthesis = expressionTree[currentIndex].CloseParenthesis + 1
                                });
                                expressionTree[currentIndex].LeftChecked = true;
                                expressionTree[currentIndex].CloseParenthesis = 0;
                                addToTree = true;
                            }
                        }

                        if (!expressionTree[currentIndex].RightChecked)
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
                                    ParentIndex = currentIndex,
                                    OpenParenthesis = expressionTree[currentIndex].OpenParenthesis + 1
                                });
                                expressionTree[currentIndex].RightChecked = true;
                                expressionTree[currentIndex].OpenParenthesis = 0;
                                addToTree = true;
                            }
                            else if (expressionTree[currentIndex].Operation?.Right is UnaryExpression uExp)
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
                                        ParentIndex = currentIndex,
                                        LeftMember = rightExp,
                                        OpenParenthesis = expressionTree[currentIndex].OpenParenthesis + 1
                                    });
                                    expressionTree[currentIndex].RightChecked = true;
                                    expressionTree[currentIndex].OpenParenthesis = 0;
                                    addToTree = true;
                                }
                            }
                            else if (expressionTree[currentIndex].Operation?.Right is MemberExpression rightExp)
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
                                    ParentIndex = currentIndex,
                                    LeftMember = rightExp,
                                    OpenParenthesis = expressionTree[currentIndex].OpenParenthesis + 1
                                });
                                expressionTree[currentIndex].RightChecked = true;
                                expressionTree[currentIndex].OpenParenthesis = 0;
                                addToTree = true;
                            }
                        }

                        if (addToTree)
                        {
                            currentIndex = expressionTree.Count - 1;
                        }
                    }
                    else
                    {
                        if (!expressionTree[currentIndex].RightChecked)
                        {
                            if (expressionTree[currentIndex].Operation?.Right is MemberExpression rightMember)
                            {
                                expressionTree[currentIndex].InvokeOrTake<T>(rightMember, true);
                            }

                            if (expressionTree[currentIndex].Operation?.Right is ConstantExpression rightConstant)
                            {
                                expressionTree[currentIndex].MemberValue = rightConstant?.Value;
                                expressionTree[currentIndex].IsNullValue = rightConstant?.Value == null;
                            }

                            if (expressionTree[currentIndex].Operation?.Right is BinaryExpression rightBinary)
                            {
                                expressionTree[currentIndex].MemberValue = Expression.Lambda(rightBinary).Compile().DynamicInvoke();
                                expressionTree[currentIndex].IsNullValue = expressionTree[currentIndex].MemberValue == null;
                            }

                            if (expressionTree[currentIndex].Operation?.Right is MethodCallExpression rightMethodMember)
                            {
                                expressionTree[currentIndex].RightMethodMember = rightMethodMember;
                            }

                            if (expressionTree[currentIndex].Operation?.Right is UnaryExpression unaryMember)
                            {
                                expressionTree[currentIndex].MemberValue = Expression.Lambda(unaryMember).Compile().DynamicInvoke();
                            }

                            expressionTree[currentIndex].RightChecked = true;
                        }

                        if (!expressionTree[currentIndex].LeftChecked)
                        {
                            if (expressionTree[currentIndex].Operation?.Left is MemberExpression leftMember)
                            {
                                expressionTree[currentIndex].InvokeOrTake<T>(leftMember, false);
                            }

                            if (expressionTree[currentIndex].Operation?.Left is ConstantExpression leftConstant)
                            {
                                expressionTree[currentIndex].MemberValue = leftConstant?.Value;
                                expressionTree[currentIndex].IsNullValue = leftConstant?.Value == null;
                            }

                            if (expressionTree[currentIndex].Operation?.Left is BinaryExpression leftBinary)
                            {
                                expressionTree[currentIndex].MemberValue = Expression.Lambda(leftBinary).Compile().DynamicInvoke();
                                expressionTree[currentIndex].IsNullValue = expressionTree[currentIndex].MemberValue == null;
                            }

                            if (expressionTree[currentIndex].Operation?.Left is MethodCallExpression leftMethodMember)
                            {
                                expressionTree[currentIndex].LeftMethodMember = leftMethodMember;
                            }

                            if (expressionTree[currentIndex].Operation?.Left is UnaryExpression unaryMember)
                            {
                                expressionTree[currentIndex].MemberValue = Expression.Lambda(unaryMember).Compile().DynamicInvoke();
                            }

                            expressionTree[currentIndex].LeftChecked = true;
                        }
                    }
                }

                if (expressionTree[currentIndex].MethodData.Count == 0)
                {
                    MethodCallExpression? leftMethodMember = expressionTree[currentIndex].LeftMethodMember;
                    MethodCallExpression? rightMethodMember = expressionTree[currentIndex].RightMethodMember;

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

                        if (!expressionTree[currentIndex].MethodChecked)
                        {
                            List<object?> MethodArguments = [];
                            object?[] parameters = new object[arguments.Count];

                            for (int i = 0; i < arguments.Count; i++)
                            {
                                if (arguments[i] is MemberExpression argMember)
                                {
                                    string? typeName = argMember.Member.ReflectedType?.FullName;

                                    if (typeName != null && (typeName == typeof(T).BaseType?.FullName || typeName == typeof(T).FullName))
                                    {
                                        cleanOfMembers = false;
                                        obj = argMember;
                                        MethodArguments.Add(argMember.Member);
                                    }
                                    else
                                    {
                                        parameters[i] = Expression.Lambda(argMember).Compile().DynamicInvoke();
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
                                        MemberExpression? SubArgMember = arg as MemberExpression;

                                        if (SubArgMember?.Member.ReflectedType?.FullName == typeof(T).FullName)
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
                                    expressionTree[currentIndex].RightMember = argLambda.Body as MemberExpression;
                                }
                            }

                            if (cleanOfMembers)
                            {
                                if (obj != null)
                                {
                                    object? item = obj.Type != typeof(string) ? Activator.CreateInstance(obj.Type, null) : string.Empty;
                                    expressionTree[currentIndex].MemberValue = func.Invoke(item, parameters);
                                }
                            }
                            else
                            {
                                expressionTree[currentIndex].MethodData.Add(new MethodExpressionData
                                {
                                    MethodName = func.Name,
                                    MethodArguments = MethodArguments,
                                    CastedOn = obj,
                                    ComparedValue = expressionTree[currentIndex].MemberValue,
                                    CompareProperty = expressionTree[currentIndex].RightMember,
                                    OperatorType = expressionTree[currentIndex].OperationType,
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

                        if (!expressionTree[currentIndex].MethodChecked)
                        {
                            List<object?> MethodArguments = [];
                            object?[] parameters = new object[arguments.Count];

                            for (int i = 0; i < arguments.Count; i++)
                            {
                                if (arguments[i] is MemberExpression argMember)
                                {
                                    string? typeName = argMember.Member.ReflectedType?.FullName;

                                    if (typeName != null && (typeName == typeof(T).BaseType?.FullName || typeName == typeof(T).FullName))
                                    {
                                        cleanOfMembers = false;
                                        obj = argMember;
                                        MethodArguments.Add(argMember.Member);
                                    }
                                    else
                                    {
                                        parameters[i] = Expression.Lambda(argMember).Compile().DynamicInvoke();
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
                                        MemberExpression? SubArgMember = arg as MemberExpression;

                                        if (SubArgMember?.Member.ReflectedType?.FullName == typeof(T).FullName)
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
                                    expressionTree[currentIndex].LeftMember = argLambda.Body as MemberExpression;
                                }
                            }

                            if (cleanOfMembers)
                            {
                                if (obj != null)
                                {
                                    object? Item = obj.Type != typeof(string) ? Activator.CreateInstance(obj.Type, null) : string.Empty;
                                    expressionTree[currentIndex].MemberValue = func.Invoke(Item, parameters);
                                }
                            }
                            else
                            {
                                expressionTree[currentIndex].MethodData.Add(new MethodExpressionData
                                {
                                    MethodName = func.Name,
                                    MethodArguments = MethodArguments,
                                    CastedOn = obj,
                                    ComparedValue = expressionTree[currentIndex].MemberValue,
                                    CompareProperty = expressionTree[currentIndex].LeftMember,
                                    OperatorType = expressionTree[currentIndex].OperationType,
                                    ReverseOperator = false,
                                    TableName = typeof(T).Name
                                });
                            }
                        }
                    }
                }

                if (!addToTree)
                {
                    int parentSpot = expressionTree[currentIndex].ParentIndex;

                    if (expressionTree[currentIndex].FinalPosition < 0)
                    {
                        expressionTree[currentIndex].FinalPosition = positionIndex;
                        positionIndex++;
                    }

                    if (parentSpot != -1 && expressionTree[parentSpot].FinalPosition < 0)
                    {
                        expressionTree[parentSpot].FinalPosition = positionIndex;
                        positionIndex++;
                    }

                    currentIndex -= 1;
                }

                if (currentIndex < 0)
                {
                    startTranslate = false;
                }
            }

            return expressionTree.OrderExpressionTree<T>();
        }
    }
}
