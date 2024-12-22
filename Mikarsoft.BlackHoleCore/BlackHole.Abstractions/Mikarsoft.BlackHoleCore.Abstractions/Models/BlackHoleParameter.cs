namespace Mikarsoft.BlackHoleCore.Abstractions.Models
{
    public class BlackHoleParameter
    {
        internal BlackHoleParameter(string pramaName, object? paramValue)
        {
            Name = pramaName;
            Value = paramValue;
        }

        public string Name { get; set; }
        public object? Value { get; set; }
    }
}
