using System.Linq.Expressions;

namespace BlackHole.CoreSupport
{
    internal class SqlFunctionsReader
    {
        public string SqlCommand { get; set; } = " 1=1 ";
        internal object? ParameterValue { get; set; }
        internal string Letter { get; set; } = string.Empty;

        internal SqlFunctionsReader(MethodExpressionData MethodData, int index, string? letter)
        {
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
                    SqlCommand = $"{NumericMethodData.CompareProperty} >= Average({NumericMethodData.CastedOn?.ToString()}) ";
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
                    break;
                case "SqlLike":
                    SqlCommand = $"{StringMethodData.CastedOn?.ToString()} Like '{StringMethodData.MethodArguments[1]}'";
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
    }
}
