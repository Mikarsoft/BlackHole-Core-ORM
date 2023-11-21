using BlackHole.Core;
using BlackHole.Entities;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace BlackHole.CoreSupport
{
    internal static class ExpressionTranslatorToSql
    {
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

        internal static JoinsData<Dto, T, TOther> CreateFirstJoin<T, TOther, Dto>(this List<string> Columns,LambdaExpression key, LambdaExpression otherKey, string joinType, string tableSchema, bool IsMyShit, bool OpenEntity)
        {
            string? parameter = key.Parameters[0].Name;
            MemberExpression? member = key.Body as MemberExpression;
            string? propName = member?.Member.Name;
            string? parameterOther = otherKey.Parameters[0].Name;
            MemberExpression? memberOther = otherKey.Body as MemberExpression;
            string? propNameOther = memberOther?.Member.Name;

            JoinsData<Dto, T, TOther> firstJoin = new()
            {
                isMyShit = IsMyShit,
                BaseTable = typeof(T),
                Ignore = false
            };

            firstJoin.TablesToLetters.Add(new TableLetters { Table = typeof(T), Letter = parameter, IsOpenEntity = OpenEntity });
            firstJoin.Letters.Add(parameter);

            if (parameterOther == parameter)
            {
                parameterOther += firstJoin.HelperIndex.ToString();
                firstJoin.HelperIndex++;
            }

            bool isOpen = typeof(TOther).GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(BHOpenEntity<>));
            firstJoin.TablesToLetters.Add(new TableLetters { Table = typeof(TOther), Letter = parameterOther, IsOpenEntity = isOpen });
            firstJoin.Letters.Add(parameterOther);

            firstJoin.Joins = $" {joinType} join {tableSchema}{typeof(TOther).Name.SkipNameQuotes(IsMyShit)} {parameterOther} on {parameterOther}.{propNameOther.SkipNameQuotes(IsMyShit)} = {parameter}.{propName.SkipNameQuotes(IsMyShit)}";
            firstJoin.OccupiedDtoProps = Columns.BindPropertiesToDto<T>(typeof(TOther), typeof(Dto), parameter, parameterOther);
            return firstJoin;
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
