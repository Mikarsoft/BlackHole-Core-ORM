using BlackHole.CoreSupport;

namespace BlackHole.Core
{
    public class BHParameters
    {
        internal List<BlackHoleParameter> Parameters = new List<BlackHoleParameter>();

        public void Add(string? Name , object? Value)
        {
            Parameters.Add(new BlackHoleParameter { Name = @Name , Value = Value });
        }
    }
}
