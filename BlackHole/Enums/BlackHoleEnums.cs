namespace BlackHole.Enums
{
    internal enum BlackHoleIdTypes
    {
        IntId,
        GuidId,
        StringId
    }

    internal enum BlackHoleSqlTypes
    {
        SqlServer,
        MySql,
        Postgres,
        SqlLite,
        Oracle
    }

    internal enum CliCommandTypes
    {
        Update,
        Drop,
        Parse,
        Default
    }

    internal enum DbParsingStates
    {
        Proceed,
        ChangesRequired,
        Incompatible
    }
}
