
using Mikarsoft.BlackHoleCore.Entities;

namespace Mikarsoft.BlackHoleCore
{
    internal class BHTable<T, G> : BHTable<T>, IBHTable<T, G> where G : struct, IBHStruct where T : BHEntityAI<T, G>, new()
    {
        IMExecute<T> IBHTable<T, G>.DeleteById(G id)
        {
            throw new NotImplementedException();
        }

        IMQueryAI<T, G> IBHTable<T, G>.GetById(G id)
        {
            throw new NotImplementedException();
        }

        IMIncludeAI<T, G, Dto> IBHTable<T, G>.GetById<Dto>(G id)
        {
            throw new NotImplementedException();
        }

        IMUpdateMapper<T> IBHTable<T, G>.UpdateById(T item)
        {
            throw new NotImplementedException();
        }

        IMUpdateMapper<T> IBHTable<T, G>.UpdateById(List<T> items)
        {
            throw new NotImplementedException();
        }

        IMUpdateMapper<T, Dto> IBHTable<T, G>.UpdateById<Dto>(Dto item)
        {
            throw new NotImplementedException();
        }

        IMUpdateMapper<T, Dto> IBHTable<T, G>.UpdateById<Dto>(List<Dto> item)
        {
            throw new NotImplementedException();
        }
    }

    internal class BHTable<T> : BHContextBase, IBHTable<T> where T : BHEntity<T>, new()
    {
        IMDeleteQuery<T> IBHTable<T>.Delete()
        {
            throw new NotImplementedException();
        }

        IMExecute<T> IBHTable<T>.Insert(T item)
        {
            throw new NotImplementedException();
        }

        IMExecute<T> IBHTable<T>.Insert(List<T> items)
        {
            throw new NotImplementedException();
        }

        IMQuery<T> IBHTable<T>.Select()
        {
            throw new NotImplementedException();
        }

        IMQuery<T, Dto> IBHTable<T>.Select<Dto>()
        {
            throw new NotImplementedException();
        }

        IMUpdateQuery<T> IBHTable<T>.Update(T item)
        {
            throw new NotImplementedException();
        }

        IMUpdateQuery<T, Dto> IBHTable<T>.Update<Dto>(Dto item)
        {
            throw new NotImplementedException();
        }
    }
}
