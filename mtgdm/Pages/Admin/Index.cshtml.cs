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
using mtgdm.Services;

namespace mtgdm.Pages.Admin
{
    [Authorize(Roles="Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public IndexModel(ApplicationDbContext context,
                          IConfiguration config,
                          UserManager<IdentityUser> userManager,
                          RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public long TotalUsers { get; set; }

        [BindProperty]
        public long TotalShowpieces { get; set; }

        [BindProperty]
        public long TotalRatings { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {

            TotalUsers = await _context.Users.LongCountAsync();
            TotalRatings = await _context.ShowpieceRating.LongCountAsync();
            TotalShowpieces = await _context.Showpiece.LongCountAsync();

            return Page();
        }
    }
}
