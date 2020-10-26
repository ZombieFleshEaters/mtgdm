using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using mtgdm.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace mtgdm.Pages.Moderator.Genre
{
    [Authorize(Roles = "Admin,Moderator")]
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
        public string GenreID { get; set; }

        [BindProperty]
        public mtgdm.Data.Genre Genre { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(GenreID) || !Guid.TryParse(GenreID, out Guid genreID))
                return new RedirectToPageResult("/Moderator/Genre/List");

            Genre = await _context.Genre.FirstOrDefaultAsync(f => f.GenreID == genreID);
            if (Genre == null)
                return new RedirectToPageResult("/Moderator/Genre/List");

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var showpieceGenres = await _context.ShowpieceToGenre.Where(w => w.GenreID == Genre.GenreID).ToListAsync();

            _context.ShowpieceToGenre.RemoveRange(showpieceGenres);
            _context.Genre.Remove(Genre);

            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Moderator/Genre/List");
        }
    }
}
