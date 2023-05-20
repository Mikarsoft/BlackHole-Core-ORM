using System.Linq.Expressions;

namespace BlackHole.CoreSupport
{
    internal class SqlFunctionsReader
    {
        public string SqlCommand { get; set; } = " 1=1 ";
        internal object? Value { get; set; }
        internal string ParamName { get; set; } = string.Empty;
        internal string Letter { get; set; } = string.Empty;
        internal int Index { get; set; }
        internal bool SkipQuotes { get; set; }

        internal SqlFunctionsReader(MethodExpressionData MethodData, int index, string? letter, bool isMyShit)
        {
            if (!string.IsNullOrWhiteSpace(letter))
            {
                Letter = $"{letter}.";
            }

            SkipQuotes = isMyShit;
            Index = index;

            if(MethodData.CastedOn != null)
            {
                if(MethodData.CastedOn.Type == typeof(string))
                {
                    UseStringMethods(MethodData);
                }
                else if(MethodData.CastedOn.Type == typeof(DateTime))
                {
                    UseDateTimeMethods(MethodData);
                }
                else
                {
                    UseNumericMethods(MethodData);
                }
            }
        }

        internal void UseNumericMethods(MethodExpressionData NumericMethodData)
        {
            switch (NumericMethodData.MethodName)
            {
                case "SqlAverage":
                    if(NumericMethodData.CompareProperty != null && NumericMethodData.CastedOn != null)
                    {
                        string[] tableProperty = NumericMethodData.CastedOn != null ? NumericMethodData.CastedOn.ToString().Split(".") : new string[0];
                        string[] compareProperty = NumericMethodData.CompareProperty != null ? NumericMethodData.CompareProperty.ToString().Split(".") : new string[0];
                        if(tableProperty.Length >1 && compareProperty.Length > 1)
                        {
                            string operationType = ExpressionTypeToSql(NumericMethodData.OperatorType, NumericMethodData.ReverseOperator, false);
                            string SelectAverage = $"( Select AVG({tableProperty[1].SkipNameQuotes(SkipQuotes)}) from {NumericMethodData.TableName.SkipNameQuotes(SkipQuotes)} )";
                            SqlCommand = $" {Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)} {operationType} {SelectAverage} ";
                        }
                    }
                    break;
                case "SqlAbsolut":
                    break;
                case "SqlFloor":
                    break;
                case "SqlCeiling":
                    break;
                case "Remainder":
                    break;
                case "SqlPower":
                    break;
                case "SqlMax":
                    break;
                case "SqlMin":
                    break;
                case "SqlSum":
                    break;
            }
        }

        internal void UseStringMethods(MethodExpressionData StringMethodData)
        {
            switch (StringMethodData.MethodName)
            {
                case "Contains":
                    if (StringMethodData.MethodArguments.Count == 1)
                    {
                        string[] tableProperty = StringMethodData.CastedOn != null ? StringMethodData.CastedOn.ToString().Split(".") : new string[0];
                        if (tableProperty.Length > 1)
                        {
                            ParamName = $"{tableProperty[1]}{Index}";
                            Value = $"%{StringMethodData.MethodArguments[0]}%";
                            SqlCommand = $"{Letter}{tableProperty[1].SkipNameQuotes(SkipQuotes)} Like @{ParamName}";
                        }
                    }
                    break;
                case "SqlLike":
                    if(StringMethodData.MethodArguments.Count == 2)
                    {
                        string[] tableProperty = StringMethodData.CastedOn != null ? StringMethodData.CastedOn.ToString().Split(".") : new string[0];
                        if(tableProperty.Length > 1)
                        {
                            ParamName = $"{tableProperty[1]}{Index}";
                            Value = StringMethodData.MethodArguments[1];
                            SqlCommand = $"{Letter}{tableProperty[1].SkipNameQuotes(SkipQuotes)} Like @{ParamName}";
                        }
                    }
                    break;
                case "SqlConcat":
                    break;
                case "ToUpper":
                    break;
                case "ToLower":
                    break;
                case "SqlUpper":
                    break;
                case "SqlLower":
                    break;
                case "SqlRight":
                    break;
                case "SqlLeft":
                    break;
                case "Replace":
                    break;
                case "Length":
                    break;
                case "SqlReverse":
                    break;
            }
        }


        internal void UseDateTimeMethods(MethodExpressionData DateMethodData)
        {
            switch (DateMethodData.MethodName)
            {
                case "SqlDateAfter":
                    break;
                case "SqlDateBefore":
                    break;
            }
        }

        private string ExpressionTypeToSql(ExpressionType ExpType , bool IsReversed, bool IsNullValue)
        {
            string expOperator = string.Empty;

            switch (ExpType)
            {
                case ExpressionType.Equal:
                    expOperator = IsNullValue ? "is" : "=";
                    break;
                case ExpressionType.NotEqual:
                    expOperator = IsNullValue ? "is not" : "!=";
                    break;
                case ExpressionType.GreaterThan:
                    expOperator = IsReversed ? "<" : ">";
                    break;
                case ExpressionType.LessThan:
                    expOperator = IsReversed ? ">" : "<";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    expOperator = IsReversed ? "<=" : ">=";
                    break;
                case ExpressionType.LessThanOrEqual:
                    expOperator = IsReversed ? ">=" : "<=";
                    break;
                default:
                    expOperator = string.Empty;
                    break;
            }

            return expOperator;
        }
    }
}
