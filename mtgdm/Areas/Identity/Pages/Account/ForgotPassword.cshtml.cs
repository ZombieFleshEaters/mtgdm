using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using mtgdm.Views.Emails.Callback;
using mtgdm.Services;

namespace mtgdm.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        public ForgotPasswordModel(UserManager<IdentityUser> userManager, 
                                   IEmailSender emailSender,
                                   IRazorViewToStringRenderer razorViewToStringRenderer)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page("/Account/ResetPassword",
                                            pageHandler: null,
                                            values: new { area = "Identity", code },
                                            protocol: Request.Scheme);

                var emailCallback = new EmailCallbackViewModel(callbackUrl);
                string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/ForgotPasswordEmail.cshtml", emailCallback);
                await _emailSender.SendEmailAsync(Input.Email, "mtgdm - Reset password", body);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
