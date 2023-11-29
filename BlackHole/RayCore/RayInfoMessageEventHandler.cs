using System.Text;

namespace BlackHole.RayCore
{
    internal delegate void RayInfoMessageEventHandler(object sender, RayInfoMessageEventArgs e);

    internal sealed class RayInfoMessageEventArgs : EventArgs
    {
        private RayErrorCollection _errors;

        internal RayInfoMessageEventArgs(RayErrorCollection errors)
        {
            _errors = errors;
        }

        internal RayErrorCollection Errors
        {
            get { return _errors; }
        }

        public string Message
        {
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
            return Message;
        }
    }
}
