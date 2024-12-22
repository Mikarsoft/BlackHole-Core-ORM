using Mikarsoft.BlackHoleCore.Abstractions.Models;
using Mikarsoft.BlackHoleCore.Connector;

namespace Mikarsoft.BlackHoleCore.Tools
{
    internal static class BHParametersMapper
    {
        internal static List<BlackHoleInnerParameter> MapParameters(this List<BlackHoleParameter> parameters)
        {
            List<BlackHoleInnerParameter> innerParameters = [];

            foreach (var parameter in parameters)
            {
                innerParameters.Add(new BlackHoleInnerParameter(parameter.Name, parameter.Value));
            }

            return innerParameters;
        }
    }
}
