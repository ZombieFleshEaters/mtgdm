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
        public mtgdm.Data.Genre Genre { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (await _context.Genre.AnyAsync(a => a.Normalized == Genre.Name.ToUpper()))
            {
                ModelState.AddModelError("Validation.Key.Duplicate", $"The Genre '{Genre.Name}' already exists");
                return Page();
            }

            Genre.GenreID = Guid.NewGuid();
            Genre.Normalized = Genre.Name.ToUpper();

            _context.Genre.Add(Genre);
            await _context.SaveChangesAsync();
            return new RedirectToPageResult("/Moderator/Genre/List");
        }

    }
}
