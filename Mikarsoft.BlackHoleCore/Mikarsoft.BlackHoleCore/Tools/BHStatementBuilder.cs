using Mikarsoft.BlackHoleCore.Abstractions.Tools;
using Mikarsoft.BlackHoleCore.Connector;
using Mikarsoft.BlackHoleCore.Connector.Enums;
using Mikarsoft.BlackHoleCore.Entities;
using System.Linq.Expressions;
using System.Reflection;

namespace Mikarsoft.BlackHoleCore.Tools
{
    internal class BHStatementBuilder
    {
        private readonly Dictionary<Type, byte> TableKeys;
        private readonly List<JoinPair> JoinPairs;
        private readonly List<WhereCase> WhereCases;
        private readonly List<OrderByCase> OrderByCases;
        private readonly List<MappingCase> MappingCases;
        private readonly List<GroupByCase> GroupByCases;
        private readonly List<OccupiedProperty> OccupiedProperties;
        private readonly BHCommandType CommandType;
        private byte TableIndex = 0;

        internal BHStatementBuilder(BHCommandType commandType, Type modelType)
        {
            TableKeys = new();
            JoinPairs = new();
            WhereCases = new();
            OrderByCases = new();
            MappingCases = new();
            GroupByCases = new();
            OccupiedProperties = GenerateProperties(modelType);
            CommandType = commandType;
        }

        internal byte[] AddJoin<T,TOther>(JoinType joinType)
        {
            Type tableTypeA = typeof(T);
            Type tableTypeB = typeof(TOther);

            if (tableTypeA == tableTypeB) throw new ArgumentException("Can't Join the same table with itself.");

            byte[] tableLetters = new byte[2];

            if (TableKeys.Count == 0)
            {
                tableLetters[0] = 0;
                TableKeys.Add(tableTypeA, tableLetters[0]);
                BindPropertiesToDto(tableTypeA, tableLetters[0]);

                tableLetters[1] = AddIndex();
                TableKeys.Add(tableTypeB, tableLetters[1]);
                BindPropertiesToDto(tableTypeB, tableLetters[1]);
            }
            else
            {
                if (TableKeys.ContainsKey(tableTypeA))
                {
                    tableLetters[0] = TableKeys[tableTypeA];

                    tableLetters[1] = AddIndex();
                    TableKeys.Add(tableTypeB, tableLetters[1]);
                    BindPropertiesToDto(tableTypeB, tableLetters[1]);
                }
                else
                {
                    throw new ArgumentException($"Table {tableTypeA.Name} does Not Exist in the previous sequence." +
                        $"the first table of each join must have been used in some previous join.");
                }
            }

            JoinPairs.Add(new JoinPair(joinType, tableLetters));

            return tableLetters;
        }

        internal void AddJoinPoint<T, TOther, TKey, TOtherKey>(Expression<Func<T, TKey?>> key,
            Expression<Func<TOther, TOtherKey?>> otherKey, OuterPairType pairType = OuterPairType.On)
        {
            string columnA = key.MemberParse();
            string columnB = otherKey.MemberParse();
            JoinPairs[JoinPairs.Count - 1].AddJoinPoint(columnA, columnB, pairType);
        }

        internal void AddWhereCase<T>(Expression<Func<T, bool>> predicate, byte tableCode = 0x00)
        {
            BHExpressionPart[] parts = predicate.ParseExpression(tableCode);
            WhereCases.Add(new WhereCase(parts, tableCode));
        }

        internal void AddCastCase<T, Dto, TKey, TOtherKey>(Expression<Func<T, TKey?>> key,
            Expression<Func<Dto, TOtherKey?>> otherKey, byte tableCode)
        {
            string tablePropertyName = key.MemberParse();
            string dtoPropertyName = otherKey.MemberParse();

            PropertyInfo? tableProperty = typeof(T).GetProperty(tablePropertyName);
            PropertyInfo? dtoProperty = typeof(Dto).GetProperty(dtoPropertyName);

            //BHDataTypes tablePropertyType = GetPropertyType(tableProperty);
            //BHDataTypes dtoPropertyType = GetPropertyType(dtoProperty);

            if(tableProperty != null && dtoProperty != null)
            {
                MappingCases.Add(new MappingCase(tablePropertyName, dtoPropertyName, tableProperty.PropertyType, dtoProperty.PropertyType, tableCode));
            }
        }

        private byte AddIndex()
        {
            TableIndex++;
            return TableIndex;
        }

        private List<OccupiedProperty> GenerateProperties(Type modelType)
        {
            List<OccupiedProperty> occupiedProps = new();

            foreach(PropertyInfo pinfo in modelType.GetProperties())
            {
                occupiedProps.Add(new OccupiedProperty(pinfo.Name, pinfo.PropertyType));
            }

            return occupiedProps;
        }

        private void BindPropertiesToDto(Type tableType, byte tableCode)
        {
            List<string> OtherPropNames = tableType.GetProperties().Select(x => x.Name).ToList();

            for (int i = 0; i < OccupiedProperties.Count; i++)
            {
                OccupiedProperty property = OccupiedProperties[i];

                if (OtherPropNames.Contains(property.PropertyName) && !property.IsOccupied)
                {
                    Type? TOtherPropType = tableType.GetProperty(property.PropertyName)?.PropertyType;

                    if (TOtherPropType == property.PropertyType)
                    {
                        OccupiedProperties[i].IsOccupied = true;
                        OccupiedProperties[i].TableColumn = TOtherPropType.Name;
                        OccupiedProperties[i].TableColumnType = TOtherPropType;
                        OccupiedProperties[i].TableCode = tableCode;
                    }
                }
            }
        }

