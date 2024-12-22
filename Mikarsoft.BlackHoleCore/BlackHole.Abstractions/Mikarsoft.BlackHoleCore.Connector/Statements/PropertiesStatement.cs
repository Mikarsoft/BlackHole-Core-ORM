namespace Mikarsoft.BlackHoleCore.Connector.Statements
{
    public class PropertiesStatement
    {
        public string Name { get; private set; }

        public byte TableCode { get; private set; }

        public Type PropertyType { get; private set; }

        public PropertiesStatement(string name, Type propType,  byte tableCode = 0x00)
        {
            Name = name;
            TableCode = tableCode;
            PropertyType = propType;
        }
    }
}
