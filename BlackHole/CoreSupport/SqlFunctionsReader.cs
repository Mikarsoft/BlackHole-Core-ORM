using BlackHole.Statics;
using System.Linq.Expressions;
using System.Reflection;
using BlackHole.Logger;

namespace BlackHole.CoreSupport
{
    internal class SqlFunctionsReader
    {
        public string SqlCommand { get; set; } = " 1 != 1 ";
        internal object? Value { get; set; }
        internal string ParamName { get; set; } = string.Empty;
        internal string Letter { get; set; } = string.Empty;
        internal int Index { get; set; }
        internal bool SkipQuotes { get; set; }
        private bool WasTranslated { get; set; }
        internal string SchemaName { get; set; } = string.Empty;

        internal SqlFunctionsReader(MethodExpressionData MethodData, int index, string? letter, bool isMyShit)
        {
            if (!string.IsNullOrWhiteSpace(letter))
            {
                Letter = $"{letter}.";
            }

            if (!string.IsNullOrEmpty(DatabaseStatics.DatabaseSchema))
            {
                SchemaName = $"{DatabaseStatics.DatabaseSchema}.";
            }

            WasTranslated = false;
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

            if (!WasTranslated)
            {
                Task.Factory.StartNew(() => MethodData.MethodName.CreateErrorLogs($"SqlFunctionsReader_{MethodData.MethodName}", 
                    $"Unknown Method: {MethodData.MethodName}. It was translated into '1 != 1' to avoid data corruption.",
                    "Please read the documentation to find the supported Sql functions and the correct usage of them."));
            }
        }

