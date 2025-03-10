using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Interview.Domain.Identity;
using Interview.Web.Models;

namespace Interview.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null &&  User.Identity.IsAuthenticated)
            {
                if (!(await _userManager.IsEmailConfirmedAsync(user)))
                    TempData["info"] = $"مرحبًا {user.UserName.Split('@')[0]} !الرجاء التحقق من بريدك الإلكتروني للتأكيد!";
                else
                    TempData["info"] = $"مرحبًا {user.UserName.Split('@')[0]}!";
            }


            return View();
        }
        public IActionResult Help()
        {
            // FAQ
            // Goals
            // Contact Us

            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult Goals()
        {
            return View();
        }

        public IActionResult ContactUs() { 
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
