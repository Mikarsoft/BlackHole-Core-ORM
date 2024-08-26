using Mikarsoft.BlackHoleCore.Connector;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore
{
    internal static class BHExpressionParser
    {
        private static BHStatement? ParseMethod(this MethodExpressionData? bhMethod)
        {
            IBHMethodParser methodParser = BHServiceInjector.GetMethodParser();

            return bhMethod?.MethodName switch
            {
                nameof(methodParser.Contains) => methodParser.Contains(bhMethod),
                nameof(methodParser.ToUpper) => methodParser.ToUpper(bhMethod),
                nameof(methodParser.ToLower) => methodParser.ToLower(bhMethod),
                nameof(methodParser.Max) => methodParser.Max(bhMethod),
                nameof(methodParser.Min) => methodParser.Min(bhMethod),
                nameof(methodParser.Round) => methodParser.Round(bhMethod),
                nameof(methodParser.Average) => methodParser.Average(bhMethod),
                nameof(methodParser.Absolut) => methodParser.Absolut(bhMethod),
                nameof(methodParser.After) => methodParser.After(bhMethod),
                nameof(methodParser.Before) => methodParser.Before(bhMethod),
                nameof(methodParser.Between) => methodParser.Between(bhMethod),
                nameof(methodParser.Like) => methodParser.Like(bhMethod),
                nameof(methodParser.Length) => methodParser.Length(bhMethod),
                nameof(methodParser.GreaterThan) => methodParser.GreaterThan(bhMethod),
                nameof(methodParser.GreaterThanOrEqual) => methodParser.GreaterThanOrEqual(bhMethod),
                nameof(methodParser.LessThan) => methodParser.LessThan(bhMethod),
                nameof(methodParser.LessThanOrEqual) => methodParser.LessThanOrEqual(bhMethod),
                nameof(methodParser.Plus) => methodParser.Plus(bhMethod),
                nameof(methodParser.Minus) => methodParser.Minus(bhMethod),
                _ => throw new ArgumentException($"Method '{bhMethod?.MethodName}' is not supported by the BlackHole Parser.")
            };
        }

        private static void MapBHMethods(this BHExpressionPart expressionPart, List<MethodExpressionData> availableMethods, bool IsReversed)
        {
            if (availableMethods.Count == 0)
            {
                return;
            }

            if (availableMethods.Count > 1)
            {
                expressionPart.LeftMethod = availableMethods[0].ParseMethod();

                expressionPart.RightMethod = availableMethods[1].ParseMethod();
            }
            else
            {
                if (IsReversed)
                {
                    expressionPart.RightMethod = availableMethods[0].ParseMethod();
                }
                else
                {
                    expressionPart.LeftMethod = availableMethods[0].ParseMethod();
                }
            }
        }

        private static ExpressionType ReverseOperator(this ExpressionType expType)
        {
            return expType switch
            {
                ExpressionType.GreaterThan => ExpressionType.LessThan,
                ExpressionType.GreaterThanOrEqual => ExpressionType.LessThanOrEqual,
                ExpressionType.LessThan => ExpressionType.GreaterThan,
                ExpressionType.LessThanOrEqual => ExpressionType.GreaterThanOrEqual,
                _ => expType
            };
        }

        private static BHExpressionPart MapEdge(this BHExpressionData item, byte letter)
        {
            bool InReverse = item.IsReversed;

            BHExpressionPart edgePart = new BHExpressionPart(letter)
            {
                LeftName = item.LeftMember != null ? item.LeftMember.ToString().Split(".")[1] : item.RightMember?.ToString().Split(".")[1],
                RightName = InReverse ? null : item.RightMember?.ToString().Split(".")[1],
                Value = item.MemberValue,
                ExpType = InReverse ? item.OperationType.ReverseOperator() : item.OperationType,
                OpenParenthesis = item.OpenParenthesis,
                CLoseParenthesis = item.CloseParenthesis
            };

            edgePart.MapBHMethods(item.MethodData, InReverse);

            return edgePart;
        }

        private static BHExpressionPart MapBranch(this BHExpressionData item, byte letter)
        {
            return new BHExpressionPart(letter)
            {
                ExpType = item.OperationType,
                OpenParenthesis = item.OpenParenthesis,
                CLoseParenthesis = item.CloseParenthesis
            };
        }

        private static BHExpressionPart MapDoubleEdge(this BHExpressionData item, byte letter)
        {
            return new BHExpressionPart(letter)
            {
                LeftName = item.LeftMember?.ToString().Split('.')[1],
                RightName = item.RightMember?.ToString().Split(".")[1],
                ExpType = item.OperationType,
                OpenParenthesis = item.OpenParenthesis,
                CLoseParenthesis = item.CloseParenthesis
            };
        }

        private static BHExpressionPart[] OrderExpressionTree<T>(this List<BHExpressionData> data, byte letter)
        {
            var res = data.OrderBy(x => x.FinalPosition).ToList();
            BHExpressionPart[] parts = new BHExpressionPart[res.Count];

            for (int i = 0; i < data.Count; i++)
            {
                if (res[i].IsEdge)
                {
                    parts[i] = res[i].MapEdge(letter);
                    continue;
                }

                if (res[i].IsBranch)
                {
                    parts[i] = res[i].MapBranch(letter);
                    continue;
                }

                if (res[i].IsDoubleEdge)
                {
                    parts[i] = res[i].MapDoubleEdge(letter);
                }
            }

            return parts;
        }

        private static void InvokeOrTake<T>(this BHExpressionData thisBranch, MemberExpression memberExp, bool isRight)
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

        internal static BHExpressionPart[] ParseExpression<T>(this Expression<Func<T, bool>> completeExpression, byte letter = 0x00)
        {
            List<BHExpressionData> expressionTree = [];

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

                expressionTree.Add(new BHExpressionData()
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

                    expressionTree.Add(new BHExpressionData()
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

                expressionTree.Add(new BHExpressionData()
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
                    if (expressionTree[currentIndex].IsConnector)
                    {
                        BinaryExpression? leftOperation = expressionTree[currentIndex].Operation?.Left as BinaryExpression;
                        BinaryExpression? rightOperation = expressionTree[currentIndex].Operation?.Right as BinaryExpression;
                        MethodCallExpression? leftCallOperation = expressionTree[currentIndex].Operation?.Left as MethodCallExpression;
                        MethodCallExpression? rightCallOperation = expressionTree[currentIndex].Operation?.Right as MethodCallExpression;

                        if (!expressionTree[currentIndex].LeftChecked)
                        {
                            if (leftOperation != null || leftCallOperation != null)
                            {
                                expressionTree.Add(new BHExpressionData()
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

                                    expressionTree.Add(new BHExpressionData()
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

                                expressionTree.Add(new BHExpressionData()
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
                                expressionTree.Add(new BHExpressionData()
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

                                    expressionTree.Add(new BHExpressionData()
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

                                expressionTree.Add(new BHExpressionData()
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

                if (expressionTree[currentIndex].MethodData == null)
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

            return expressionTree.OrderExpressionTree<T>(letter);
        }
    }
}
