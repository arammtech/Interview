using Muqabalati.Domain.Common.IUnitOfWork;
using Muqabalati.Domain.Global;
using Muqabalati.Service.Interfaces;

namespace Muqabalati.Service.Implementations
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
