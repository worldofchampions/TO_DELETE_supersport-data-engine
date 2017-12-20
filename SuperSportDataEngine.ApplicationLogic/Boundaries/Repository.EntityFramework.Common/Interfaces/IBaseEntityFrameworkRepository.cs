namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IBaseEntityFrameworkRepository<T> : IDisposable where T : class
    {
        T Add(T item);

        IEnumerable<T> AddRange(IEnumerable<T> items);

        IEnumerable<T> All();

        Task<IEnumerable<T>> AllAsync();

        Task<long> CountAsync();

        Task<long> CountAsync(Expression<Func<T, bool>> predicate);

        void Delete(T item);

        void DeleteRange(IEnumerable<T> items);

        Task<T> FindAsync(dynamic id);

        void RollBackPendingChanges();

        void Update(T item);

        IEnumerable<T> Where(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate);

        IEnumerable<T> WhereIncludeLocal(Expression<Func<T, bool>> predicate);

        T FirstOrDefault(Expression<Func<T, bool>> predicate);
    }
}