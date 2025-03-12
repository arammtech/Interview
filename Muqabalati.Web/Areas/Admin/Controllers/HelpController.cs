using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Muqabalati.Utilities.Identity;

namespace Muqabalati.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppUserRoles.RoleAdmin)]
    public class HelpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
