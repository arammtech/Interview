using System.Linq.Expressions;

namespace Muqabalati.Domain.Common.IRepository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        #region Get
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter);
        TEntity? Get(Expression<Func<TEntity, bool>> filter);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null);
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter = null);
        IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> filter);
        IQueryable<TEntity> Table { get; }
        IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties);
        #endregion

        #region Add
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        #endregion

        #region Update
        Task UpdateAsync(TEntity entity);
        void Update(TEntity entity);
        Task UpdateRangeAsync(IEnumerable<TEntity> entities);
        void UpdateRange(IEnumerable<TEntity> entities);
        #endregion

        #region Delete
        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(Expression<Func<TEntity, bool>> filter);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities);
        void Delete(TEntity entity);
        void Delete(Expression<Func<TEntity, bool>> filter);
        void DeleteRange(IEnumerable<TEntity> entities);
        void SoftDelete(TEntity entity);
        void SoftDelete(Expression<Func<TEntity, bool>> filter);
        Task SoftDeleteAsync(TEntity entity);
        Task SoftDeleteAsync(Expression<Func<TEntity, bool>> filter);
        #endregion
    }
}
