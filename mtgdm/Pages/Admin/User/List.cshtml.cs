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
    public class ListModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ListModel(ApplicationDbContext context,
                         IConfiguration config,
                         UserManager<IdentityUser> userManager,
                         RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<UserWithRole> Users { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {

            //https://stackoverflow.com/questions/41822458/linq-group-by-on-multiple-tables-with-nested-group-by-with-join-and-aggregate
            var query = from user in _context.Users
                        join userRole in _context.UserRoles on user.Id equals userRole.UserId into rt
                        from def in rt.DefaultIfEmpty()
                        join role in _context.Roles on def.RoleId equals role.Id into rl
                        from rol in rl.DefaultIfEmpty()
                        select new UserWithRole
                        {
                            User = user,
                            Role = rol.Name
                        };

            Users = await query.ToListAsync();
            
            return Page();
        }
        public class UserWithRole
        {
            public IdentityUser User { get; set; }
            public string Role { get; set; }
        }

    }
}
