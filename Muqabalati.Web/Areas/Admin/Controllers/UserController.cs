using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Muqabalati.Domain.Identity;
using Muqabalati.Repository.EntityFrameworkCore.Context;
using Muqabalati.Service.DTOs.Admin;
using Muqabalati.Service.Implementations;
using Muqabalati.Service.Interfaces;
using Muqabalati.Utilities.Identity;
using Muqabalati.Web.Areas.Admin.ViewModels;

namespace Muqabalati.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppUserRoles.RoleAdmin)]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly RoleManager<ApplicationRole> _roleManager;


        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
            _roleManager = roleManager;

        }


        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                UserDto user = await _userService.GetUserByIdAsync(id);

                return View(user);
            }
            catch
            {
                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        public async Task<IActionResult> editRole(int id)
        {
            try
            {
                UserDto user = await _userService.GetUserByIdAsync(id);

                ChangeUserRoleDto changeUserRoleDto = new();
                changeUserRoleDto.Id = user.Id;
                changeUserRoleDto.oldRole = string.Join(", ", user.Role);
                changeUserRoleDto.Roles = _roleManager.Roles.ToList();
                //changeUserRoleDto.Roles = (await _userService.GetAllRolesAsync()).ToList();

                return View(changeUserRoleDto);
            }
            catch
            {
                return RedirectToAction("Index");
            }
          
        }

        [HttpPost]
        public async Task<IActionResult> editRole(int id, ChangeUserRoleDto changeUserRoleDto)
        {
            try
            {
                var result = await _userService.ChangeUserRoleAsync(id, changeUserRoleDto.oldRole, changeUserRoleDto.newRole);

                if(result.IsSuccess)
                {
                    return RedirectToAction("index");

                }
                else
                {
                    return View(changeUserRoleDto);
                }

            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
