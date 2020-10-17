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

namespace mtgdm.Areas.Identity.Pages.Showpiece
{
    [AllowAnonymous]
    public class ListModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        public ListModel(ApplicationDbContext context, 
                         IConfiguration config, 
                         UserManager<IdentityUser> userManager)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Sort { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        

    }
}
