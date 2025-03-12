using BookStore.Utilties;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Muqabalati.Web.ViewComponents
{
    public class ShoppingCartQuantityViewComponent : ViewComponent
    {

        //private readonly ShoppingCartServices _shoppingCartService;
        //private readonly SessionService _sessionService;

        //public ShoppingCartQuantityViewComponent(ShoppingCartServices shoppingCartService, SessionService sessionService)
        //{
        //    _shoppingCartService = shoppingCartService;
        //    _sessionService = sessionService;

        //}

        //public ShoppingCartServices ShoppingCartService { get; }

        //public async Task<IViewComponentResult> InvokeAsync()
        //{
        //    try
        //    {
        //        if (User?.Identity == null || !User.Identity.IsAuthenticated)
        //        {
        //            return View(0); 
        //        }

        //        ClaimsIdentity claimsIdentity = (ClaimsIdentity)User?.Identity;
        //        var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

        //        if (claim != null)
        //        {
        //            int userId = int.Parse(claim.Value);

        //            int? cartQuantityFromSession = _sessionService.GetFromSession<int>(SessionHelper.SessionCart);

        //            if (!cartQuantityFromSession.HasValue)
        //            {
        //                int cartQuantity = (int)await _shoppingCartService.GetShoppingItemsCountByUserIdAsync(userId);

        //                _sessionService.SetInSession<int>(SessionHelper.SessionCart, cartQuantity);
        //            }

        //            cartQuantityFromSession = _sessionService.GetFromSession<int>(SessionHelper.SessionCart);
        //            return View(cartQuantityFromSession.Value);
        //        }
        //        else
        //        {
        //            HttpContext.Session.Clear();
        //            return View(0);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["error"] = "An error occurred while retrieving the cart quantity.";
        //        return View(0);
        //    }
        //}
    }
}
