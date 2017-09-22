using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace SuperSportDataEngine.Tests.Common.Repositories.Test
{
    public class TestEntityFrameworkRepository<T> : IBaseEntityFrameworkRepository<T> where T : class
    {
        private readonly List<T> _items;

        public TestEntityFrameworkRepository(List<T> items)
        {
            _items = items;
        }

        public T Add(T item)
        {
            _items.Add(item);
            return item;
        }

        public IEnumerable<T> AddRange(IEnumerable<T> items)
        {
            _items.AddRange(items);
            return _items;
        }

        public IEnumerable<T> All()
        {
            return _items;
        }

        public Task<IEnumerable<T>> AllAsync()
        {
            return Task.FromResult<IEnumerable<T>>(_items);
        }

        public Task<long> CountAsync()
        {
            return Task.FromResult<long>(_items.Count);
        }

        public Task<long> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return Task.FromResult<long>(_items.AsQueryable().Where(predicate).Count());
        }

        public void Delete(T item)
        {
            _items.Remove(item);
        }

        public void DeleteRange(IEnumerable<T> items)
        {
            foreach (var i in items)
                _items.Remove(i);
        }

        public Task<T> FindAsync(dynamic id)
        {
            // TODO
            return null;
        }

        public void RollBackPendingChanges()
        {
            // TODO
        }

        public Task<int> SaveAsync()
        {
            return Task.FromResult<int>(1);
        }

        public void Update(T item)
        {
            // TODO
        }

        public IEnumerable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _items.AsQueryable().Where(predicate);
        }

        public Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        {
            return Task.FromResult<IEnumerable<T>>(_items.AsQueryable().Where(predicate));
        }
    }
}