        internal void UseNumericMethods(MethodExpressionData NumericMethodData)
        {
            switch (NumericMethodData.MethodName)
            {
                case "SqlEqualTo":
                    if (NumericMethodData.MethodArguments.Count == 2)
                    {
                        if (NumericMethodData.CompareProperty != null && NumericMethodData.CastedOn != null)
                        {
                            string? aTable = NumericMethodData.CompareProperty.Member?.ReflectedType?.Name;
                            string? PropertyName = NumericMethodData.CompareProperty.Member?.Name;
                            string[] compareProperty = NumericMethodData.CastedOn.ToString().Split(".");

                            if (aTable != null && PropertyName != null && compareProperty.Length > 1)
                            {
                                ParamName = $"OtherId{Index}";
                                Value = NumericMethodData.MethodArguments[1];
                                string SelectFromOtherTable = $"( Select {PropertyName.SkipNameQuotes(SkipQuotes)} from {SchemaName}{aTable.SkipNameQuotes(SkipQuotes)} where {"Id".SkipNameQuotes(SkipQuotes)}= @{ParamName} )";
                                SqlCommand = $" {Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)} = {SelectFromOtherTable} ";
                                WasTranslated = true;
                            }
                        }
                    }
                    break;
                case "SqlAverage":
                    if(NumericMethodData.CompareProperty != null && NumericMethodData.CastedOn != null)
                    {
                        string[] tableProperty = NumericMethodData.CastedOn != null ? NumericMethodData.CastedOn.ToString().Split(".") : Array.Empty<string>();
                        string[] compareProperty = NumericMethodData.CompareProperty != null ? NumericMethodData.CompareProperty.ToString().Split(".") : Array.Empty<string>();
                        if(tableProperty.Length >1 && compareProperty.Length > 1)
                        {
                            string operationType = ExpressionTypeToSql(NumericMethodData.OperatorType, NumericMethodData.ReverseOperator, false);
                            string SelectAverage = $"( Select AVG({tableProperty[1].SkipNameQuotes(SkipQuotes)}) from {SchemaName}{NumericMethodData.TableName.SkipNameQuotes(SkipQuotes)} )";
                            SqlCommand = $" {Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)} {operationType} {SelectAverage} ";
                            WasTranslated = true;
                        }
                    }
                    break;
                case "SqlAbsolut":
                    if (NumericMethodData.CastedOn != null)
                    {
                        string[] compareProperty = NumericMethodData.CastedOn.ToString().Split(".");

                        bool compareProp = false;

                        if (NumericMethodData.ComparedValue != null)
                        {
                            if (compareProperty.Length > 1)
                            {
                                string operationType = ExpressionTypeToSql(NumericMethodData.OperatorType, !NumericMethodData.ReverseOperator, false);
                                ParamName = $"{compareProperty[1]}{Index}";
                                Value = NumericMethodData.ComparedValue;
                                SqlCommand = $" ABS({Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)}) {operationType} @{ParamName} ";
                                compareProp = true;
                                WasTranslated = true;
                            }
                        }

                        if (!compareProp && NumericMethodData.CompareProperty != null)
                        {
                            string[] comparePropName = NumericMethodData.CompareProperty.ToString().Split(".");

                            if (compareProperty.Length > 1 && comparePropName.Length > 1)
                            {
                                string operationType = ExpressionTypeToSql(NumericMethodData.OperatorType, !NumericMethodData.ReverseOperator, false);
                                SqlCommand = $" ABS({Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)}) {operationType} {Letter}{comparePropName[1].SkipNameQuotes(SkipQuotes)} ";
                                WasTranslated = true;
                            }
                        }
                    }
                    break;
                case "SqlRound":
                    if (NumericMethodData.CastedOn != null)
                    {
                        string[] compareProperty = NumericMethodData.CastedOn.ToString().Split(".");

                        bool compareProp = false;
                        string decimalPoints = string.Empty;

                        if (NumericMethodData.MethodArguments.Count > 1)
                        {
                            decimalPoints = $",{NumericMethodData.MethodArguments[1]}";
                        }

                        if (NumericMethodData.ComparedValue != null)
                        {
                            if (compareProperty.Length > 1)
                            {
                                string operationType = ExpressionTypeToSql(NumericMethodData.OperatorType, !NumericMethodData.ReverseOperator, false);
                                ParamName = $"{compareProperty[1]}{Index}";
                                Value = NumericMethodData.ComparedValue;
                                SqlCommand = $" ROUND({Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)}{decimalPoints}) {operationType} @{ParamName} ";
                                compareProp = true;
                                WasTranslated = true;
                            }
                        }

                        if (!compareProp && NumericMethodData.CompareProperty != null)
                        {
                            string[] comparePropName = NumericMethodData.CompareProperty.ToString().Split(".");

                            if (compareProperty.Length > 1 && comparePropName.Length > 1)
                            {
                                string operationType = ExpressionTypeToSql(NumericMethodData.OperatorType, !NumericMethodData.ReverseOperator, false);
                                SqlCommand = $" ROUND({Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)}{decimalPoints}) {operationType} {Letter}{comparePropName[1].SkipNameQuotes(SkipQuotes)} ";
                                WasTranslated = true;
                            }
                        }
                    }
                    break;
                case "SqlMax":
                    if (NumericMethodData.CastedOn != null)
                    {
                        string[] tableProperty = NumericMethodData.CastedOn.ToString().Split(".");
                        if (tableProperty.Length > 1)
                        {
                            string SelectAverage = $"( Select MAX({tableProperty[1].SkipNameQuotes(SkipQuotes)}) from {SchemaName}{NumericMethodData.TableName.SkipNameQuotes(SkipQuotes)} )";
                            SqlCommand = $" {Letter}{tableProperty[1].SkipNameQuotes(SkipQuotes)} = {SelectAverage} ";
                            WasTranslated = true;
                        }
                    }
                    break;
                case "SqlMin":
                    if (NumericMethodData.CastedOn != null)
                    {
                        string[] tableProperty = NumericMethodData.CastedOn.ToString().Split(".");
                        if (tableProperty.Length > 1)
                        {
                            string SelectAverage = $"( Select MIN({tableProperty[1].SkipNameQuotes(SkipQuotes)}) from {SchemaName}{NumericMethodData.TableName.SkipNameQuotes(SkipQuotes)} )";
                            SqlCommand = $" {Letter}{tableProperty[1].SkipNameQuotes(SkipQuotes)} = {SelectAverage} ";
                            WasTranslated = true;
                        }
                    }
                    break;
                case "SqlPlus":
                    if(NumericMethodData.CastedOn != null && NumericMethodData.MethodArguments.Count > 1)
                    {
                        PropertyInfo? thePlusValue = NumericMethodData.MethodArguments[1] as PropertyInfo;
                        string[] compareProperty = NumericMethodData.CastedOn.ToString().Split(".");
                        string operationType = ExpressionTypeToSql(NumericMethodData.OperatorType, !NumericMethodData.ReverseOperator, false);
                        bool hasCompareValue = false;

                        if (compareProperty.Length > 1)
                        {
                            if (NumericMethodData.ComparedValue != null)
                            {
                                hasCompareValue = true;

                                if (thePlusValue != null)
                                {
                                    ParamName = $"{compareProperty[1]}{Index}";
                                    Value = NumericMethodData.ComparedValue;
                                    SqlCommand = $" ({Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)} + {Letter}{thePlusValue.Name.SkipNameQuotes(SkipQuotes)}) {operationType} @{ParamName} ";
                                    WasTranslated = true;
                                }
                                else
                                {
                                    ParamName = $"{compareProperty[1]}{Index}";
                                    Value = NumericMethodData.ComparedValue;
                                    SqlCommand = $" ({Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)} + {NumericMethodData.MethodArguments[1]}) {operationType} @{ParamName} ";
                                    WasTranslated = true;
                                }
                            }

                            if (!hasCompareValue && NumericMethodData.CompareProperty != null)
                            {
                                string[] otherPropName = NumericMethodData.CompareProperty.ToString().Split(".");

                                if(otherPropName.Length > 1)
                                {
                                    if (thePlusValue != null)
                                    {
                                        SqlCommand = $" ({Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)} + {Letter}{thePlusValue.Name.SkipNameQuotes(SkipQuotes)}) {operationType} {Letter}{otherPropName[1].SkipNameQuotes(SkipQuotes)} ";
                                    }
                                    else
                                    {
                                        SqlCommand = $" ({Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)} + {NumericMethodData.MethodArguments[1]}) {operationType} {Letter}{otherPropName[1].SkipNameQuotes(SkipQuotes)} ";
                                    }

                                    WasTranslated = true;
                                }
                            }
                        }
                    }
                    break;
                case "SqlMinus":
                    if (NumericMethodData.CastedOn != null && NumericMethodData.MethodArguments.Count > 1)
                    {
                        PropertyInfo? thePlusValue = NumericMethodData.MethodArguments[1] as PropertyInfo;
                        string[] compareProperty = NumericMethodData.CastedOn.ToString().Split(".");
                        string operationType = ExpressionTypeToSql(NumericMethodData.OperatorType, !NumericMethodData.ReverseOperator, false);
                        bool hasCompareValue = false;

                        if (compareProperty.Length > 1)
                        {
                            if (NumericMethodData.ComparedValue != null)
                            {
                                hasCompareValue = true;

                                if (thePlusValue != null)
                                {
                                    ParamName = $"{compareProperty[1]}{Index}";
                                    Value = NumericMethodData.ComparedValue;
                                    SqlCommand = $" ({Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)} + {Letter}{thePlusValue.Name.SkipNameQuotes(SkipQuotes)}) {operationType} @{ParamName} ";
                                }
                                else
                                {
                                    ParamName = $"{compareProperty[1]}{Index}";
                                    Value = NumericMethodData.ComparedValue;
                                    SqlCommand = $" ({Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)} + {NumericMethodData.MethodArguments[1]}) {operationType} @{ParamName} ";
                                }

                                WasTranslated = true;
                            }

                            if (!hasCompareValue && NumericMethodData.CompareProperty != null)
                            {
                                string[] otherPropName = NumericMethodData.CompareProperty.ToString().Split(".");

                                if (otherPropName.Length > 1)
                                {
                                    if (thePlusValue != null)
                                    {
                                        SqlCommand = $" ({Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)} - {Letter}{thePlusValue.Name.SkipNameQuotes(SkipQuotes)}) {operationType} {Letter}{otherPropName[1].SkipNameQuotes(SkipQuotes)} ";
                                    }
                                    else
                                    {
                                        SqlCommand = $" ({Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)} - {NumericMethodData.MethodArguments[1]}) {operationType} {Letter}{otherPropName[1].SkipNameQuotes(SkipQuotes)} ";
                                    }

                                    WasTranslated = true;
                                }
                            }
                        }
                    }
                    break;
                default:
                    WasTranslated = false;
                    break;
            }
        }

