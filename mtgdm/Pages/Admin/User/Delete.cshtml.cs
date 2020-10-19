using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using mtgdm.Data;

namespace mtgdm.Pages.Admin.User
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DeleteModel(ApplicationDbContext context,
                         IConfiguration config,
                         UserManager<IdentityUser> userManager,
                         RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty(SupportsGet = true)]
        public string UserID { get; set; }


        [BindProperty]
        public IdentityUser UserDelete { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(UserID) || !Guid.TryParse(UserID, out Guid userID))
            {
                return new RedirectToPageResult("/Admin/User/List");
            }

            UserDelete = await _userManager.FindByIdAsync(UserID);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(UserID) || !Guid.TryParse(UserID, out Guid userID))
            {
                return new RedirectToPageResult("/Admin/User/List");
            }

            var user = await _userManager.FindByIdAsync(UserDelete.Id);
            var userRoles = await _userManager.GetRolesAsync(user);
            var remove = await _userManager.RemoveFromRolesAsync(user, userRoles);

            //Remove showpieces
            var ratings = await _context.ShowpieceRating.Where(w => w.UserID == userID).ToListAsync();
            var comments = await _context.Comment.Where(w => w.UserID == userID).ToListAsync();

            _context.ShowpieceRating.RemoveRange(ratings);
            _context.Comment.RemoveRange(comments);

            await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Admin/User/List");
        }
    }
}
