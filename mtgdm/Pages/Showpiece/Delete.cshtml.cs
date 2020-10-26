using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using mtgdm.Data;
using Microsoft.AspNetCore.Identity;

namespace mtgdm.Pages.Showpiece
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        public DeleteModel(ApplicationDbContext context,
                           IConfiguration config,
                           UserManager<IdentityUser> userManager)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public mtgdm.Data.Showpiece Showpiece { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Name))
                return new RedirectToPageResult("/Showpiece/List");

            Showpiece = await _context.Showpiece.FirstOrDefaultAsync(f => f.Slug == Name);
            if (Showpiece == null)
                return new RedirectToPageResult("/Showpiece/List");

            var user = await _userManager.FindByIdAsync(Showpiece.UserID.ToString());
            Username = user.UserName;

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Showpiece.Remove(Showpiece);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Showpiece/List");
        }
    }
}
