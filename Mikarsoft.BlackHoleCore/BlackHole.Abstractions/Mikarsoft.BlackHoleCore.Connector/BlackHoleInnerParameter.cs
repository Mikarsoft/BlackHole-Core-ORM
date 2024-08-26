namespace Mikarsoft.BlackHoleCore.Connector
{
    public class BlackHoleInnerParameter
    {
        public BlackHoleInnerParameter(string paramName, object? paramValue)
        {
            Name = paramName;
            Value = paramValue;
        }

        public string Name { get; set; }
        public object? Value { get; set; }
    }
}
