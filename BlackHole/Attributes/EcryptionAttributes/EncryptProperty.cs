

namespace BlackHole.Attributes.EcryptionAttributes
{
    /// <summary>
    /// A Feature that will be added in the upcoming versions
    /// to automatically Encrypt String columns that contain sensitive data.
    /// It doesn't do anything if you use it now.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EncryptProperty : Attribute
    {
        private bool decryptOnSelect { get; set; }
        /// <summary>
        /// A Feature that will be added in the upcoming versions
        /// to automatically Encrypt String columns that contain sensitive data.
        /// It doesn't do anything if you use it now.
        /// </summary>
        public EncryptProperty()
        {
            decryptOnSelect = false;
        }

        /// <summary>
        /// A Feature that will be added in the upcoming versions
        /// to automatically Encrypt String columns that contain sensitive data.
        /// It doesn't do anything if you use it now.
        /// </summary>
        /// <param name="decryptOnSelect">Defines if the field will be decrypted on read.</param>
        public EncryptProperty(bool decryptOnSelect)
        {
            this.decryptOnSelect = decryptOnSelect;
        }
    }
}
