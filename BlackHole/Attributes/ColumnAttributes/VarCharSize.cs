
namespace BlackHole.Attributes.ColumnAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class VarCharSize : Attribute
    {
        public int Charlength { get; set; }

        public VarCharSize(int Characters)
        {
            Charlength = Characters;
        }
    }
}