        internal void UseStringMethods(MethodExpressionData StringMethodData)
        {
            switch (StringMethodData.MethodName)
            {
                case "SqlEqualTo":
                    if(StringMethodData.MethodArguments.Count == 2)
                    {
                        if (StringMethodData.CompareProperty != null && StringMethodData.CastedOn != null)
                        {
                            string? aTable = StringMethodData.CompareProperty.Member?.ReflectedType?.Name;
                            string? PropertyName = StringMethodData.CompareProperty.Member?.Name;
                            string[] compareProperty = StringMethodData.CastedOn.ToString().Split(".");

                            if (aTable != null && PropertyName != null && compareProperty.Length > 1)
                            {
                                ParamName = $"OtherId{Index}";
                                Value = StringMethodData.MethodArguments[1];
                                string SelectFromOtherTable = $"( Select {PropertyName.SkipNameQuotes(SkipQuotes)} from {SchemaName}{aTable.SkipNameQuotes(SkipQuotes)} where {"Id".SkipNameQuotes(SkipQuotes)}= @{ParamName} )";
                                SqlCommand = $" {Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)} = {SelectFromOtherTable} ";
                                WasTranslated = true;
                            }
                        }
                    }
                    break;
                case "Contains":
                    if (StringMethodData.MethodArguments.Count == 1 && StringMethodData.CastedOn != null)
                    {
                        string[] tableProperty = StringMethodData.CastedOn.ToString().Split(".");
                        if (tableProperty.Length > 1)
                        {
                            ParamName = $"{tableProperty[1]}{Index}";
                            Value = $"%{StringMethodData.MethodArguments[0]}%";
                            SqlCommand = $" {Letter}{tableProperty[1].SkipNameQuotes(SkipQuotes)} Like @{ParamName}";
                            WasTranslated = true;
                        }
                    }
                    break;
                case "SqlLike":
                    if(StringMethodData.MethodArguments.Count == 2 && StringMethodData.CastedOn != null)
                    {
                        string[] tableProperty = StringMethodData.CastedOn.ToString().Split(".");
                        if(tableProperty.Length > 1)
                        {
                            ParamName = $"{tableProperty[1]}{Index}";
                            Value = StringMethodData.MethodArguments[1];
                            SqlCommand = $" {Letter}{tableProperty[1].SkipNameQuotes(SkipQuotes)} Like @{ParamName}";
                            WasTranslated = true;
                        }
                    }
                    break;
                case "ToUpper":
                    if (StringMethodData.ComparedValue != null && StringMethodData.CastedOn != null)
                    {
                        string[] compareProperty = StringMethodData.CastedOn.ToString().Split(".");

                        if (compareProperty.Length > 1)
                        {
                            string operationType = ExpressionTypeToSql(StringMethodData.OperatorType, StringMethodData.ReverseOperator, false);
                            ParamName = $"{compareProperty[1]}{Index}";
                            Value = StringMethodData.ComparedValue;
                            SqlCommand = $" UPPER({Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)}) {operationType} @{ParamName} ";
                            WasTranslated = true;
                        }
                    }
                    break;
                case "ToLower":
                    if(StringMethodData.ComparedValue != null && StringMethodData.CastedOn != null)
                    {
                        string[] compareProperty = StringMethodData.CastedOn.ToString().Split(".");

                        if(compareProperty.Length > 1)
                        {
                            string operationType = ExpressionTypeToSql(StringMethodData.OperatorType, StringMethodData.ReverseOperator, false);
                            ParamName = $"{compareProperty[1]}{Index}";
                            Value = StringMethodData.ComparedValue;
                            SqlCommand = $" LOWER({Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)}) {operationType} @{ParamName} ";
                            WasTranslated = true;
                        }
                    }
                    break;
                case "Replace":
                    if(StringMethodData.CastedOn != null && StringMethodData.MethodArguments.Count == 2 && StringMethodData.ComparedValue != null)
                    {
                        string[] compareProperty = StringMethodData.CastedOn.ToString().Split(".");

                        object? first = StringMethodData.MethodArguments[0];
                        object? second = StringMethodData.MethodArguments[1];

                        if (compareProperty.Length > 1)
                        {
                            string operationType = ExpressionTypeToSql(StringMethodData.OperatorType, StringMethodData.ReverseOperator, false);
                            ParamName = $"{compareProperty[1]}{Index}";
                            Value = StringMethodData.ComparedValue;
                            SqlCommand = $" REPLACE({Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)},'{first}','{second}') {operationType} @{ParamName} ";
                            WasTranslated = true;
                        }
                    }
                    break;
                case "SqlLength":
                    if (StringMethodData.ComparedValue != null && StringMethodData.CastedOn != null)
                    {
                        string[] compareProperty = StringMethodData.CastedOn.ToString().Split(".");

                        if (compareProperty.Length > 1)
                        {
                            string operationType = ExpressionTypeToSql(StringMethodData.OperatorType, !StringMethodData.ReverseOperator, false);
                            ParamName = $"{compareProperty[1]}{Index}";
                            Value = StringMethodData.ComparedValue;
                            SqlCommand = $" LENGTH({Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)}) {operationType} @{ParamName} ";
                            WasTranslated = true;
                        }
                    }
                    break;
                default:
                    WasTranslated = false;
                    break;
            }
        }


