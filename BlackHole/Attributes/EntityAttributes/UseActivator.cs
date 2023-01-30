
namespace BlackHole.Attributes.EntityAttributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]

    public class UseActivator : Attribute
    {
        public bool useActivator = true;
    }
}
