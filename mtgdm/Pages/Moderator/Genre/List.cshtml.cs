using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using mtgdm.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace mtgdm.Pages.Moderator.Genre
{
    [Authorize(Roles = "Admin,Moderator")]
    public class ListModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public ListModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<mtgdm.Data.Genre> Genres { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Genres = await _context.Genre.AsNoTracking().ToListAsync();
            Genres = Genres.OrderBy(o => o.Name).ToList();
            return Page();
        }
    }
}
