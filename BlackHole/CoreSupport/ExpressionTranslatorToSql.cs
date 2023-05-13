using System.Linq.Expressions;
using System.Reflection;

namespace BlackHole.CoreSupport
{
    internal static class ExpressionTranslatorToSql
    {
        internal static ColumnsAndParameters SplitMembers<T>(this Expression expression, bool isMyShit, string? letter, List<BlackHoleParameter>? DynamicParams, int index)
        {
            List<ExpressionsData> expressionTree = new List<ExpressionsData>();

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
                            MemberExpression? rightMember = expressionTree[currentIndx].operation?.Right as MemberExpression;
                            ConstantExpression? rightConstant = expressionTree[currentIndx].operation?.Right as ConstantExpression;
                            BinaryExpression? rightBinary = expressionTree[currentIndx].operation?.Right as BinaryExpression;
                            MethodCallExpression? rightmethodMember = expressionTree[currentIndx].operation?.Right as MethodCallExpression;

                            if (rightMember != null)
                            {
                                if (rightMember.Member.ReflectedType?.FullName == typeof(T).FullName)
                                {
                                    expressionTree[currentIndx].rightMember = rightMember;
                                }
                                else
                                {
                                    expressionTree[currentIndx].memberValue = Expression.Lambda(rightMember).Compile().DynamicInvoke();
                                }
                            }

                            if (rightConstant != null)
                            {
                                expressionTree[currentIndx].memberValue = rightConstant?.Value;
                            }

                            if (rightBinary != null)
                            {
                                expressionTree[currentIndx].memberValue = Expression.Lambda(rightBinary).Compile().DynamicInvoke();
                            }

                            if (rightmethodMember != null)
                            {
                                expressionTree[currentIndx].rightMethodMember = rightmethodMember;
                            }

                            expressionTree[currentIndx].rightChecked = true;
                        }

