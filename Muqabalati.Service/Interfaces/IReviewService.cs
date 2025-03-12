using System.Linq.Expressions;
using Muqabalati.Domain.Entities;
using Muqabalati.Domain.Global;
using Muqabalati.Service.DTOs;

namespace Muqabalati.Service.Interfaces
{
    public interface IReviewService : IBaseService
    {
        Task<Result> AddAsync(Review departmentDto);
        Task<Result> UpdateAsync(Review departmentDto);
        Task<Result> DeleteAsync(int id);
    }

}
