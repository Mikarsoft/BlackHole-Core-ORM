using Mikarsoft.BlackHoleCore.Abstractions.Tools;
using Mikarsoft.BlackHoleCore.Entities;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore.Abstractions.Entities
{
    public class IncludeOptionsBuilder<T> where T : BHEntity<T>, new()
    {
        public IncludeSettings<T> IncludeNone()
        {
            return new();
        }

        public IncludeMap<T, G, H> HasMany<G, H>(Expression<Func<T, BHCollection<G, H>>> include) where G : BHEntityAI<G, H>, new() where H : struct, IBHStruct
        {
            string propertyName = include.MemberParse();

            IncludeSettings<T> settings = new();

            return new(settings, propertyName);
        }

        public IncludeMap<T, G, H> HasOne<G, H>(Expression<Func<T, BHItem<G, H>>> include) where G : BHEntityAI<G, H>, new() where H : struct, IBHStruct
        {
            string propertyName = include.MemberParse();

            IncludeSettings<T> settings = new();

            return new(settings, propertyName);
        }
    }

    public static class IncludeExtensions
    {
        public static IncludeMap<T, G, H> HasMany<T, G, H>(this IncludeSettings<T> settings, Expression<Func<T, BHCollection<G, H>>> include) 
            where T : BHEntity<T> , new() where G : BHEntityAI<G, H>, new() where H : struct, IBHStruct
        {
            string propertyName = include.MemberParse();

            return new(settings, propertyName);
        }

        public static IncludeMap<T, G, H> HasOne<T, G, H>(this IncludeSettings<T> settings, Expression<Func<T, BHItem<G, H>>> include)
            where T : BHEntity<T>, new() where G : BHEntityAI<G, H>, new() where H : struct, IBHStruct
        {
            string propertyName = include.MemberParse();

            return new(settings, propertyName);
        }
    }

    public class IncludeMap<T, G, H> where T : BHEntity<T>, new() where G : BHEntityAI<G, H> , new() where H : struct, IBHStruct
    {
        internal string PropertyName { get; set; }

        internal string ParentProperty {  get; set; } = string.Empty;

        internal string ChildProperty {  get; set; } = string.Empty;

        internal IncludeSettings<T> Settings { get; set; }

        internal IncludeMap(IncludeSettings<T> settings, string propertyName)
        {
            PropertyName = propertyName;
            Settings = settings;
        }

        public IncludeSettings<T> On<TKey>(Expression<Func<T, TKey?>> key, Expression<Func<G, TKey?>> otherKey) where TKey : IComparable<TKey>
        {
            string parentProp = key.MemberParse();
            string childProp = otherKey.MemberParse();
            Settings.AddPair(ParentProperty, parentProp, childProp);

            return Settings;
        }
    }

    public class IncludeSettings<T> where T: BHEntity<T>, new()
    {
        internal List<IncludePair> IncludePairs { get; private set; } = new();

        internal void AddPair(string parentProperty, string parentPropertyMatch, string childPropertyMatch)
        {
            IncludePair? existing = IncludePairs.FirstOrDefault(p => p.ParentProperty == parentProperty);

            if (existing != null)
            {
                IncludePairs.Remove(existing);
            }

            IncludePairs.Add(new IncludePair(parentProperty, parentPropertyMatch, childPropertyMatch));
        }
    }

    public class IncludePair
    {
        internal string ParentProperty { get; set; }

        internal string ParentPropertyMatch { get; set; }

        internal string ChildPropertyMatch { get; set; }

        internal IncludePair(string parentProperty, string parentPropertyMatch, string childPropertyMatch)
        {
            ParentProperty = parentProperty;
            ParentPropertyMatch = parentPropertyMatch;
            ChildPropertyMatch = childPropertyMatch;
        }
    }
}
