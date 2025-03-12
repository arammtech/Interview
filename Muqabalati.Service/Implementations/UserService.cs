using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using Muqabalati.Domain.Common.IUnitOfWork;
using Muqabalati.Domain.Global;
using Muqabalati.Domain.Identity;
using Muqabalati.Service.DTOs.Admin;
using Muqabalati.Service.Interfaces;
using Muqabalati.Service.Mapper;
using Muqabalati.Utilities.Identity;
using Muqabalati.Repository.EntityFrameworkCore.Context;
using Muqabalati.Web.Areas.Admin.ViewModels;

namespace Muqabalati.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private Dictionary<string, List<ApplicationUser>> _roleUserDictionary;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, AppDbContext context, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _roleUserDictionary = new Dictionary<string, List<ApplicationUser>>();
            _mapper = mapper;
            _context = context;
        }

        private async Task BuildRoleUserDictionary()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                _roleUserDictionary[role.Name] = usersInRole.ToList();
            }
        }
        public async Task<(IEnumerable<UserDto> Users, int TotalRecords)> GetUsersAsync(int page, int pageSize = 10, string? role = null, Expression<Func<ApplicationUser, bool>>? filter = null, bool? isLocked = null)
        {
            if (page < 1 || pageSize < 1)
            {
                return (new List<UserDto>(), 0);
            }

            var usersQuery = _userManager.Users.AsQueryable();

            if (filter != null)
            {
                usersQuery = usersQuery.Where(filter);
            }

            if (!string.IsNullOrEmpty(role))
            {
                var roleId = await _roleManager.Roles
                    .Where(r => r.Name == role)
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                if (roleId == null)
                {
                    return (new List<UserDto>(), 0);
                }

                usersQuery = from user in usersQuery
                             join userRole in _context.UserRoles on user.Id equals userRole.UserId
                             where userRole.RoleId == roleId
                             select user;
            }

            // Check if isLocked parameter has a value
            if (isLocked.HasValue)
            {
                // If isLocked.Value is true, select users who are currently locked out
                usersQuery = isLocked.Value
                    ? usersQuery.Where(u => u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow)
                    // If isLocked.Value is false, select users who are not currently locked out
                    : usersQuery.Where(u => !u.LockoutEnd.HasValue || u.LockoutEnd.Value <= DateTimeOffset.UtcNow);
            }

            var totalRecords = await usersQuery.CountAsync();

            var paginatedUsers = await usersQuery
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    IsLocked = isLocked.HasValue ? isLocked.Value : false,
                    Role = (from userRole in _context.UserRoles
                            join role in _context.Roles on userRole.RoleId equals role.Id
                            where userRole.UserId == user.Id
                            select role.Name).ToList()
                })
                .ToListAsync();

            return (paginatedUsers, totalRecords);
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return null;
                }

                var roleNames = await _userManager.GetRolesAsync(user);

                return await UserDTOsMapper.ToUserDto(user, Task.FromResult(roleNames));
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return null;
            }
        }

        public async Task<UserDto?> GetAdminUserAsync(int userId)
        {
            try
            {
                var adminRole = await _roleManager.FindByNameAsync("Admin");
                if (adminRole == null)
                {
                    return null;
                }

                var adminUsers = await _userManager.GetUsersInRoleAsync(adminRole.Name);

                var adminUser = adminUsers.SingleOrDefault(x => x.Id == userId);

                if (adminUser == null)
                {
                    return null;
                }

                return await UserDTOsMapper.ToUserDto(adminUser, _userManager.GetRolesAsync(adminUser));
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return null;
            }
        }
        public async Task<Result> AddUserAsync(UserDto userDto)
        {
            if (userDto == null)
            {
                return Result.Failure("بيانات المستخدم غير صحيحة");
            }
            if (userDto == null)
            {
                return Result.Failure("بيانات المستخدم غير صحيحة");
            }

            var requiredFields = new Dictionary<string, string>
            {
                { userDto.FirstName, "الاسم الأول مطلوب" },
                { userDto.LastName, "اسم العائلة مطلوب" },
                { userDto.Email, "البريد الإلكتروني مطلوب" },
                { userDto.Phone, "رقم الهاتف مطلوب" },
                { userDto.Password, "كلمة المرور مطلوبة" }
            };

            foreach (var field in requiredFields)
            {
                if (string.IsNullOrWhiteSpace(field.Key))
                {
                    return Result.Failure(field.Value);
                }
            }
            string errorMsg;
            var transactionResult = await _unitOfWork.StartTransactionAsync();
            if (!transactionResult.IsSuccess)
            {
                errorMsg = transactionResult.ErrorMessage;
                return transactionResult;
            }

            try
            {

                var user = UserDTOsMapper.ToApplicationUser(userDto);
                var result = await _userManager.CreateAsync(user, user.PasswordHash); // Use a default or temporary password

                if (result.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, userDto.Role.FirstOrDefault());
                    if (!roleResult.Succeeded)
                    {
                        await _unitOfWork.RollbackAsync();
                        errorMsg = roleResult.Errors.Select(e => e.Description).FirstOrDefault();

                        return Result.Failure("حدث خطا اثناء اضافة يوزر");
                    }

                    var commitResult = await _unitOfWork.CommitAsync();
                    if (!commitResult.IsSuccess)
                    {
                        await _unitOfWork.RollbackAsync();
                        errorMsg = commitResult.ErrorMessage;
                        return commitResult;
                    }

                    return Result.Success();
                }

                await _unitOfWork.RollbackAsync();
                errorMsg = result.Errors.Select(e => e.Description).FirstOrDefault();
                return Result.Failure($"فشل في إضافة المستخدم: ");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result.Failure($"فشل في إضافة المستخدم: ");
            }
        }

        public async Task<Result> UpdateUserAsync(UserDto userDto)
        {
            if (userDto == null)
            {
                return Result.Failure("بيانات المستخدم غير صحيحة");
            }

            if (userDto.Id <= 0)
            {
                return Result.Failure("معرف المستخدم غير صحيح");
            }

            if (string.IsNullOrWhiteSpace(userDto.FirstName))
            {
                return Result.Failure("الاسم الأول مطلوب");
            }

            if (string.IsNullOrWhiteSpace(userDto.LastName))
            {
                return Result.Failure("اسم العائلة مطلوب");
            }

            if (string.IsNullOrWhiteSpace(userDto.Email))
            {
                return Result.Failure("البريد الإلكتروني مطلوب");
            }

            if (string.IsNullOrWhiteSpace(userDto.UserName))
            {
                return Result.Failure("اسم المستخدم مطلوب");
            }

            if (string.IsNullOrWhiteSpace(userDto.Phone))
            {
                return Result.Failure("رقم الهاتف مطلوب");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userDto.Id.ToString());

                if (user == null)
                {
                    return Result.Failure("المستخدم غير موجود");
                }

                // Perform business logic/validation here

                user.FirstName = userDto.FirstName;
                user.LastName = userDto.LastName;
                user.Email = userDto.Email;
                user.UserName = userDto.UserName;
                user.PhoneNumber = userDto.Phone;
                user.ImagePath = userDto.ImagePath;

                var result = await _userManager.UpdateAsync(user);

                return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).FirstOrDefault());
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Result.Failure($"فشل في تحديث المستخدم: {ex.Message}");
            }
        }


        public async Task<Result> LockUserAsync(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return Result.Failure("User not found");
                }

                var isAdmin = await _userManager.GetRolesAsync(user);

                if (isAdmin.Contains(AppUserRoles.RoleAdmin))
                {
                    return Result.Failure("لا تستطيع ان تنفذ هذه العملية على الادمن");
                }

                user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100); // Lockout indefinitely
                var result = await _userManager.UpdateAsync(user);

                return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).FirstOrDefault());
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Result.Failure($"Failed to lock user: {ex.Message}");
            }
        }

        public async Task<Result> UnlockUserAsync(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return Result.Failure("User not found");
                }

                user.LockoutEnd = null; // Remove lockout
                var result = await _userManager.UpdateAsync(user);

                return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).FirstOrDefault());
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Result.Failure($"Failed to unlock user: {ex.Message}");
            }
        }

        public async Task<Result> ChangeUserRoleAsync(int userId, string oldRole, string newRole)
        {
            if (userId <= 0 || oldRole == newRole) return Result.Failure("Some Thing Erroe in parameter");

            var transactionResult = await _unitOfWork.StartTransactionAsync();
            if (!transactionResult.IsSuccess)
            {
                return Result.Failure(transactionResult.ErrorMessage);
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return Result.Failure("User not found");
                }

                var removeResult = await _userManager.RemoveFromRoleAsync(user, oldRole);
                if (!removeResult.Succeeded)
                {
                    await _unitOfWork.RollbackAsync();
                    return Result.Failure(removeResult.Errors.Select(e => e.Description).FirstOrDefault());
                }

                var addResult = await _userManager.AddToRoleAsync(user, newRole);
                if (!addResult.Succeeded)
                {
                    await _unitOfWork.RollbackAsync();
                    return Result.Failure(addResult.Errors.Select(e => e.Description).FirstOrDefault());
                }

                var commitResult = await _unitOfWork.CommitAsync();
                return commitResult.IsSuccess ? Result.Success() : Result.Failure(commitResult.ErrorMessage);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                // Log the exception (optional)
                return Result.Failure($"Failed to change user role: {ex.Message}");
            }
        }


        public async Task<List<ApplicationRole>> GetUserRolesAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var roleNames = await _userManager.GetRolesAsync(user);

            var roles = await _roleManager.Roles.Where(r => roleNames.Contains(r.Name)).ToListAsync();

            return roles;
        }



        public async Task<ChangeUserRoleDto?> ChangeUserRoleAndReturnDtoAsync(int userId, string oldRole, string newRole)
        {
            if (userId <= 0 || oldRole == newRole) return null;

            var transactionResult = await _unitOfWork.StartTransactionAsync();
            if (!transactionResult.IsSuccess)
            {
                return null;
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return null;
                }

                var removeResult = await _userManager.RemoveFromRoleAsync(user, oldRole);
                if (!removeResult.Succeeded)
                {
                    await _unitOfWork.RollbackAsync();
                    return null;
                }

                var addResult = await _userManager.AddToRoleAsync(user, newRole);
                if (!addResult.Succeeded)
                {
                    await _unitOfWork.RollbackAsync();
                    return null;
                }

                var commitResult = await _unitOfWork.CommitAsync();
                if (!commitResult.IsSuccess)
                {
                    return null;
                }

                var roles = await GetUserRolesAsync(userId);

                return new ChangeUserRoleDto
                {
                    Id = userId,
                    oldRole = oldRole,
                    newRole = newRole,
                    Roles = roles
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                // Log the exception (optional)
                throw new Exception($"Failed to change user role: {ex.Message}");

            }
            return null;

        }


        public async Task<List<string>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return roles;
        }
        public async Task<List<ApplicationRole>?> GetAllApplicationRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles;
        }

        public async Task<Result> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return Result.Failure("User not found");
                }

                var result = await _userManager.DeleteAsync(user);

                return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).FirstOrDefault());
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Result.Failure($"Failed to delete user: {ex.Message}");
            }
        }
    }
}
