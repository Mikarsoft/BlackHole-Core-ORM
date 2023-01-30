

namespace BlackHole.Attributes.EcryptionAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EncryptProperty : Attribute
    {
        private bool decryptOnSelect { get; set; }

        public EncryptProperty()
        {
            decryptOnSelect = false;
        }

        public EncryptProperty(bool decryptOnSelect)
        {
            this.decryptOnSelect = decryptOnSelect;
        }
    }
}
