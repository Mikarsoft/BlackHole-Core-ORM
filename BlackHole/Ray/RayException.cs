using System.Runtime.Serialization;
using System.Text;
using System.Data.Common;

namespace BlackHole.Ray
{
    [Serializable]
    public sealed class RayException : DbException
    {
        RayErrorCollection odbcErrors = new RayErrorCollection();

        Ray32.RETCODE _retcode;    // DO NOT REMOVE! only needed for serialization purposes, because Everett had it.

        static internal RayException CreateException(RayErrorCollection errors, Ray32.RetCode retcode)
        {
            StringBuilder builder = new StringBuilder();
            foreach (RayError error in errors)
            {
                if (builder.Length > 0)
                {
                    builder.Append(Environment.NewLine);
                }

                builder.Append($"RayException: {Ray32.RetcodeToString(retcode)}, {error.SQLState}, {error.Message}"); // MDAC 68337
            }
            RayException exception = new RayException(builder.ToString(), errors);
            return exception;
        }

        internal RayException(string message, RayErrorCollection errors) : base(message)
        {
            odbcErrors = errors;
            HResult = HResults.RayException;
        }

        // runtime will call even if private...
        private RayException(SerializationInfo si, StreamingContext sc) : base(si, sc)
        {
            _retcode = (Ray32.RETCODE)si.GetValue("odbcRetcode", typeof(Ray32.RETCODE));
            odbcErrors = (RayErrorCollection)si.GetValue("odbcErrors", typeof(RayErrorCollection));
            HResult = HResults.RayException;
        }

        public RayErrorCollection Errors
        {
            get
            {
                return odbcErrors;
            }
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
        override public void GetObjectData(SerializationInfo si, StreamingContext context)
        {
            // MDAC 72003
            if (null == si)
            {
                throw new ArgumentNullException("si");
            }
            si.AddValue("odbcRetcode", _retcode, typeof(Ray32.RETCODE));
            si.AddValue("odbcErrors", odbcErrors, typeof(RayErrorCollection));
            base.GetObjectData(si, context);
        }

        // mdac bug 62559 - if we don't have it return nothing (empty string)
        override public string Source
        {
            get
            {
                if (0 < Errors.Count)
                {
                    string source = Errors[0].Source;
                    return string.IsNullOrEmpty(source) ? "" : source; // base.Source;
                }
                return ""; // base.Source;
            }
        }
    }
}
