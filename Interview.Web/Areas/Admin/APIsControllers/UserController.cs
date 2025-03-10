using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using Interview.Domain.Identity;
using Interview.Repository.EntityFrameworkCore.Context;
using Interview.Service.DTOs.Admin;
using Interview.Service.Implementations;
using Interview.Service.Interfaces;
using Interview.Service.Mapper;
using Interview.Utilities.Identity;
using static System.Reflection.Metadata.BlobBuilder;
using AutoMapper;
using Interview.Domain.Common.IUnitOfWork;
using Interview.Domain.Global;

namespace Interview.Web.Areas.Admin.APIsControllers
{
    [Authorize(Roles = AppUserRoles.RoleAdmin)]
    [Route("api/admin/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly RoleManager<ApplicationRole> _roleManager;

      
        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, AppDbContext context, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
            _roleManager = roleManager;
            _context = context;

        }

        //C:\Users\meryk\AppData\Local\SourceServer\13a535b10cec5cb2768d6ab5c80731f49d6b54dc56d6977252e531ad338caf07\Interview.Web\Areas\Admin\APIsControllers\UserController.cs
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]int page, [FromQuery]int rowsPerPage, [FromQuery] string? filterProperty, [FromQuery] string? filter)
        {
            List<UserDto> lstUsers = [];
            (IEnumerable<UserDto> Users, int TotalRecords) result ;
            int usersCount = 0;
            bool? isLocked = null;
            string? role = null;

            try
            {
                // apply filter
                if (!(string.IsNullOrEmpty(filterProperty) && string.IsNullOrEmpty(filter)))
                {
                    if (filterProperty == "role")
                    {
                        role = filter;
                        result = (await _userService.GetUsersAsync(page, rowsPerPage, role));
                        lstUsers = result.Users.ToList();
                        usersCount = result.TotalRecords;

                    }
                    else if (filterProperty == "isLocked")
                    {
                        isLocked = filter.ToLower() == "true";
                        result = (await _userService.GetUsersAsync(page, rowsPerPage, null, null, isLocked));
                        lstUsers = result.Users.ToList();
                        usersCount = result.TotalRecords;

                    }

                    return Ok(new { success = true, data = lstUsers, totalUsers = usersCount });
                }

                result = await _userService.GetUsersAsync(page, rowsPerPage);
                lstUsers = result.Users.ToList();
                usersCount = result.TotalRecords;


                return Ok(new { success = true, data = lstUsers, totalUsers = usersCount });
            }
            catch (Exception ex)
            {
                // Optionally log the exception details
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving users." });
            }
        }

        // move this feature to the same method => make it return list and count or in a similar way
        private async Task<int> GetUsersCountAsync(string? role = null, Expression<Func<ApplicationUser, bool>>? filter = null, bool? isLocked = null)
        {
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
                    return 0;
                }

                usersQuery = from user in usersQuery
                             join userRole in _context.UserRoles on user.Id equals userRole.UserId
                             where userRole.RoleId == roleId
                             select user;
            }

            if (isLocked.HasValue)
            {
                if (isLocked.Value)
                {
                    usersQuery = usersQuery.Where(u => u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow);
                }
                else
                {
                    usersQuery = usersQuery.Where(u => !u.LockoutEnd.HasValue || u.LockoutEnd.Value <= DateTimeOffset.UtcNow);
                }
            }

            var userCount = usersQuery.Count();

            return userCount;
        }






        [HttpPost("LockUnLock")]
        public async Task<IActionResult> LockUnLock([FromQuery] int userId)
        {
            try
            {
                // Find the user by Id (adjust depending on your user ID type)
                var userFromDb = await _userManager.FindByIdAsync(userId.ToString());

                if (userFromDb == null)
                {
                    return NotFound(new { success = false, message = $"There is no user with the Id = {userId}" });
                }

                // Prevent locking the main admin user
                if (!userFromDb.LockoutEnabled)
                {
                    return BadRequest(new { success = false, message = "Oops! You cannot lock this user, they are the main user in this system." });
                }

                string result;

                // If the user is already locked, unlock them; otherwise, lock them
                if (userFromDb.LockoutEnd > DateTime.Now)
                {
                    userFromDb.LockoutEnd = DateTime.Now; // Unlock user
                    result = "unlocked";
                }
                else
                {
                    userFromDb.LockoutEnd = DateTime.Now.AddYears(1000); // Lock user for 1000 years
                    result = "locked";
                }

                await _userManager.UpdateAsync(userFromDb); // Save changes via UserManager

                return Ok(new { success = true, message = $"User has been {result} successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while locking/unlocking the user." });
            }
        }


        [HttpDelete]
        public async Task<IActionResult> Delete( int id)
        {
            if (id <= 0)
                return BadRequest(new { success = false, message = $"({id}) is an invalid Id" });

            try
            {
               var result = await _userService.DeleteUserAsync(id);
                if(result.IsSuccess)
                {
                    return Ok(new { success = true, message = "User deleted successfully!" });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "An error occurred while deleting the user." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the user." });
            }
        }



    }
}

//var lstUsers = _context.ApplicationUsers.Select(user => new
//{
//    user.Id,
//    Name = user.FirstName + user.LastName,
//    user.Email,
//    Phone = user.PhoneNumber,
//    Role = (from ur in _context.UserRoles
//            join r in _context.Roles on ur.RoleId equals r.Id
//            where ur.UserId == user.Id
//            select r.Name).FirstOrDefault(),
//    // Compare using UtcDateTime to ensure proper UTC comparison
//    IsLocked = user.LockoutEnd != null &&  user.LockoutEnd > DateTime.Now
//}).ToList();
