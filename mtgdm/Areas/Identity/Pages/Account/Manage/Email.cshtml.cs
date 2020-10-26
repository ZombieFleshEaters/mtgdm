using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using mtgdm.Services;
using mtgdm.Views.Emails.Callback;

namespace mtgdm.Areas.Identity.Pages.Account.Manage
{
    public partial class EmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        public EmailModel(UserManager<IdentityUser> userManager,
                          SignInManager<IdentityUser> signInManager,
                          IEmailSender emailSender,
                          IRazorViewToStringRenderer razorViewToStringRenderer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public bool ChangeEmailConfirmationSent { get; set; }

        [TempData]
        public string ChangeEmailStatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            //[Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };
            
            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return new RedirectResult("/Identity/Account/Login");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return new RedirectResult("/Identity/Account/Login");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                var callbackUrl = Url.Page("/Account/ConfirmEmailChange",
                                           pageHandler: null,
                                           values: new { userId = userId, email = Input.NewEmail, code = code },
                                           protocol: Request.Scheme);

                var emailCallback = new EmailCallbackViewModel(callbackUrl);
                string body = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/EmailChangeEmail.cshtml", emailCallback);
                await _emailSender.SendEmailAsync(Input.NewEmail, "mtgdm - Confirm your email", body);

                ChangeEmailStatusMessage = "Check your email for a link to change your email address. If it doesn’t appear within a few minutes, check your spam folder.";
                return RedirectToPage();
            }
            Email = email;
            ModelState.AddModelError("Validation.SameEmail", "Email addresses are the same");
            return Page();
        }
    }
}
