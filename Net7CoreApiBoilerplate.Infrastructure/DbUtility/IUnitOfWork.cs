using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Net7CoreApiBoilerplate.Infrastructure.DbUtility
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext Context { get; }

        bool HasChanges();
        bool IsInTransaction();
        void ExecuteInTransaction(Action action);
        // void RollbackTransaction();
        void ClearChangeTracker();

        Task ClearDb(params string[] excluded);
        Task TruncateTables(params string[] tables);

        void Commit();
        Task CommitAsync();
        Task CommitAsync(bool skipCacheUpdate);

        IQueryable<T> Query<T>() where T : class, IEntity;
        IQueryable<T> Query<T>(Expression<Func<T, bool>> filter) where T : class, IEntity;
        IQueryable<T> GetQueryable<T>() where T : class, IEntity;

        // Procedures logic. ONLY IDEA taken from: https://stackoverflow.com/a/48165699/4267429 (more in implementation)
        Task<List<T>> ExecuteStoredProc<T>(string storedProcName, Dictionary<string, object> procParams) where T : class;

        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}