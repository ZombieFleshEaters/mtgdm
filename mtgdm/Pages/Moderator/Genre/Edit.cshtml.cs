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

namespace mtgdm.Pages.Moderator.Genre
{
    [Authorize(Roles = "Admin,Moderator")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public EditModel(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [BindProperty(SupportsGet = true)]
        public string GenreID { get; set; }

        [BindProperty]
        public mtgdm.Data.Genre Genre { get; set;}

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
            if(!ModelState.IsValid)
            {
                return Page();
            }

            if (await _context.Genre.AnyAsync(a => a.Normalized == Genre.Name.ToUpper() && a.GenreID != Genre.GenreID))
            {
                ModelState.AddModelError("Validation.Key.Duplicate", $"The Genre '{Genre.Name}' already exists");
                return Page();
            }

            Genre.Normalized = Genre.Name.ToUpper();

            _context.Genre.Update(Genre);
            await _context.SaveChangesAsync();
            return new RedirectToPageResult("/Moderator/Genre/List");
        }

    }
}
