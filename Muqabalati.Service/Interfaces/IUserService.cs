using static Muqabalati.Domain.Global.Result;
using Muqabalati.Service.DTOs.Admin;
using Muqabalati.Domain.Global;
using System.Linq.Expressions;
using Muqabalati.Domain.Identity;
using Muqabalati.Web.Areas.Admin.ViewModels;

namespace Muqabalati.Service.Interfaces
{
    public interface IUserService
    {
        //  Task<PaginatedResult<UserDto>> GetUsersAsync(int page, int pageSize, string role);

        Task<(IEnumerable<UserDto> Users, int TotalRecords)> GetUsersAsync(int page, int pageSize = 10, string? role = null, Expression<Func<ApplicationUser, bool>>? filter = null, bool? isLocked = null);
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<Result> AddUserAsync(UserDto userDto);
        Task<Result> UpdateUserAsync(UserDto userDto);
        Task<Result> DeleteUserAsync(int userId);

        Task<List<string>> GetAllRolesAsync();

        Task<Result> ChangeUserRoleAsync(int userId, string oldRole, string newRole);

        Task<ChangeUserRoleDto?> ChangeUserRoleAndReturnDtoAsync(int userId, string oldRole, string newRole);

        Task<List<ApplicationRole>> GetUserRolesAsync(int userId);

        Task<Result> UnlockUserAsync(int userId);

        Task<Result> LockUserAsync(int userId);

        Task<UserDto?> GetAdminUserAsync(int userId);

        Task<List<ApplicationRole>?> GetAllApplicationRolesAsync();
    }
}
