
using System.Diagnostics;

namespace BlackHole.Ray
{
    sealed internal class RayReferenceCollection : DbReferenceCollection
    {
        internal const int Closing = 0;
        internal const int Recover = 1;

        internal const int CommandTag = 1;

        override public void Add(object value, int tag)
        {
            base.AddItem(value, tag);
        }

        override protected void NotifyItem(int message, int tag, object value)
        {
            switch (message)
            {
                case Recover:
                    if (CommandTag == tag)
                    {
                        ((RayCommand)value).RecoverFromConnection();
                    }
                    else
                    {
                        Debug.Assert(false, "shouldn't be here");
                    }
                    break;
                case Closing:
                    if (CommandTag == tag)
                    {
                        ((RayCommand)value).CloseFromConnection();
                    }
                    else
                    {
                        Debug.Assert(false, "shouldn't be here");
                    }
                    break;
                default:
                    Debug.Assert(false, "shouldn't be here");
                    break;
            }
        }

        override public void Remove(object value)
        {
            base.RemoveItem(value);
        }

    }
}
