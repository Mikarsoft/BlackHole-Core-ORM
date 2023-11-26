
using System.Collections;

namespace BlackHole.Ray
{
    [Serializable]
    public sealed class RayErrorCollection : ICollection
    {
        private ArrayList _items = new ArrayList();

        internal RayErrorCollection()
        {
        }

        Object System.Collections.ICollection.SyncRoot
        {
            get { return this; }
        }

        bool System.Collections.ICollection.IsSynchronized
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

        public RayError this[int i]
        {
            get
            {
                return (RayError)_items[i];
            }
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
                ((RayError)error).SetSource(Source);
            }
        }
    }
}
