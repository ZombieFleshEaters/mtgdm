using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using mtgdm.Data;

namespace mtgdm.Pages.Admin.User
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public EditModel(ApplicationDbContext context,
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
        public IdentityUser UserEdit { get; set; }

        [BindProperty]
        public List<SelectListItem> UserRoles { get; set; }

        [BindProperty]
        public string UserRole { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if(string.IsNullOrEmpty(UserID) || !Guid.TryParse(UserID, out Guid userID))
            {
                return new RedirectToPageResult("/Admin/User/List");
            }

            UserEdit = await _userManager.FindByIdAsync(UserID);
            var roles = await _userManager.GetRolesAsync(UserEdit);
            UserRoles = await _context.Roles.Select(s => new SelectListItem
            {
                Value = s.NormalizedName,
                Text = s.Name,
                Selected = roles.Contains(s.Name)
            }).ToListAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(UserID) || !Guid.TryParse(UserID, out Guid userID))
            {
                return new RedirectToPageResult("/Admin/User/List");
            }
            var userRoles = await _userManager.GetRolesAsync(UserEdit);
            var user = await _userManager.FindByIdAsync(UserEdit.Id);
            var remove = await _userManager.RemoveFromRolesAsync(user, userRoles);
            var result = await _userManager.AddToRoleAsync(user, UserRole);
            if(!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }
            UserRoles = await _context.Roles.Select(s => new SelectListItem
            {
                Value = s.NormalizedName,
                Text = s.Name,
                Selected = userRoles.Contains(s.Name)
            }).ToListAsync();

            return new RedirectToPageResult("/Admin/User/List");
        }
    }
}
