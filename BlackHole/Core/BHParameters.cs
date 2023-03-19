using BlackHole.CoreSupport;

namespace BlackHole.Core
{
    public class BHParameters
    {
        internal BlackHoleParameter[] Parameters = new BlackHoleParameter[0];

        public void Add(string? Name , object? Value)
        {
            Parameters.Append(new BlackHoleParameter { Name = @Name , Value = Value });
        }
    }
}
