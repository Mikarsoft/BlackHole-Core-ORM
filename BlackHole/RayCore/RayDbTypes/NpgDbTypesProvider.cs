
namespace BlackHole.RayCore.RayDbTypes
{
    internal class NpgDbTypesProvider : IRayDbTypesProvider
    {
        short IRayDbTypesProvider.GetBoolean() => (short)NpgDbTypes.Boolean;

        short IRayDbTypesProvider.GetByte() => (short)NpgDbTypes.Bit;

        short IRayDbTypesProvider.GetByteArray() => (short)NpgDbTypes.Bytea;

        short IRayDbTypesProvider.GetChar() => (short)NpgDbTypes.Char;

        short IRayDbTypesProvider.GetDateTime() => (short)NpgDbTypes.Timestamp;

        short IRayDbTypesProvider.GetDecimal() => (short)NpgDbTypes.Numeric;

        short IRayDbTypesProvider.GetDouble() => (short)NpgDbTypes.Double;

        short IRayDbTypesProvider.GetGuid() => (short)NpgDbTypes.Uuid;

        short IRayDbTypesProvider.GetInt() => (short)NpgDbTypes.Integer;

        short IRayDbTypesProvider.GetJson() => (short)NpgDbTypes.Json;

        short IRayDbTypesProvider.GetLong() => (short)NpgDbTypes.BigInt;

        short IRayDbTypesProvider.GetShort() => (short)NpgDbTypes.Smallint;

        short IRayDbTypesProvider.GetSingle() => (short)NpgDbTypes.Real;

        short IRayDbTypesProvider.GetString() => (short)NpgDbTypes.Varchar;

        short IRayDbTypesProvider.GetTime() => (short)NpgDbTypes.Time;

        short IRayDbTypesProvider.GetUnknown() => (short)NpgDbTypes.Unknown;
    }

    internal enum NpgDbTypes : short
    {
        BigInt = 1,
        Boolean = 2,
        Double = 8,
        Integer = 9,
        Numeric = 13,
        Real = 17,
        Smallint = 18,
        Uuid = 27,
        Bytea = 4,
        Char = 6,
        Varchar = 22,
        Timestamp = 21,
        Bit = 25,
        Json = 35,
        Unknown = 40,
        Time = 20,
    }
}
