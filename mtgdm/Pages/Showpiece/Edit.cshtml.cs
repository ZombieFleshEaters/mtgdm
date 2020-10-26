using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using mtgdm.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mtgdm.Areas.Identity.Pages.Showpiece
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        public EditModel(ApplicationDbContext context,
                         IConfiguration config,
                         UserManager<IdentityUser> userManager)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
        }
               
        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        [BindProperty]
        public mtgdm.Data.Showpiece Showpiece { get; set; }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public IEnumerable<string> Genres { get; set; }

        [BindProperty]
        public List<SelectListItem> GenreChoices { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if(string.IsNullOrEmpty(Name))
                return new RedirectToPageResult("/Showpiece/List");

            Showpiece = await _context.Showpiece.FirstOrDefaultAsync(f => f.Slug == Name);
            if (Showpiece == null)
                return new RedirectToPageResult("/Showpiece/List");

            GenreChoices = await _context.Genre.Select(s => new SelectListItem
            {
                Value = s.GenreID.ToString(),
                Text = s.Name
            }).ToListAsync();

            Genres =  _context.Genre
                            .AsNoTracking()
                            .Join(_context.ShowpieceToGenre,
                            g => g.GenreID,
                            stg => stg.GenreID,
                            (g, stg) => new
                            {
                                stg.ShowpieceID,
                                g.GenreID,
                                g.Name,
                                g.Normalized
                            })
                            .Where(w => w.ShowpieceID == Showpiece.ShowpieceID)
                            .Select(s => s.GenreID.ToString())
                            .ToArray();

            var user = await _userManager.FindByIdAsync(Showpiece.UserID.ToString());
            Username = user.UserName;

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                GenreChoices = await _context.Genre.Select(s => new SelectListItem
                {
                    Value = s.GenreID.ToString(),
                    Text = s.Name
                }).ToListAsync();
                return Page();
            }

            foreach(var genre in Genres)
            {
                if(!await _context.Genre.AnyAsync(a => a.GenreID.ToString() == genre))
                {
                    GenreChoices = await _context.Genre.Select(s => new SelectListItem
                    {
                        Value = s.GenreID.ToString(),
                        Text = s.Name
                    }).ToListAsync();
                    ModelState.AddModelError("Validation.Genre.IsInvalid", "An unknown Genre was provided");
                    return Page();
                }
            }

            if (!await _context.Showpiece.AnyAsync(a => a.ShowpieceID == Showpiece.ShowpieceID))
            {
                return new RedirectToPageResult("/Showpiece/List");
            }

            if (await _userManager.FindByIdAsync(Showpiece.UserID.ToString()) == null)
            {
                return new RedirectToPageResult("/Showpiece/List");
            }

            Showpiece.Slug = mtgdm.Helpers.Slugify.ToUrlSlug(Showpiece.Title);

            if(await _context.Showpiece.AnyAsync(a => a.Slug.Equals(Showpiece.Slug, StringComparison.OrdinalIgnoreCase) && a.ShowpieceID != Showpiece.ShowpieceID))
            {
                GenreChoices = await _context.Genre.Select(s => new SelectListItem
                {
                    Value = s.GenreID.ToString(),
                    Text = s.Name
                }).ToListAsync();
                ModelState.AddModelError("Validation.Title.IsUsed", $"The title '{Showpiece.Title}' has already been taken");
                return Page();
            }

            if (Showpiece.Synopsis == null)
                Showpiece.Synopsis = "";

            _context.ShowpieceToGenre.RemoveRange(_context.ShowpieceToGenre.Where(w => w.ShowpieceID == Showpiece.ShowpieceID));
            var genres = Genres.Select(s => new ShowpieceToGenre()
            {
                ShowpieceToGenreID = Guid.NewGuid(),
                ShowpieceID = Showpiece.ShowpieceID,
                GenreID = Guid.Parse(s)
            });

            await _context.ShowpieceToGenre.AddRangeAsync(genres);
            _context.Showpiece.Update(Showpiece);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Showpiece/View", new { name = Showpiece.Slug });
        }
    }
}
