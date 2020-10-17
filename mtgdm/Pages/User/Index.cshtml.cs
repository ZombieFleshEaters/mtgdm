using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using mtgdm.Data;

namespace mtgdm.Pages.User
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ApplicationDbContext context,
                        IConfiguration config,
                        UserManager<IdentityUser> userManager)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string UserID { get; set; }

        [BindProperty]
        public string Username { get; set; }
        
        [BindProperty]
        public List<mtgdm.Data.Showpiece> Showpieces { get; set; }

        [BindProperty]
        public decimal AverageRating { get; set; }

        [BindProperty]
        public decimal RatingsGiven { get; set; }

        [BindProperty]
        public decimal RatingsReceived { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if(String.IsNullOrEmpty(UserID) || !Guid.TryParse(UserID, out Guid userID))
                return new RedirectToPageResult("/Index");

            var user = await _userManager.Users.FirstOrDefaultAsync(w => w.Id == UserID);
            if (user == null)
                return new RedirectToPageResult("/Index");

            Username = user.UserName;
            Showpieces = await _context.Showpiece.AsNoTracking().Where(w => w.UserID == userID).ToListAsync();

            var averageRating = await _context
                                      .ShowpieceRating
                                      .AsNoTracking()
                                      .Join( _context.Showpiece,
                                             rate=>rate.ShowpieceID,
                                             sp=>sp.ShowpieceID,(rate,sp) => new { rate.Rating, sp.UserID })
                                      .Where(w=>w.UserID == userID)
                                      .AverageAsync(a => (decimal?)a.Rating);

            AverageRating = averageRating ?? decimal.Zero;

            RatingsGiven = await _context
                                    .ShowpieceRating
                                    .AsNoTracking()
                                    .Join(_context.Showpiece,
                                            rate => rate.ShowpieceID,
                                            sp => sp.ShowpieceID, (rate, sp) => new 
                                            { 
                                                rate.Rating, 
                                                UserShowpiece = sp.UserID, 
                                                UserRating = rate.UserID 
                                            })
                                    .Where(w => w.UserShowpiece != userID && w.UserRating == userID)
                                    .LongCountAsync();

            RatingsReceived = await _context
                                    .ShowpieceRating
                                    .AsNoTracking()
                                    .Join(_context.Showpiece,
                                            rate => rate.ShowpieceID,
                                            sp => sp.ShowpieceID, (rate, sp) => new
                                            {
                                                rate.Rating,
                                                UserShowpiece = sp.UserID,
                                                UserRating = rate.UserID
                                            })
                                    .Where(w => w.UserShowpiece == userID && w.UserRating != userID)
                                    .LongCountAsync();
            return Page();
        }
    }
}
