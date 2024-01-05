﻿using BlackHole.Enums;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace BlackHole.Engine
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectionReference : IDisposable
    {
        internal ConnectionReference(int connectionIndex)
        {
            Connection = connectionIndex.GetConnection();
            Connection.Open();
            IsOpen = true;
            Transaction = Connection.BeginTransaction();
        }

        internal bool IsOpen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IDbConnection Connection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IDbTransaction Transaction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (IsOpen)
            {
                Transaction.Dispose();
                Connection.Close();
                Connection.Dispose();
                IsOpen= false;
            }
        }
    }

    internal class EntityContext
    {
        internal int ConnectionIndex { get; set; }
        internal BlackHoleSqlTypes DatabaseType { get; set; }
        internal bool IsQuotedDb { get; set; }
        internal List<string> Columns { get; set; } = new();
        internal bool UseIdGenerator { get; set; }
        internal bool WithActivator { get; set; }
        internal string ThisTable { get; set; } = string.Empty;
        internal string PropertyNames { get; set; } = string.Empty;
        internal string PropertyParams { get; set; } = string.Empty;
        internal string UpdateParams { get; set; } = string.Empty;
        internal string ThisId { get; set; } = string.Empty;
        internal string ThisInactive { get; set; } = string.Empty;
        internal string ThisSchema { get; set; } = string.Empty;
        internal string ReturningId {  get; set; } = string.Empty;
        internal PropertyInfo[] Tprops { get; set; } = new PropertyInfo[0];
    }

    internal class SqlFunctionResult
    {
        internal SqlFunctionResult()
        {
            WasTranslated = false;
        }

        public string SqlCommand { get; set; } = " 1 != 1 ";
        public string SqlCommandReverseQuotes { get; set; } = "1 != 1";
        internal object? Value { get; set; }
        internal string ParamName { get; set; } = string.Empty;
        internal string Letter { get; set; } = string.Empty;
        internal int Index { get; set; }
        internal bool IsQuotedDb { get; set; }
        internal bool WasTranslated { get; set; }
        internal string SchemaName { get; set; } = string.Empty;
    }

    internal class ExpressionsData
    {
        internal BinaryExpression? Operation { get; set; }
        internal MethodCallExpression? LeftMethodMember { get; set; }
        internal MethodCallExpression? RightMethodMember { get; set; }
        public MemberExpression? LeftMember { get; set; }
        internal MemberExpression? RightMember { get; set; }
        internal List<MethodExpressionData> MethodData { get; set; } = new List<MethodExpressionData>();
        internal ExpressionType OperationType { get; set; }
        internal object? MemberValue { get; set; }
        internal bool MethodChecked { get; set; }
        internal bool RightChecked { get; set; }
        internal bool LeftChecked { get; set; }
        internal int ParentIndex { get; set; }
        internal string SqlCommand { get; set; } = string.Empty;
        internal string SqlCommandReverseQuotes { get; set; } = string.Empty;
    }

    internal class MethodExpressionData
    {
        internal List<object?> MethodArguments { get; set; } = new List<object?>();
        internal Expression? CastedOn { get; set; }
        internal string MethodName { get; set; } = string.Empty;
        internal ExpressionType OperatorType { get; set; }
        internal object? ComparedValue { get; set; }
        internal MemberExpression? CompareProperty { get; set; }
        internal bool ReverseOperator { get; set; }
        internal string? TableName { get; set; } = string.Empty;
    }

    internal class BlackHoleParameter
    {
        internal string? Name { get; set; }
        internal object? Value { get; set; }
    }

    internal class ColumnsAndParameters
    {
        internal string Columns { get; set; } = string.Empty;
        internal string ColumnsReverseQuotes { get; set; } = string.Empty;
        internal List<BlackHoleParameter> Parameters { get; set; } = new List<BlackHoleParameter>();
        internal int Count { get; set; }
    }

    internal class ColumnAndParameter
    {
        internal string? Column { get; set; }
        internal string? ColumnReverseQuotes { get; set; }
        internal string? ParamName { get; set; }
        internal object? Value { get; set; }
    }

    internal class TripleStringBuilder : IDisposable
    {
        internal StringBuilder PNSb { get; }
        internal StringBuilder PPSb { get; }
        internal StringBuilder UPSb { get; }

        internal TripleStringBuilder()
        {
            PNSb = new StringBuilder();
            PPSb = new StringBuilder();
            UPSb = new StringBuilder();
        }

        public void Dispose()
        {
            PNSb.Clear();
            PPSb.Clear();
            UPSb.Clear();
        }
    }

    internal class SPData
    {
        internal SPData(Type dtoType, string procName)
        {
            DtoType = dtoType;
            ProcedureName = procName;
        }

        internal Type DtoType { get; set; }
        internal string ProcedureName { get; set; } = string.Empty;
        internal int ConnectionIndex { get; set; }
        internal bool IsQuotedDb { get; set; }
    }

    internal class JoinsData
    {
        internal JoinsData(Type dtoType)
        {
            DtoType = dtoType;
        }
        internal int ConnectionIndex { get; set; }
        internal Type DtoType { get; set; }
        internal Type? BaseTable { get; set; }
        internal List<PropertyOccupation> OccupiedDtoProps { get; set; } = new List<PropertyOccupation>();
        internal List<TableLetters> TablesToLetters { get; set; } = new List<TableLetters>();
        internal List<string?> Letters { get; set; } = new List<string?>();
        internal string Joins { get; set; } = string.Empty;
        internal string JoinsReverseQuotes { get; set; } = string.Empty;
        internal string WherePredicates { get; set; } = string.Empty;
        internal string WhereReverseQuotes { get; set; } = string.Empty;
        internal List<BlackHoleParameter> DynamicParams { get; set; } = new List<BlackHoleParameter>();
        internal int HelperIndex { get; set; }
        internal bool IsQuotedDb { get; set; }
        internal bool Ignore { get; set; }
        internal int ParamsCount { get; set; }
        internal string OrderByOptions { get; set; } = string.Empty;
        internal string OrderByReverseQuotes { get; set; } = string.Empty;
        internal bool HAMode { get; set; }
    }

    internal class TableLetters
    {
        internal Type? Table { get; set; }
        internal string? Letter { get; set; }
        internal string Schema { get; set; } = string.Empty;
        internal bool IsOpenEntity { get; set; }
    }

    internal class PropertyOccupation
    {
        internal string PropName { get; set; } = string.Empty;
        internal Type? PropType { get; set; }
        internal bool Occupied { get; set; }
        internal string? TableLetter { get; set; }
        internal string? TableProperty { get; set; } = string.Empty;
        internal Type? TablePropertyType { get; set; }
        internal int WithCast { get; set; }
    }
}
