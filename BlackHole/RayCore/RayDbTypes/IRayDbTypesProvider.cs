namespace BlackHole.RayCore.RayDbTypes
{
    internal interface IRayDbTypesProvider
    {
        internal short GetString();
        internal short GetChar();
        internal short GetGuid();
        internal short GetDateTime();
        internal short GetShort();
        internal short GetSingle();
        internal short GetInt();
        internal short GetLong();
        internal short GetDecimal();
        internal short GetDouble();
        internal short GetBoolean();
        internal short GetByteArray();
        internal short GetByte();
        internal short GetJson();
        internal short GetUnknown();
        internal short GetTime();
    }
}
