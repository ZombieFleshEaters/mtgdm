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

    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public DeleteModel(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [BindProperty(SupportsGet = true)]
        public string SystemValueKey { get; set; }


        [BindProperty]
        public SystemValue SystemValue { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(SystemValueKey))
                return new RedirectToPageResult("/Admin/SystemValues/List");

            SystemValue = await _context.SystemValue.FirstOrDefaultAsync(f => f.Key == SystemValueKey);
            if (SystemValue == null)
                return new RedirectToPageResult("/Admin/SystemValues/List");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.SystemValue.Remove(SystemValue);
            await _context.SaveChangesAsync();
            return new RedirectToPageResult("/Admin/SystemValues/List");
        }
    }
}
