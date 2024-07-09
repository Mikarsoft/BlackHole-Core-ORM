
namespace BlackHole.Engine
{
    internal class BlackHoleSqlBuilder
    {
        internal EntityContext _context;

        internal BlackHoleSqlBuilder(EntityContext context)
        {
            _context = context;
        }

        internal string SelectCommand()
        {
            return string.Empty;
        }

        internal string DeleteCommand(string TableName)
        {
            return string.Empty;
        }

        internal string UpdateCommand(string TableName)
        {
            return string.Empty;
        }

        internal string InsertCommand(string TableName)
        {
            return string.Empty;
        }

        internal string WhereCase(string TableName)
        {
            return string.Empty;
        }

        internal string OrderByCase()
        {
            return string.Empty;
        }
    }
}
