using Mikarsoft.BlackHoleCore.Entities;

namespace Mikarsoft.BlackHoleCore
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="G"></typeparam>
    public interface IBHTable<T, G> : IBHTable<T> where G : struct, IBHStruct where T : BHEntityAI<T, G> , new()
    {
        IMQueryAI<T, G> GetById(G id);

        IMIncludeAI<T, G, Dto> GetById<Dto>(G id) where Dto : BHDto<G>;

        IMUpdateMapper<T> UpdateById(T item);

        IMUpdateMapper<T> UpdateById(List<T> items);

        IMUpdateMapper<T, Dto> UpdateById<Dto>(Dto item) where Dto : BHDto<G>;

        IMUpdateMapper<T, Dto> UpdateById<Dto>(List<Dto> item) where Dto : BHDto<G>;

        IMExecute<T> DeleteById(G id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBHTable<T> : IBHContextBase where T : BHEntity<T> , new()
    {
        IMQuery<T> Select();

        IMQuery<T, Dto> Select<Dto>() where Dto : class;

        IMUpdateQuery<T> Update(T item);

        IMUpdateQuery<T, Dto> Update<Dto>(Dto item) where Dto : class;

        IMExecute<T> Insert(T item);

        IMExecute<T> Insert(List<T> items);

        IMDeleteQuery<T> Delete();
    }
}
