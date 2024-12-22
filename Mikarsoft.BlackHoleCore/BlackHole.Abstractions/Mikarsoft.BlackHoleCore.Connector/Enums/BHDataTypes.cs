namespace Mikarsoft.BlackHoleCore.Connector.Enums
{
    public enum BHDataTypes : byte
    {
        Byte            =         0x00,
        Boolean         =         0x01,
        Short           =         0x02,
        Integer         =         0x03,
        Long            =         0x04,
        Float           =         0x05,
        Decimal         =         0x06,
        Double          =         0x07,
        DateTime        =         0x08,
        Time            =         0x09,
        DateTimeOffset  =         0x10,
        Uid             =         0x16,
        Character       =         0x20,
        Text            =         0x40,
        ByteArray       =         0x60,
        Json            =         0x80,
        Collection      =         0x82
    }
}
