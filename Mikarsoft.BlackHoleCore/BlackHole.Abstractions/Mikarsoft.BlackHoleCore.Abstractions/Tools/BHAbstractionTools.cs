using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore.Abstractions.Tools
{
    public static class BHAbstractionTools
    {
        public static string MemberParse<T, TKey>(this Expression<Func<T, TKey?>> key)
        {
            if (key.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }

            if (key.Body is UnaryExpression unaryExpression &&
                unaryExpression.Operand is MemberExpression operandMemberExpression)
            {
                return operandMemberExpression.Member.Name;
            }

            return string.Empty;
        }
    }
}
