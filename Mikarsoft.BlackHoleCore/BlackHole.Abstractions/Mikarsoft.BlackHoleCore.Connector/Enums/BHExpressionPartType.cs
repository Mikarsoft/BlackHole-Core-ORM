namespace Mikarsoft.BlackHoleCore.Connector.Enums
{
    public enum BHExpressionPartType : byte
    {
        Select    =    0xC4,
        Insert    =    0xC5,
        Delete    =    0xC6,
        Update    =    0xC8,
        Table     =    0xC9,
        Column,
        Parameter,

    }
}
