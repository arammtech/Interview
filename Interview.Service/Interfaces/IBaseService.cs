using Interview.Domain.Global;

namespace Interview.Service.Interfaces
{
    public interface IBaseService
    {
        Task<Result> SaveChangesAsync();
        Result SaveChanges();
    }
}
