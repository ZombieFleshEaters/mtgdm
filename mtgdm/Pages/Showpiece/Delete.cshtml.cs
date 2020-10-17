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

namespace mtgdm.Pages.Showpiece
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public DeleteModel(ApplicationDbContext context,
                        IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [BindProperty(SupportsGet = true)]
        public string ShowpieceID { get; set; }

        [BindProperty]
        public mtgdm.Data.Showpiece Showpiece { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!Guid.TryParse(ShowpieceID, out Guid showpieceID))
            {
                return new RedirectToPageResult("/Showpiece/List");
            }

            Showpiece = await _context.Showpiece.FirstOrDefaultAsync(f => f.ShowpieceID == showpieceID);
            if (Showpiece == null)
                return new RedirectToPageResult("/Showpiece/List");

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Showpiece.Remove(Showpiece);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Showpiece/List");
        }
    }
}
