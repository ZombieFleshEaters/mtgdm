using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using mtgdm.Data;
using Microsoft.AspNetCore.Identity;

namespace mtgdm.ViewComponents
{
    public class ShowpieceListViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        public ShowpieceListViewComponent(ApplicationDbContext context,
                                          IConfiguration config,
                                          UserManager<IdentityUser> userManager)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string UserID, string SearchTerm, string Sort, string ReturnURL)
        {
            //https://stackoverflow.com/questions/41822458/linq-group-by-on-multiple-tables-with-nested-group-by-with-join-and-aggregate
            var query = from show in _context.Showpiece
                        join user in _context.Users on show.UserID.ToString() equals user.Id
                        join rate in _context.ShowpieceRating on show.ShowpieceID equals rate.ShowpieceID into rt
                        from def in rt.DefaultIfEmpty()
                        group def by new { user.UserName, show.ShowpieceID, show.Title, show.URL, show.UserID, show.Synopsis, show.Published, show.Created } into g
                        select new
                        {
                            g.Key.UserName,
                            g.Key.ShowpieceID,
                            g.Key.Synopsis,
                            g.Key.Title,
                            g.Key.URL,
                            g.Key.Published,
                            g.Key.UserID,
                            g.Key.Created,
                            Rating = g.Average(a => a.Rating)
                        };

            var userAuthenticated = await _userManager.GetUserAsync(Request.HttpContext.User);
            if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(UserID) || userAuthenticated.Id != UserID)
            {
                query = query.Where(w => w.Published);
            }

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(w => w.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                                   || w.Synopsis.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
            }



            if(!string.IsNullOrEmpty(UserID) && Guid.TryParse(UserID, out Guid userID))
            {
                query = query.Where(w => w.UserID == userID);
            }

            if (!string.IsNullOrEmpty(Sort))
            {
                switch (Sort)
                {
                    case "CreatedAsc":
                        query = query.OrderBy(o => o.Created);
                        break;
                    case "CreatedDesc":
                        query = query.OrderByDescending(o => o.Created);
                        break;
                    case "RatedAsc":
                        query = query.OrderBy(o => o.Rating);
                        break;
                    case "RatedDesc":
                        query = query.OrderByDescending(o => o.Rating);
                        break;
                    default:
                        break;
                }
            }

            var showpieces = await query.ToListAsync();

            var showpiecesWithUsers = showpieces.Select(s => new ShowpiecesWithUser
            {
                Showpiece = new mtgdm.Data.Showpiece()
                {
                    ShowpieceID = s.ShowpieceID,
                    Published = s.Published,
                    Synopsis = s.Synopsis,
                    Title = s.Title,
                    Created = s.Created,
                    UserID = s.UserID,
                    URL = s.URL
                },
                Username = s.UserName,
                Rating = s.Rating,
                ReturnURL = ReturnURL
            }).ToList();

            return View(showpiecesWithUsers);
        }

        public class ShowpiecesWithUser
        {
            public mtgdm.Data.Showpiece Showpiece { get; set; }

            public string Username { get; set; }

            public decimal Rating { get; set; }

            public string ReturnURL { get; set; }
        }

    }
}
