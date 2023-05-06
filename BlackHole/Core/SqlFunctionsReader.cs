
namespace BlackHole.Core
{
    internal class SqlFunctionsReader
    {
        internal string functionName { get; set; }
        internal string propertyType { get; set; }
        internal object? dataValue { get; set; }
        internal bool methodNotFound { get; set; }

        internal SqlFunctionsReader(string MethodName)
        {
            switch (MethodName)
            {
                case "Contains":
                    break;
                case "Like":
                    break;
                case "Concat":
                    break;
                case "ToUpper":
                    break;
                case "ToLower":
                    break;
                case "Translate":
                    break;
                case "Replace":
                    break;
                case "Length":
                    break;
                case "Average":
                    break;
                case "Floor":
                    break;
                case "Remainder":
                    break;
                case "Max":
                    break;
                case "Min":
                    break;
                case "Power":
                    break;
                case "Sum":
                    break;
            }
        }
    }
}