        private BHDataTypes GetPropertyType(PropertyInfo? pinfo)
        {
            if(pinfo == null)
            {
                throw new ArgumentNullException("Property was not found on the entity or dto");
            }

            Type propertyType = pinfo.PropertyType;
            Type actualType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            return actualType switch
            {
                Type t when t == typeof(Int) => BHDataTypes.Integer,
                Type t when t == typeof(Str) => BHDataTypes.Text,
                Type t when t == typeof(Uid) => BHDataTypes.Uid,
                Type t when t == typeof(int) => BHDataTypes.Integer,
                Type t when t == typeof(string) => BHDataTypes.Text,
                Type t when t == typeof(Guid) => BHDataTypes.Uid,
                Type t when t == typeof(bool) => BHDataTypes.Boolean,
                Type t when t == typeof(decimal) => BHDataTypes.Decimal,
                Type t when t == typeof(short) => BHDataTypes.Short,
                Type t when t == typeof(double) => BHDataTypes.Double,
                Type t when t == typeof(byte[]) => BHDataTypes.ByteArray,
                Type t when t == typeof(DateTime) => BHDataTypes.DateTime,
                Type t when t == typeof(DateTimeOffset) => BHDataTypes.DateTimeOffset,
                Type t when t.IsGenericType && t.GetGenericTypeDefinition() == typeof(BHJson<>) => BHDataTypes.Json,
                Type t when t == typeof(long) => BHDataTypes.Long,
                Type t when t == typeof(float) => BHDataTypes.Float,
                Type t when t == typeof(char) => BHDataTypes.Character,
                Type t when t == typeof(byte) => BHDataTypes.Byte,
                Type t when t == typeof(TimeOnly) => BHDataTypes.Time,
                _ => throw new ArgumentException("Property type is Not Supported by BlackHole ORM"),
            };
        }
    }

    internal class OccupiedProperty
    {
        internal OccupiedProperty(string propname, Type propertyType)
        {
            PropertyName = propname;
            TableColumn = propname;
            PropertyType = propertyType;
            TableColumnType = propertyType;
        }

        internal string PropertyName { get; set; }
        internal Type PropertyType { get; set; }

        internal Type TableColumnType { get; set; }
        internal byte TableCode { get; set; }
        internal string TableColumn { get; set; }
        internal bool IsOccupied { get; set; }
    }

    internal class GroupByCase
    {
        internal GroupByCase(string propName)
        {
            PropertyName = propName;
        }

        internal string PropertyName { get; set; }
    }

    internal class WhereCase
    {
        internal WhereCase(BHExpressionPart[] parts, byte tableCode)
        {
            ExpressionParts = parts;
            TableCode = tableCode;
        }

        internal BHExpressionPart[] ExpressionParts { get; set; }
        internal byte TableCode { get; set; }
    }

    internal class MappingCase
    {
        internal MappingCase(string fromColumn, string toColumn, Type fromType, Type toType , byte tableCode)
        {
            ColumnFrom = fromColumn;
            ColumnTo = toColumn;
            FromType = fromType;
            ToType = toType;
            TableCode = tableCode;
        }

        internal byte TableCode { get; set; }

        internal string ColumnFrom { get; set; }
        internal Type FromType { get; set; }

        internal string ColumnTo { get; set; }
        internal Type ToType { get; set; }

        internal bool UseCast
        {
            get
            {
                return FromType != ToType;
            }
        }

        //internal bool InvalidCast
        //{
        //    get
        //    {
        //        return FromType > ToType;
        //    }
        //}
    }

    internal class OrderByCase
    {
        internal OrderByCase(string column, OrderByDirection direcation = OrderByDirection.Ascending, byte tableCode = 0x00)
        {
            ColumnName = column;
            Direcation = direcation;
            TableCode = tableCode;
        }

        internal string ColumnName { get; set; }
        internal byte TableCode { get; set; }
        internal OrderByDirection Direcation { get; set; }
    }

    internal class JoinPair
    {
        internal JoinPair(JoinType joinType, byte[] tableLetters)
        {
            JType = joinType;
            TableACode = tableLetters[0];
            TableDCode = tableLetters[1];
            JoinPoints = new();
        }

        internal JoinType JType { get; set; }

        internal byte TableACode { get; set; }
        internal byte TableDCode { get; set; }

        internal List<JoinPoint> JoinPoints { get; set; }

        internal void AddJoinPoint(string columnA, string columnB, OuterPairType pairType)
        {
            JoinPoints.Add(new JoinPoint(columnA, columnB, pairType));
        }
    }

    internal class JoinPoint
    {
        internal JoinPoint(string columnA, string columnB, OuterPairType pairType)
        {
            PairType = pairType;
            ColumnA = columnA;
            ColumnB = columnB;
        }

        internal OuterPairType PairType { get; set; }
        internal string ColumnA { get; set; }
        internal string ColumnB { get; set; }
    }
}
