namespace SuperSportDataEngine.Repository.EntityFramework.Common.Repositories.Base
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class BaseEntityFrameworkRepository<T> : IBaseEntityFrameworkRepository<T> where T : class
    {
        protected readonly DbContext _dbContext;

        public BaseEntityFrameworkRepository(DbContext context)
        {
            _dbContext = context;
            ((IObjectContextAdapter)_dbContext).ObjectContext.CommandTimeout = 120;
        }

        public T Add(T item)
        {
            AsSet().Add(item);

            return item;
        }

        public IEnumerable<T> AddRange(IEnumerable<T> items)
        {
            return AsSet().AddRange(items);
        }

        public IEnumerable<T> All()
        {
            return AsSet();
        }

        public async Task<IEnumerable<T>> AllAsync()
        {
            return await AsSet().ToListAsync();
        }

        public async Task<long> CountAsync()
        {
            return await AsSet().LongCountAsync();
        }

        public async Task<long> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await AsSet().LongCountAsync(predicate);
        }

        public void Delete(T item)
        {
            AsSet().Remove(item);
        }

        public void DeleteRange(IEnumerable<T> items)
        {
            AsSet().RemoveRange(items);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<T> FindAsync(dynamic id)
        {
            return await AsSet().FindAsync(id);
        }

        public void RollBackPendingChanges()
        {
            var changedEntries = _dbContext.ChangeTracker.Entries()
                .Where(x => x.State != EntityState.Unchanged)
                .ToList();

            foreach (var entry in changedEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged;
                        break;

                    case EntityState.Modified:
                        entry.CurrentValues.SetValues(entry.OriginalValues);
                        entry.State = EntityState.Unchanged;
                        break;
                }
            }
        }

        public void Update(T item)
        {
            var entry = item as DbEntityEntry<T>;

            if (entry != null)
            {
                entry.State = EntityState.Modified;
            }
        }

        public virtual IEnumerable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return AsSet().Where(predicate);
        }

        public virtual async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await AsSet().Where(predicate).ToListAsync();
        }
        public virtual async Task<IEnumerable<T>> WhereAsyncAsNoTracking(Expression<Func<T, bool>> predicate)
        {
            return await AsSet().Where(predicate).AsNoTracking().ToListAsync();
        }

        public virtual IEnumerable<T> WhereIncludeLocal(Expression<Func<T, bool>> predicate)
        {
            var dbResult = AsSet().Where(predicate).ToList();
            var offlineResult = AsSet().Local.AsQueryable().Where(predicate).ToList();
            return offlineResult.Union(dbResult);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return AsSet().FirstOrDefault(predicate);
        }

        protected DbSet<T> AsSet()
        {
            return _dbContext.Set<T>();
        }

       
    }
}