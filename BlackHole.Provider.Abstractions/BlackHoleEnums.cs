namespace Mikarsoft.BlackHoleCore.Connector
{
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

    internal enum BHMode
    {
        Single,
        Multiple,
        HighAvailability
    }

    internal enum DatabaseRole
    {
        Master,
        StandBy,
        BackUp
    }
}
