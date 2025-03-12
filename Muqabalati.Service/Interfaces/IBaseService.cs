using Muqabalati.Domain.Global;

namespace Muqabalati.Service.Interfaces
{
    public interface IBaseService
    {
        Task<Result> SaveChangesAsync();
        Result SaveChanges();
    }
}