                        if (!expressionTree[currentIndx].leftChecked)
                        {
                            MemberExpression? leftMember = expressionTree[currentIndx].operation?.Left as MemberExpression;
                            ConstantExpression? leftConstant = expressionTree[currentIndx].operation?.Left as ConstantExpression;
                            BinaryExpression? leftBinary = expressionTree[currentIndx].operation?.Left as BinaryExpression;
                            MethodCallExpression? leftmethodMember = expressionTree[currentIndx].operation?.Left as MethodCallExpression;

                            if (leftMember != null)
                            {
                                if (leftMember.Member.ReflectedType?.FullName == typeof(T).FullName)
                                {
                                    expressionTree[currentIndx].leftMember = leftMember;
                                }
                                else
                                {
                                    expressionTree[currentIndx].memberValue = Expression.Lambda(leftMember).Compile().DynamicInvoke();
                                }
                            }

                            if (leftConstant != null)
                            {
                                expressionTree[currentIndx].memberValue = leftConstant?.Value;
                            }

                            if (leftBinary != null)
                            {
                                expressionTree[currentIndx].memberValue = Expression.Lambda(leftBinary).Compile().DynamicInvoke();
                            }

                            if (leftmethodMember != null)
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

                        if (!expressionTree[currentIndx].methodChecked)
                        {
                            List<object?> MethodArguments = new List<object?>();
                            bool cleanOfMembers = true;
                            object?[] parameters = new object[arguments.Count];

                            for (int i = 0; i < arguments.Count; i++)
                            {
                                MemberExpression? argMemmber = arguments[i] as MemberExpression;
                                ConstantExpression? argConstant = arguments[i] as ConstantExpression;
                                BinaryExpression? argBinary = arguments[i] as BinaryExpression;
                                MethodCallExpression? argMethod = arguments[i] as MethodCallExpression;

                                if (argMemmber != null)
                                {
                                    if (argMemmber.Member.ReflectedType?.FullName == typeof(T).FullName)
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

                                if (argConstant != null)
                                {
                                    parameters[i] = argConstant.Value;
                                    MethodArguments.Add(argConstant.Value);
                                }

                                if (argBinary != null)
                                {
                                    parameters[i] = Expression.Lambda(argBinary).Compile().DynamicInvoke();
                                    MethodArguments.Add(parameters[i]);
                                }

                                if (argMethod != null)
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
                            }

                            if (cleanOfMembers)
                            {
                                if (obj != null)
                                {
                                    object? skata = Activator.CreateInstance(obj.Type, null);
                                    expressionTree[currentIndx].memberValue = func.Invoke(skata, parameters);
                                }
                            }
                            else
                            {
                                expressionTree[currentIndx].methodData.Add(new MethodExpressionData { MethodName = func.Name, MethodArguments = MethodArguments, CastedOn = obj });
                            }
                        }
                    }

                    if (rightMethodMember != null)
                    {
                        var func = rightMethodMember.Method;
                        var arguments = rightMethodMember.Arguments;
                        var obj = rightMethodMember.Object;

                        if (!expressionTree[currentIndx].methodChecked)
                        {
                            List<object?> MethodArguments = new List<object?>();
                            bool cleanOfMembers = true;
                            object?[] parameters = new object[arguments.Count];

                            for (int i = 0; i < arguments.Count; i++)
                            {
                                MemberExpression? argMemmber = arguments[i] as MemberExpression;
                                ConstantExpression? argConstant = arguments[i] as ConstantExpression;
                                BinaryExpression? argBinary = arguments[i] as BinaryExpression;
                                MethodCallExpression? argMethod = arguments[i] as MethodCallExpression;

                                if (argMemmber != null)
                                {
                                    if (argMemmber.Member.ReflectedType?.FullName == typeof(T).FullName)
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

                                if (argConstant != null)
                                {
                                    parameters[i] = argConstant.Value;
                                    MethodArguments.Add(argConstant.Value);
                                }

                                if (argBinary != null)
                                {
                                    parameters[i] = Expression.Lambda(argBinary).Compile().DynamicInvoke();
                                    MethodArguments.Add(parameters[i]);
                                }

                                if (argMethod != null)
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
                            }

                            if (cleanOfMembers)
                            {
                                if (obj != null)
                                {
                                    object? skata = Activator.CreateInstance(obj.Type, null);
                                    expressionTree[currentIndx].memberValue = func.Invoke(skata, parameters);
                                }
                            }
                            else
                            {
                                expressionTree[currentIndx].methodData.Add(new MethodExpressionData { MethodName = func.Name, MethodArguments = MethodArguments, CastedOn = obj });
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

        internal static ColumnsAndParameters ExpressionTreeToSql(this List<ExpressionsData> data, bool isMyShit, string? letter, List<BlackHoleParameter>? parameters, int index)
        {
            string result = "";

            if(parameters == null)
            {
                parameters = new List<BlackHoleParameter>();
            }

            List<ExpressionsData> children = data.Where(x => x.memberValue != null).ToList();
            string[] translations = new string[children.Count];

            foreach (ExpressionsData child in children)
            {
                ExpressionsData parent = data[child.parentIndex];
                if (parent.leftChecked)
                {
                    if (child.methodData.Count > 0)
                    {

                    }
                    else
                    {
                        ColumnAndParameter childParams = child.TranslateExpression(index, isMyShit, letter);

                        if (childParams.ParamName != string.Empty)
                        {
                            parameters.Add(new BlackHoleParameter { Name = childParams.ParamName, Value = childParams.Value });
                        }

                        parent.sqlCommand = $"{childParams.Column}";
                    }

                    parent.leftChecked = false;
                    index++;
                }
                else
                {
                    if (child.methodData.Count > 0)
                    {

                    }
                    else
                    {
                        ColumnAndParameter parentCols = parent.TranslateExpression(index, isMyShit, letter);

                        if (parentCols.ParamName != string.Empty)
                        {
                            parameters.Add( new BlackHoleParameter { Name = parentCols.ParamName, Value = parentCols.Value });
                        }

                        index++;

                        ColumnAndParameter childCols = child.TranslateExpression(index, isMyShit, letter);

                        if (childCols.ParamName != string.Empty)
                        {
                            parameters.Add(new BlackHoleParameter { Name = childCols.ParamName, Value = childCols.Value });
                        }

                        parent.sqlCommand = $"({parent.sqlCommand} {parentCols.Column} {childCols.Column})";
                    }

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
                        if (parent.methodData.Count > 0)
                        {

                        }
                        else
                        {
                            ColumnAndParameter parentParams = parent.TranslateExpression(index, isMyShit, letter);
                            if (parentParams.ParamName != string.Empty)
                            {
                                parameters.Add(new BlackHoleParameter { Name = parentParams.ParamName, Value = parentParams.Value });
                            }

                            parent.sqlCommand = $"({parent.sqlCommand} {parentParams.Column} {parents[parentsCount - 1 - i].sqlCommand})";
                        }

                        index++;
                    }
                }
            }

            result = data[0].sqlCommand;

            return new ColumnsAndParameters { Columns = result, Parameters = parameters, Count = index};
        }

        private static ColumnAndParameter TranslateExpression(this ExpressionsData expression, int index, bool isMyShit, string? letter)
        {
            string? column = string.Empty;
            string? parameter = string.Empty;
            object? value = new object();
            string[]? variable = new string[2];
            string subLetter = letter != string.Empty ? $"{letter}." : string.Empty;

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
                    variable = expression?.leftMember != null ? expression?.leftMember.ToString().Split(".") : expression?.rightMember?.ToString().Split(".");
                    column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} >= @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.LessThanOrEqual:
                    variable = expression?.leftMember != null ? expression?.leftMember.ToString().Split(".") : expression?.rightMember?.ToString().Split(".");
                    column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} <= @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.LessThan:
                    variable = expression?.leftMember?.ToString().Split(".");
                    column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} < @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.GreaterThan:
                    variable = expression?.leftMember != null ? expression?.leftMember.ToString().Split(".") : expression?.rightMember?.ToString().Split(".");
                    column = $"{subLetter}{variable?[1].SkipNameQuotes(isMyShit)} > @{variable?[1]}{index}";
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
            }

            return new ColumnAndParameter { Column = column, ParamName = parameter, Value = value};
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
