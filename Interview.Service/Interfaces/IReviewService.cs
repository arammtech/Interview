using System.Linq.Expressions;
using Interview.Domain.Entities;
using Interview.Domain.Global;
using Interview.Service.DTOs;

namespace Interview.Service.Interfaces
{
    public interface IReviewService : IBaseService
    {
        Task<Result> AddAsync(Review departmentDto);
        Task<Result> UpdateAsync(Review departmentDto);
        Task<Result> DeleteAsync(int id);
    }

}
