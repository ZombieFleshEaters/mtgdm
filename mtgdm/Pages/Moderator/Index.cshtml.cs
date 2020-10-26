using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using mtgdm.Data;

namespace mtgdm.Pages.Moderator
{
    [Authorize(Roles = "Admin,Moderator")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public IndexModel(ApplicationDbContext context,
                          IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [BindProperty]
        public long TotalUnresolvedReports { get; set; }

        [BindProperty]
        public long TotalResolvedReports { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            TotalUnresolvedReports = await _context.Report.Where(w => w.Resolved == new DateTime(9999,12,31)).LongCountAsync();
            TotalResolvedReports = await _context.Report.Where(w => w.Resolved < new DateTime(9999, 12, 31)).LongCountAsync();

            return Page();
        }
    }
}
