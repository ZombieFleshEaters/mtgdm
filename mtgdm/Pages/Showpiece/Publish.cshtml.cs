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

namespace mtgdm.Pages.Showpiece
{
    [Authorize]
    public class PublishModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public PublishModel(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        
        [BindProperty(SupportsGet = true)]
        public string ShowpieceID { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return new RedirectToPageResult("/Showpiece/View", new { ShowpieceID });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!Guid.TryParse(ShowpieceID, out Guid showpieceID))
            {
                return new RedirectToPageResult("/Showpiece/List");
            }

            var showpiece = await _context.Showpiece.FirstOrDefaultAsync(f => f.ShowpieceID == showpieceID);
            if(showpiece == null)
                return new RedirectToPageResult("/Showpiece/List");

            showpiece.Published = !showpiece.Published;
            _context.Showpiece.Update(showpiece);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Showpiece/View", new { name = showpiece.Slug });
        }
    }
}
