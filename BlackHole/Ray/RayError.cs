namespace BlackHole.Ray
{
    [Serializable]
    public sealed class RayError
    {
        //Data
        internal string _message;
        internal string _state;
        internal int _nativeerror;
        internal string _source;

        internal RayError(string source, string message, string state, int nativeerror)
        {
            _source = source;
            _message = message;
            _state = state;
            _nativeerror = nativeerror;
        }

        public string Message
        {
            get
            {
                return ((null != _message) ? _message : String.Empty);
            }
        }

        public string SQLState
        {
            get
            {
                return _state;
            }
        }

        public int NativeError
        {
            get
            {
                return _nativeerror;
            }
        }

        public string Source
        {
            get
            {
                return ((null != _source) ? _source : String.Empty);
            }
        }

        internal void SetSource(string Source)
        {
            _source = Source;
        }

        override public string ToString()
        {
            return Message;
        }
    }
}
