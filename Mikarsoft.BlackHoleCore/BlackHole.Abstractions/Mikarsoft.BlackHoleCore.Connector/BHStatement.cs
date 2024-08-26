using Mikarsoft.BlackHoleCore.Connector.Statements;

namespace Mikarsoft.BlackHoleCore.Connector
{
    public class BHStatement
    {
        public BHStatement(int parameterStartingIndex)
        {
            ParameterIndex = parameterStartingIndex;
        }

        public string Command { get; set; } = string.Empty;
        public int ParameterIndex { get; set; }
        public List<BlackHoleInnerParameter> InnerParameters { get; set; } = new();

        public string AddParameter(object? Value)
        {
            string parameterName = $"ParVal{ParameterIndex}";

            InnerParameters.Add(new BlackHoleInnerParameter(parameterName, Value));

            ParameterIndex++;

            return parameterName;
        }
    }


    public class BHStatement<T> where T : class
    {
        public JoinStatement<T>? JoinStatement { get; set; }
    }
}
