using Interview.Domain.Common.IUnitOfWork;
using Interview.Domain.Global;
using Interview.Service.Interfaces;

namespace Interview.Service.Implementations
{
    public abstract class BaseService : IBaseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Result SaveChanges()
        {
            try
            {
                return _unitOfWork.SaveChanges();

            }
            catch (Exception ex)
            {
                return Result.Failure("");

            }
        }

        public async Task<Result> SaveChangesAsync()
        {
            return await _unitOfWork.SaveChangesAsync();

        }
    }
}
