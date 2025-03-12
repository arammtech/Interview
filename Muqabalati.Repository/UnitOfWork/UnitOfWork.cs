using Microsoft.EntityFrameworkCore.Storage;
using Muqabalati.Domain.Common.IRepository;
using Muqabalati.Domain.Common.IUnitOfWork;
using Muqabalati.Domain.Global;
using Muqabalati.Repository.EntityFrameworkCore.Context;
using Muqabalati.Repository.Repository;

namespace Muqabalati.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private IDbContextTransaction? _currentTransaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repositories = new Dictionary<Type, object>();
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (!_repositories.TryGetValue(typeof(TEntity), out var repository))
            {
                repository = new Repository<TEntity>(_context);
                _repositories[typeof(TEntity)] = repository;
            }

            return (IRepository<TEntity>)repository;
        }

        public Result SaveChanges()
        {
            try
            {
                var changes = _context.SaveChanges();
                return changes > 0 ? Result.Success() : Result.Failure("No changes were made to the database.");
            }
            catch (Exception ex)
            {
                return Result.Failure($"An error occurred while saving changes: {ex.Message}");
            }
        }

        public async Task<Result> SaveChangesAsync()
        {
            try
            {
                var changes = await _context.SaveChangesAsync();
                return changes > 0 ? Result.Success() : Result.Failure("No changes were made to the database.");
            }
            catch (Exception ex)
            {
                return Result.Failure($"An error occurred while saving changes: {ex.Message}");
            }
        }

        public Result StartTransaction()
        {
            try
            {
                if (_currentTransaction != null)
                    return Result.Failure("A transaction is already in progress.");

                _currentTransaction = _context.Database.BeginTransaction();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to start a transaction: {ex.Message}");
            }
        }

        public async Task<Result> StartTransactionAsync()
        {
            try
            {
                if (_currentTransaction != null)
                    return Result.Failure("A transaction is already in progress.");

                _currentTransaction = await _context.Database.BeginTransactionAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to start a transaction: {ex.Message}");
            }
        }

        public Result Commit()
        {
            try
            {
                if (_currentTransaction == null)
                    return Result.Failure("No active transaction to commit.");

                _context.SaveChanges();
                _currentTransaction.Commit();
                CleanupTransaction();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"An error occurred during commit: {ex.Message}");
            }
        }

        public async Task<Result> CommitAsync()
        {
            try
            {
                if (_currentTransaction == null)
                    return Result.Failure("No active transaction to commit.");

                await _context.SaveChangesAsync();
                await _currentTransaction.CommitAsync();
                CleanupTransaction();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"An error occurred during commit: {ex.Message}");
            }
        }

        public Result Rollback()
        {
            try
            {
                if (_currentTransaction == null)
                    return Result.Failure("No active transaction to roll back.");

                _currentTransaction.Rollback();
                CleanupTransaction();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"An error occurred during rollback: {ex.Message}");
            }
        }

        public async Task<Result> RollbackAsync()
        {
            try
            {
                if (_currentTransaction == null)
                    return Result.Failure("No active transaction to roll back.");

                await _currentTransaction.RollbackAsync();
                CleanupTransaction();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"An error occurred during rollback: {ex.Message}");
            }
        }

        private void CleanupTransaction()
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }

        public void Dispose()
        {
            try
            {
                _context.Dispose();
                _currentTransaction?.Dispose();
                //return Result.Success();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
