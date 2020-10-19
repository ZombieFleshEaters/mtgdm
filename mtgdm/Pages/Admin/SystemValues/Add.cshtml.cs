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

namespace mtgdm.Pages.Admin.SystemValues
{
    [Authorize(Roles = "Admin")]
    public class AddModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public AddModel(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [BindProperty]
        public SystemValue SystemValue { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if(await _context.SystemValue.AnyAsync(a => a.Key == SystemValue.Key))
            {
                ModelState.AddModelError("Validation.Key.Duplicate", $"The key '{SystemValue.Key}' already exists");
                return Page();
            }

            _context.SystemValue.Add(SystemValue);
            await _context.SaveChangesAsync();
            return new RedirectToPageResult("/Admin/SystemValues/List");
        }
    }
}
