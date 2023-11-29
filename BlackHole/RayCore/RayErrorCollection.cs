using System.Collections;

namespace BlackHole.RayCore
{
    [Serializable]
    internal sealed class RayErrorCollection : ICollection
    {
        private ArrayList _items = new ArrayList();

        internal RayErrorCollection()
        {
        }

        object ICollection.SyncRoot
        {
            get { return this; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public RayError? GetError(int i)
        {
            return _items[i] as RayError;
        }

        internal void Add(RayError error)
        {
            _items.Add(error);
        }

        public void CopyTo(Array array, int i)
        {
            _items.CopyTo(array, i);
        }

        public void CopyTo(RayError[] array, int i)
        {
            _items.CopyTo(array, i);
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        internal void SetSource(string Source)
        {
            foreach (object error in _items)
            {
                if(error is RayError rayErr)
                {
                    rayErr.SetSource(Source);
                }
            }
        }
    }

     [Serializable]
    internal sealed class RayError
    {
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
                return _message ?? string.Empty;
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
                return _source ?? string.Empty;
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
