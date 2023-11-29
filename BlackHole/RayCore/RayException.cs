using System.Data.Common;
using System.Runtime.Serialization;
using System.Text;

namespace BlackHole.RayCore
{
    [Serializable]
    internal sealed class RayException : DbException
    {
        RayErrorCollection rayErrors = new RayErrorCollection();

        Ray64.RETCODE _retcode;    // DO NOT REMOVE! only needed for serialization purposes, because Everett had it.

        static internal RayException CreateException(RayErrorCollection errors, Ray64.RetCode retcode)
        {
            StringBuilder builder = new StringBuilder();
            foreach (RayError error in errors)
            {
                if (builder.Length > 0)
                {
                    builder.Append(Environment.NewLine);
                }

                builder.Append($"RayException: {Ray64.RetcodeToString(retcode)}, {error.SQLState}, {error.Message}"); // MDAC 68337
            }
            RayException exception = new RayException(builder.ToString(), errors);
            return exception;
        }

        internal RayException(string message, RayErrorCollection errors) : base(message)
        {
            rayErrors = errors;
            HResult = HResults.RayException;
        }

        // runtime will call even if private...
        private RayException(SerializationInfo si, StreamingContext sc) : base(si, sc)
        {
            if(si.GetValue("odbcRetcode", typeof(Ray64.RETCODE)) is Ray64.RETCODE retcode)
            {
                _retcode = retcode;
            }

            if(si.GetValue("odbcErrors", typeof(RayErrorCollection)) is RayErrorCollection collection)
            {
                rayErrors = collection;
            }

            HResult = HResults.RayException;
        }

        internal RayErrorCollection Errors
        {
            get
            {
                return rayErrors;
            }
        }

        override public void GetObjectData(SerializationInfo si, StreamingContext context)
        {
            if (null == si)
            {
                throw new ArgumentNullException("si");
            }
            si.AddValue("odbcRetcode", _retcode, typeof(Ray64.RETCODE));
            si.AddValue("odbcErrors", rayErrors, typeof(RayErrorCollection));
            base.GetObjectData(si, context);
        }

        override public string Source
        {
            get
            {
                if (Errors.Count > 0)
                {
                    string source = Errors.GetError(0)?.Source ?? string.Empty;
                    return source; // base.Source;
                }
                return string.Empty;
            }
        }
    }
}
