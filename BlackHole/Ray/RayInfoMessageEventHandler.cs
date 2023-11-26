
using System.Text;

namespace BlackHole.Ray
{
    public delegate void RayInfoMessageEventHandler(object sender, RayInfoMessageEventArgs e);

    public sealed class RayInfoMessageEventArgs : System.EventArgs
    {
        private RayErrorCollection _errors;

        internal RayInfoMessageEventArgs(RayErrorCollection errors)
        {
            _errors = errors;
        }

        public RayErrorCollection Errors
        {
            get { return _errors; }
        }

        public string Message
        { // MDAC 84407
            get
            {
                StringBuilder builder = new StringBuilder();
                foreach (RayError error in Errors)
                {
                    if (0 < builder.Length) { builder.Append(Environment.NewLine); }
                    builder.Append(error.Message);
                }
                return builder.ToString();
            }
        }

        public override string ToString()
        {
            // MDAC 84407
            return Message;
        }
    }
}
