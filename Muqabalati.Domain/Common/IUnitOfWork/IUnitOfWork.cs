using Muqabalati.Domain.Common.IRepository;
using Muqabalati.Domain.Global;

namespace Muqabalati.Domain.Common.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;

        Result SaveChanges();
        Task<Result> SaveChangesAsync();

        Result StartTransaction();
        Task<Result> StartTransactionAsync();

        Result Commit();
        Task<Result> CommitAsync();

        Result Rollback();
        Task<Result> RollbackAsync();
    }
}
