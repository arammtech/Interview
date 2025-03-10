// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Interview.Domain.Identity;

namespace Interview.Web.Areas.Identity.Pages.Account
{
    public class ForgetPasswordResetModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ForgetPasswordResetModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

       
        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required(ErrorMessage = "كلمة المرور مطلوبة.")]
            [DataType(DataType.Password)]
            [Display(Name = "كلمة المرور")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "تأكيد كلمة المرور")]
            [Compare("Password", ErrorMessage = "كلمة المرور وتأكيد كلمة المرور غير متطابقين.")]
            public string ConfirmPassword { get; set; }


            [Required]
            public string Code { get; set; }

        }

        public IActionResult OnGet(string code = null, string email =null)
        {
            if (email == null)
            {
                return BadRequest("A email must be supplied for password reset.");
            }

            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
           
            Input = new InputModel
            {
                Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)),
                Email = email
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);

            if (result.Succeeded)
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
