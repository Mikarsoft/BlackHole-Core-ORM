namespace BlackHole.CoreSupport
{
    internal class SqlFunctionsReader
    {
        internal string SqlCommand { get; set; }
        internal object? ParameterValue { get; set; }
        internal string Letter { get; set; }

        internal SqlFunctionsReader(MethodExpressionData MethodData)
        {
            switch (MethodData.MethodName)
            {
                case "Contains":
                    break;
                case "SqlLike":
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
                case "SqlAverage":
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
                case "SqlDateAfter":
                    break;
                case "SqlDateBefore":
                    break;
            }
        }
    }
}
