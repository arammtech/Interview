// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Muqabalati.Domain.Identity;
using Muqabalati.Utilities.Global;
using Muqabalati.Utilities.Identity;

namespace Muqabalati.Web.Areas.Identity.Pages.Account
{
    public class ConfirmEmailWithCodeModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _cache;

        public ConfirmEmailWithCodeModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IMemoryCache cache)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _cache = cache;
        }


        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required(ErrorMessage = "يجب إدخال هذا الحقل.")]
            public string Digit6 { get; set; }

            [Required(ErrorMessage = "يجب إدخال هذا الحقل.")]
            public string Digit5 { get; set; }

            [Required(ErrorMessage = "يجب إدخال هذا الحقل.")]
            public string Digit4 { get; set; }

            [Required(ErrorMessage = "يجب إدخال هذا الحقل.")]
            public string Digit3 { get; set; }

            [Required(ErrorMessage = "يجب إدخال هذا الحقل.")]
            public string Digit2 { get; set; }

            [Required(ErrorMessage = "يجب إدخال هذا الحقل.")]
            public string Digit1 { get; set; }

            public string Message { get; set; }
        }


        public async Task<IActionResult> OnGetAsync(string email, string? message)
        {
            if (!string.IsNullOrEmpty(email))
            {
                Input.Email = email;

                string verificationCode = GlobalFunctions.GetRandom(100000, 999999).ToString();

                _cache.Set(StoredDataPrefixes.VerificationCodeKey + email, verificationCode, TimeSpan.FromMinutes(StoredDataPrefixes.VerificationCodeKeyTime));

                await _emailSender.SendEmailAsync(
                    email,
                    "رمز التحقق من الحساب",
                    EmailMuqabalatis.GetVerificationCodeEmailBody(verificationCode, StoredDataPrefixes.VerificationCodeKeyTime));

                if (!string.IsNullOrEmpty(message))
                    Input.Message = message;
                else
                    Input.Message = "تم إرسال رمز التحقق إلى بريدك الإلكتروني";
            }
            else
            {
                return RedirectToPage("./ForgotPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            if (string.IsNullOrEmpty(Input.Email))
            {
                ModelState.AddModelError("", "يجب تقديم البريد الإلكتروني.");
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                return RedirectToPage("./ForgotPassword");
            }

            if (action == "verify")
            {
                var codeDigits = new string[] { Input.Digit1, Input.Digit2, Input.Digit3, Input.Digit4, Input.Digit5, Input.Digit6 };
                if (codeDigits.Any(digit => string.IsNullOrEmpty(digit)))
                {
                    ModelState.AddModelError("", "يجب إدخال جميع الأرقام الستة.");
                    Input.Message = "تم إرسال رمز التحقق إلى بريدك الإلكتروني";
                    return Page();
                }
                var storedCode = _cache.Get<string>(StoredDataPrefixes.VerificationCodeKey + Input.Email);
                var enteredCode = $"{Input.Digit1}{Input.Digit2}{Input.Digit3}{Input.Digit4}{Input.Digit5}{Input.Digit6}";

                if (storedCode == null)
                {
                    ModelState.AddModelError("", "انتهت صلاحية الرمز. الرجاء إعادة الإرسال.");
                    return Page();
                }

                if (enteredCode == storedCode)
                {
                    // Remove the code from cache after successful verification
                    _cache.Remove(StoredDataPrefixes.VerificationCodeKey + Input.Email);


                    // Generate a password reset token
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));


                    // Email Confirmed
                    var codeToConfirm = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                    var result = await _userManager.ConfirmEmailAsync(user, codeToConfirm);


                    // Redirect to Reset Password Page
                    return RedirectToPage("/Account/ResetPassword", new { area = "Identity", code, email = Input.Email });
                }
                else
                {
                    ModelState.AddModelError("", "رمز التحقق غير صحيح.");
                    Input.Message = "تم إرسال رمز التحقق إلى بريدك الإلكتروني";
                    return Page();
                }
            }
            else if (action == "resend")
            {
                return RedirectToPage("./ConfirmEmailWithCode", new { email = Input.Email, message = " تم إعادة إرسال رمز التحقق إلى بريدك الإلكتروني" });
            }

            return Page();
        }
    }
}
