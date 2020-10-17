using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    public class RatingModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public RatingModel(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [BindProperty(SupportsGet = true)]
        public string ShowpieceID { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal Rating { get; set; }
        
        public async Task<IActionResult> OnGetAsync()
        {
            return new RedirectToPageResult("/Showpiece/View", new { ShowpieceID });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!Guid.TryParse(ShowpieceID, out Guid showpieceID))
                return new NotFoundResult();

            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userID == null)
                return new NotFoundResult();

            var userID_guid = Guid.Parse(userID);

            //Clear any ratings that exist for this user/showpiece
            var showpieceRatings = _context.ShowpieceRating.Where(f => f.ShowpieceID == showpieceID && f.UserID == userID_guid).ToList();
            _context.RemoveRange(showpieceRatings);

            //Add a distinct rating
            var showpieceRating = new ShowpieceRating()
            {
                ShowpieceRatingID = Guid.NewGuid(),
                UserID = userID_guid,
                ShowpieceID = showpieceID,
                Rating = Rating
            };

            _context.ShowpieceRating.Add(showpieceRating);
            await _context.SaveChangesAsync();
            return new OkResult();
        }
    }
}
