using Mikarsoft.BlackHoleCore.Abstractions.Tools;
using Mikarsoft.BlackHoleCore.Entities;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore.Abstractions.Models
{
    public class BHColumnUsageConfig<T> where T : BHEntity<T>, new()
    {
        List<string> PropertiesToMap = new List<string>();

        public BHColumnUsageConfig<T> Use<TKey>(Expression<Func<T, TKey?>> key) where TKey : IComparable
        {
            string sourceProperty = key.MemberParse();

            if (PropertiesToMap.Contains(sourceProperty))
            {
                return this;
            }

            PropertiesToMap.Add(sourceProperty);

            return this;
        }
    }

    public class BHMappingConfig<T, Dto> where T : BHEntity<T>, new() where Dto : class
    {
        internal List<BHMappingModel> Mappings { get; set; } = new();

        public BHMappingConfig<T, Dto> UseAs<TKey>(Expression<Func<Dto, TKey?>> key, Expression<Func<T, TKey?>> otherKey) where TKey : IComparable
        {
            string sourceProperty = key.MemberParse();
            string targetProperty = otherKey.MemberParse();

            BHMappingModel? existingModel = Mappings.FirstOrDefault(x => x.TargetPropertyName == targetProperty);

            if (existingModel != null)
            {
                Mappings.Remove(existingModel);
            }

            Mappings.Add(new BHMappingModel(sourceProperty, targetProperty));

            return this;
        }
    }
}
