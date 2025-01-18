using Mikarsoft.BlackHoleCore.Abstractions.Tools;
using Mikarsoft.BlackHoleCore.Entities;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore.Abstractions.Models
{
    public class BHUpdateMappingConfig<T> where T : BHEntity<T>, new()
    {
        List<string> PropertiesToMap = new List<string>();

        public BHUpdateMappingConfig<T> Use<TKey>(Expression<Func<T, TKey?>> key)
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

    public class BHUpdateMappingConfig<T, Dto> where T : BHEntity<T>, new() where Dto : class
    {
        internal List<BHMappingModel> Mappings { get; set; } = new();

        public BHUpdateMappingConfig<T, Dto> UseAs<TKey>(Expression<Func<T, TKey?>> key, Expression<Func<T, TKey?>> otherKey)
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
