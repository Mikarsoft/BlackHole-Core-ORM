using Mikarsoft.BlackHoleCore.Connector;
using Mikarsoft.BlackHoleCore.Entities;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore
{
    internal class BHTable<T, G> : BHTable<T>, IBHTable<T, G> where G : struct, IBHStruct where T : BHEntityAI<T, G>
    {
        public T? GetById(G Id, IBHTransaction? transaction = null)
        {
            string selectCommand = _commandBuilder.SelectCommand<T>();
            BHStatement query = WhereBuild(x => x.Id.Equals(Id), selectCommand);
            return _dataProvider.QueryFirst<T>(query.Command, query.InnerParameters);
        }

        public Dto? GetById<Dto>(G Id, IBHTransaction? transaction = null) where Dto : BHDto<G>
        {
            string selectCommand = _commandBuilder.SelectCommand<Dto>();
            BHStatement query = WhereBuild(x => x.Id.Equals(Id), selectCommand);
            return _dataProvider.QueryFirst<Dto>(query.Command, query.InnerParameters);
        }

        public IBHQueryUpdatable<T> UpdateById(T entry)
        {
            return new BHQueryUpdatable<T>(entry, _dataProvider);
        }

        public IBHQueryUpdatable<T, Dto> UpdateById<Dto>(Dto entry) where Dto : BHDto<G>
        {
            throw new NotImplementedException();
        }

        public IBHQueryUpdatable<T> UpdateById(List<T> entries)
        {
            throw new NotImplementedException();
        }

        public IBHQueryUpdatable<Dto> UpdateById<Dto>(List<Dto> entries) where Dto : BHDto<G>
        {
            throw new NotImplementedException();
        }

        public bool DeleteById(G Id, IBHTransaction? transaction = null)
        {
            string deleteCommand = _commandBuilder.DeactivateCommand<T>();
            BHExpressionPart[] parts = WhereBuild(x => x.Id.Equals(Id));
            return _dataProvider.JustExecute(query.Command, query.InnerParameters);
        }

        public bool RemoveById(G Id, IBHTransaction? transaction = null)
        {
            string deleteCommand = _commandBuilder.DeleteCommand<T>();
            BHExpressionPart[] parts = WhereBuild(x => x.Id.Equals(Id));
            return _dataProvider.JustExecute(query.Command, query.InnerParameters);
        }

        public bool RestoreById(G Id, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public List<G> GetIdsWhere(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public async Task<T?> GetByIdAsync(G Id, IBHTransaction? transaction = null)
        {
            string selectCommand = _commandBuilder.SelectCommand<T>();
            BHStatement query = WhereBuild(x => x.Id.Equals(Id), selectCommand);
            return await _dataProvider.QueryFirstAsync<T>(query.Command, query.InnerParameters);
        }

        public async Task<Dto?> GetByIdAsync<Dto>(G Id, IBHTransaction? transaction = null) where Dto : BHDto<G>
        {
            string selectCommand = _commandBuilder.SelectCommand<Dto>();
            BHStatement query = WhereBuild(x => x.Id.Equals(Id), selectCommand);
            return await _dataProvider.QueryFirstAsync<Dto>(query.Command, query.InnerParameters);
        }

        public Task<bool> DeleteByIdAsync(G Id, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveByIdAsync(G Id, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RestoreByIdAsync(G Id, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<G>> GetIdsAsyncWhere(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        IBHQueryUpdatable<T, Dto> IBHTable<T, G>.UpdateById<Dto>(List<Dto> entries)
        {
            throw new NotImplementedException();
        }
    }

    internal class BHTable<T> : BHContextBase, IBHTable<T> where T : BHEntity<T>
    {
        internal readonly IBHDataProvider _dataProvider;
        internal readonly IBHCommandBuilder _commandBuilder;
        internal readonly EntitySettings<T> _settings;

        public BHTable()
        {
            _dataProvider = BHServiceInjector.GetDataProvider();
            _commandBuilder = BHServiceInjector.GetCommandBuilder();

            if (Activator.CreateInstance(typeof(T)) is T entity)
            {
                _settings = entity.EntityOptions(new EntityOptionsBuilder<T>());
            }
            else
            {
                _settings = new();
            }
        }

        internal BHExpressionPart[] WhereBuild(Expression<Func<T, bool>> predicate)
        {
            return BHExpressionParser.ParseExpression(predicate);
        }

        public bool Any(IBHTransaction? transaction = null)
        {
            string selectCommand = _commandBuilder.SelectCommand<T>();
            return _dataProvider.QueryFirst<T>(selectCommand) != null;
        }

        public bool Any(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public int Count(IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllEntries(IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllEntriesAsync(IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public bool DeleteEntriesWhere(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public bool InsertEntries(IBHQuery<T> entries, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertEntriesAsync(IBHQuery<T> entries, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public bool InsertEntry(T entry, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertEntryAsync(T entry, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public IBHQuerySearchable<T> Select()
        {
            throw new NotImplementedException();
        }

        public IBHQueryJoinable<Dto, T> Select<Dto>() where Dto : BHDto
        {
            throw new NotImplementedException();
        }

        public IBHQueryUpdatable<T> UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry)
        {
            throw new NotImplementedException();
        }

        public IBHQueryUpdatable<T, Columns> UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class
        {
            throw new NotImplementedException();
        }
    }
}
