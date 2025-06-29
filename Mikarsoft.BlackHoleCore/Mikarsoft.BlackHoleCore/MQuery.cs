


using Mikarsoft.BlackHoleCore.Entities;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore
{
    internal class MQeury<T> : MIncludeBase<T>, IMQuery<T> where T : BHEntity<T>, new()
    {
        bool IMQuery<T>.Any()
        {
            throw new NotImplementedException();
        }

        bool IMQuery<T>.Any(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        Task<bool> IMQuery<T>.AnyAsync()
        {
            throw new NotImplementedException();
        }

        Task<bool> IMQuery<T>.AnyAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        int IMQuery<T>.Count()
        {
            throw new NotImplementedException();
        }

        int IMQuery<T>.Count(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        Task<int> IMQuery<T>.CountAsync()
        {
            throw new NotImplementedException();
        }

        Task<int> IMQuery<T>.CountAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }

    internal class MIncludeBase<T> : MSearchQuery<T>, IMIncludeBase<T> where T : BHEntity<T>, new()
    {
        IMThenInclude<T, G> IMIncludeBase<T>.Include<G, H>(Func<T, BHCollection<G, H>> predicate)
        {
            throw new NotImplementedException();
        }

        IMThenInclude<T, G> IMIncludeBase<T>.Include<G, H>(Func<T, BHItem<G, H>> predicate)
        {
            throw new NotImplementedException();
        }
    }

    internal class MThenInclude<T, G> : MIncludeBase<T>, IMThenInclude<T, G> where T : BHEntity<T>, new() where G : BHEntity<G>, new()
    {
        IMThenInclude<T, J> IMThenInclude<T, G>.ThenInclude<J, H>(Func<G, BHCollection<J, H>> predicate)
        {
            throw new NotImplementedException();
        }

        IMThenInclude<T, J> IMThenInclude<T, G>.ThenInclude<J, H>(Func<G, BHItem<J, H>> predicate)
        {
            throw new NotImplementedException();
        }
    }

    internal class MSearchQuery<T> : MGroupBy<T>, IMSearchQuery<T> where T : BHEntity<T> , new()
    {
        IMGroupBy<T> IMSearchQuery<T>.Where(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }

    internal class MGroupBy<T> : MOrderBy<T>, IMGroupBy<T> where T : class
    {
        IMEnumerable<IMGroupBy<G, T>, T> IMGroupBy<T>.GroupBy<G>(Expression<Func<T, G>> keySelectors)
        {
            throw new NotImplementedException();
        }
    }

    internal class MEnumerable<J, T> : IMEnumerable<J, T> where T : class
    {
        IMOrderBy<T> IMEnumerable<J, T>.Map(Func<J, T> selector)
        {
            throw new NotImplementedException();
        }
    }

    internal class MOrderBy<T> : MQueryBase<T>, IMOrderBy<T> where T : class
    {
        IMOrderByAfter<T> IMOrderBy<T>.OrderByAscending<TKey>(Expression<Func<T, TKey>> action)
        {
            throw new NotImplementedException();
        }

        IMOrderByAfter<T> IMOrderBy<T>.OrderByDescending<TKey>(Expression<Func<T, TKey>> action)
        {
            throw new NotImplementedException();
        }
    }

    internal class MOrderByAfter<T> : MQueryBase<T>, IMOrderByAfter<T> where T : class
    {
        IMQueryBaseList<T> IMOrderByAfter<T>.Skip(int offset)
        {
            throw new NotImplementedException();
        }

        IMQueryBaseList<T> IMOrderByAfter<T>.Take(int fetchRows)
        {
            throw new NotImplementedException();
        }

        IMQueryBaseList<T> IMOrderByAfter<T>.TakeWithOffset(int offsetRows, int fetchRows)
        {
            throw new NotImplementedException();
        }

        IMOrderByAfter<T> IMOrderByAfter<T>.ThenByAscending<TKey>(Expression<Func<T, TKey>> action)
        {
            throw new NotImplementedException();
        }

        IMOrderByAfter<T> IMOrderByAfter<T>.ThenByDescending<TKey>(Expression<Func<T, TKey>> action)
        {
            throw new NotImplementedException();
        }
    }

    internal class MQueryBase<T> : MQueryBaseList<T>, IMQueryBase<T> where T : class
    {
        T? IMQueryBase<T>.FirstOrDefault(IBHTransaction? transaction)
        {
            throw new NotImplementedException();
        }

        Task<T?> IMQueryBase<T>.FirstOrDefaultAsync(IBHTransaction? transaction)
        {
            throw new NotImplementedException();
        }

        T? IMQueryBase<T>.LastOrDefeult(IBHTransaction? transaction)
        {
            throw new NotImplementedException();
        }

        Task<T?> IMQueryBase<T>.LastOrDefeultAsync(IBHTransaction? transaction)
        {
            throw new NotImplementedException();
        }
    }

    internal class MQueryBaseList<T> : IMQueryBaseList<T> where T : class
    {
        List<T> IMQueryBaseList<T>.ToList(IBHTransaction? transaction)
        {
            throw new NotImplementedException();
        }

        Task<List<T>> IMQueryBaseList<T>.ToListAsync(IBHTransaction? transaction)
        {
            throw new NotImplementedException();
        }
    }
}