        internal void UseDateTimeMethods(MethodExpressionData DateMethodData)
        {
            switch (DateMethodData.MethodName)
            {
                case "SqlEqualTo":
                    if (DateMethodData.MethodArguments.Count == 2)
                    {
                        if (DateMethodData.CompareProperty != null && DateMethodData.CastedOn != null)
                        {
                            string? aTable = DateMethodData.CompareProperty.Member?.ReflectedType?.Name;
                            string? PropertyName = DateMethodData.CompareProperty.Member?.Name;
                            string[] compareProperty = DateMethodData.CastedOn.ToString().Split(".");

                            if (aTable != null && PropertyName != null && compareProperty.Length > 1)
                            {
                                ParamName = $"OtherId{Index}";
                                Value = DateMethodData.MethodArguments[1];
                                string SelectFromOtherTable = $"( Select {PropertyName.SkipNameQuotes(SkipQuotes)} from {SchemaName}{aTable.SkipNameQuotes(SkipQuotes)} where {"Id".SkipNameQuotes(SkipQuotes)}= @{ParamName} )";
                                SqlCommand = $" {Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)} = {SelectFromOtherTable} ";
                                WasTranslated = true;
                            }
                        }
                    }
                    break;
                case "SqlDateAfter":
                    if (DateMethodData.MethodArguments.Count == 2 && DateMethodData.CastedOn != null)
                    {
                        string[] compareProperty = DateMethodData.CastedOn.ToString().Split(".");

                        if(compareProperty.Length > 1)
                        {
                            ParamName = $"DateProp{Index}";
                            Value = DateMethodData.MethodArguments[1];
                            SqlCommand = $" {Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)} >= @{ParamName} ";
                            WasTranslated = true;
                        }
                    }
                    break;
                case "SqlDateBefore":
                    if (DateMethodData.MethodArguments.Count == 2 && DateMethodData.CastedOn != null)
                    {
                        string[] compareProperty = DateMethodData.CastedOn.ToString().Split(".");

                        if (compareProperty.Length > 1)
                        {
                            ParamName = $"DateProp{Index}";
                            Value = DateMethodData.MethodArguments[1];
                            SqlCommand = $" {Letter}{compareProperty[1].SkipNameQuotes(SkipQuotes)} < @{ParamName} ";
                            WasTranslated = true;
                        }
                    }
                    break;
                case "SqlMax":
                    if (DateMethodData.CastedOn != null)
                    {
                        string[] tableProperty = DateMethodData.CastedOn.ToString().Split(".");
                        if (tableProperty.Length > 1)
                        {
                            string SelectAverage = $"( Select MAX({tableProperty[1].SkipNameQuotes(SkipQuotes)}) from {SchemaName}{DateMethodData.TableName.SkipNameQuotes(SkipQuotes)} )";
                            SqlCommand = $" {Letter}{tableProperty[1].SkipNameQuotes(SkipQuotes)} = {SelectAverage} ";
                            WasTranslated = true;
                        }
                    }
                    break;
                case "SqlMin":
                    if (DateMethodData.CastedOn != null)
                    {
                        string[] tableProperty = DateMethodData.CastedOn.ToString().Split(".");
                        if (tableProperty.Length > 1)
                        {
                            string SelectAverage = $"( Select MIN({tableProperty[1].SkipNameQuotes(SkipQuotes)}) from {SchemaName}{DateMethodData.TableName.SkipNameQuotes(SkipQuotes)} )";
                            SqlCommand = $" {Letter}{tableProperty[1].SkipNameQuotes(SkipQuotes)} = {SelectAverage} ";
                            WasTranslated = true;
                        }
                    }
                    break;
                default:
                    WasTranslated = false;
                    break;
            }
        }

        private static string ExpressionTypeToSql(ExpressionType ExpType , bool IsReversed, bool IsNullValue)
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
    }
}
