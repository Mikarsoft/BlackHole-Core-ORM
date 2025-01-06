using Mikarsoft.BlackHoleCore.Abstractions.Models;
using Mikarsoft.BlackHoleCore.Connector;
using System.Linq.Expressions;

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

        internal static Dictionary<string, string> GetPropertyMappings<TSource, TTarget>(this Expression<Func<TSource, TTarget>> mapExpression)
        {
            // Create a dictionary to hold the mappings
            var mappings = new Dictionary<string, string>();

            // Check if the body is a MemberInitExpression (i.e., a new object initialization)
            if (mapExpression.Body is MemberInitExpression initExpression)
            {
                foreach (var binding in initExpression.Bindings)
                {
                    // Ensure it's a MemberAssignment (i.e., a direct property assignment)
                    if (binding is MemberAssignment assignment)
                    {
                        // Get the target property name
                        var targetProperty = assignment.Member.Name;

                        // Get the source property name from the expression
                        if (assignment.Expression is MemberExpression sourceExpression)
                        {
                            var sourceProperty = sourceExpression.Member.Name;
                            mappings[sourceProperty] = targetProperty;
                        }
                    }
                }
            }

            return mappings;
        }
    }
}
