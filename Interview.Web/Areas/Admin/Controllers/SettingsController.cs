using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Interview.Domain.Identity;
using Interview.Repository.EntityFrameworkCore.Context;
using Interview.Service.DTOs.Admin;
using Interview.Service.Interfaces;
using Interview.Utilities.Identity;

namespace Interview.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppUserRoles.RoleAdmin)]
    public class SettingsController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly RoleManager<ApplicationRole> _roleManager;


        public SettingsController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,  IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
            _roleManager = roleManager;

        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Profile()
        {

            try
            {
                UserDto admin = await _userService.GetUserByIdAsync(1);

                return View(admin);
            }
            catch
            {
                return RedirectToAction("Index");
            }

        }
    }
}
