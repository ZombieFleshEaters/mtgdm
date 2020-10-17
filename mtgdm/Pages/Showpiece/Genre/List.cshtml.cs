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

namespace mtgdm.Pages.Showpiece.Genre
{
    [AllowAnonymous]
    public class ListModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public ListModel(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [BindProperty]
        public List<mtgdm.Data.Genre> Genres { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Genres = await _context.Genre.ToListAsync();
            return new JsonResult(Genres);
        }
    }
}
