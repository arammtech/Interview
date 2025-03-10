using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Interview.Utilities.Identity;

namespace Interview.Web.Areas.Admin.Controllers
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
