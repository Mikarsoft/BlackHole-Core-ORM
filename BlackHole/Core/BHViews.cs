﻿using BlackHole.CoreSupport;

namespace BlackHole.Core
{
    /// <summary>
    /// A class that contains methods to get the Stored  Views of BlackHole
    /// </summary>
    public class BHViews : IBHViews
    {
        List<Dto> IBHViews.ExecuteView<Dto>(Action<BHOrderBy<Dto>> orderBy)
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            List<Dto> result = new();

            if (existingJoin != null)
            {
                JoinComplete<Dto> join = new(existingJoin);
                result = join.ExecuteQuery(orderBy);
            }

            return result;
        }

        List<Dto> IBHViews.ExecuteView<Dto>(Action<BHOrderBy<Dto>> orderBy, BHTransaction transaction)
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            List<Dto> result = new();

            if (existingJoin != null)
            {
                JoinComplete<Dto> join = new(existingJoin);
                result = join.ExecuteQuery(orderBy, transaction);
            }

            return result;
        }

        async Task<List<Dto>> IBHViews.ExecuteViewAsync<Dto>(Action<BHOrderBy<Dto>> orderBy)
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            List<Dto> result = new();

            if (existingJoin != null)
            {
                JoinComplete<Dto> join = new(existingJoin);
                result = await join.ExecuteQueryAsync(orderBy);
            }

            return result;
        }

        async Task<List<Dto>> IBHViews.ExecuteViewAsync<Dto>(Action<BHOrderBy<Dto>> orderBy, BHTransaction transaction)
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            List<Dto> result = new();

            if (existingJoin != null)
            {
                JoinComplete<Dto> join = new(existingJoin);
                result = await join.ExecuteQueryAsync(orderBy, transaction);
            }

            return result;
        }

        List<Dto> IBHViews.ExecuteView<Dto>()
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            List<Dto> result = new();

            if (existingJoin != null)
            {
                JoinComplete<Dto> join = new(existingJoin);
                result = join.ExecuteQuery();
            }

            return result;
        }

        List<Dto> IBHViews.ExecuteView<Dto>(BHTransaction transaction)
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            List<Dto> result = new();

            if (existingJoin != null)
            {
                JoinComplete<Dto> join = new(existingJoin);
                result = join.ExecuteQuery(transaction);
            }

            return result;
        }

        async Task<List<Dto>> IBHViews.ExecuteViewAsync<Dto>()
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            List<Dto> result = new();

            if (existingJoin != null)
            {
                JoinComplete<Dto> join = new(existingJoin);
                result = await join.ExecuteQueryAsync();
            }

            return result;
        }

        async Task<List<Dto>> IBHViews.ExecuteViewAsync<Dto>(BHTransaction transaction)
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            List<Dto> result = new();

            if (existingJoin != null)
            {
                JoinComplete<Dto> join = new(existingJoin);
                result = await join.ExecuteQueryAsync(transaction);
            }

            return result;
        }

        JoinsProcess<Dto> IBHViews.Using<Dto>()
        {
            return new JoinsProcess<Dto>();
        }
    }
}
