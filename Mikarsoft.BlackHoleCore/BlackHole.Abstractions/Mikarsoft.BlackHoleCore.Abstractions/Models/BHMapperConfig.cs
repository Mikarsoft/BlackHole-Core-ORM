

using Mikarsoft.BlackHoleCore.Abstractions.Tools;
using Mikarsoft.BlackHoleCore.Entities;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore.Abstractions.Models
{
    public class BHMapperConfig<T, Dto> where T : BHEntity<T>, new() where Dto : class
    {
        internal List<BHMappingModel> Mappings { get; set; } = new();

        public BHMapperConfig<T, Dto> MapProperty<TKey>(Expression<Func<T, TKey?>> key, Expression<Func<Dto, TKey?>> otherKey) where TKey : IComparable
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
