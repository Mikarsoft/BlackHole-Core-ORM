

namespace BlackHole.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UnderLineToDash : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public bool replaceUnderlines = true;
    }
}
