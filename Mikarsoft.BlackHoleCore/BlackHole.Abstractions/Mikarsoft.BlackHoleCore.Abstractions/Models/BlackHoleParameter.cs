namespace Mikarsoft.BlackHoleCore.Abstractions.Models
{
    internal class BlackHoleParameter
    {
        internal BlackHoleParameter(string pramaName, object? paramValue)
        {
            Name = pramaName;
            Value = paramValue;
        }

        internal string Name { get; set; }
        internal object? Value { get; set; }
    }
}
