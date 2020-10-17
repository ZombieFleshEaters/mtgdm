using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using mtgdm.Data;

namespace mtgdm.Areas.Identity.Pages.Showpiece
{
    [AllowAnonymous]
    public class ViewModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;

        public ViewModel(ApplicationDbContext context,
                        IConfiguration config,
                        UserManager<IdentityUser> userManager)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string ShowpieceID { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ReturnURL { get; set; }



        [BindProperty]
        public mtgdm.Data.Showpiece Showpiece { get; set; }

        [BindProperty]
        public string ImageClass { get; set; }

        [BindProperty]
        public decimal Rating { get; set; }

        [BindProperty]
        public bool RatingDisabled { get; set; }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public List<Genre> Genres { get; set; }

        [BindProperty]
        public List<CommentWithUser> Comments { get; set; }

        [BindProperty]
        public mtgdm.Data.Comment Comment { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!Guid.TryParse(ShowpieceID, out Guid showpieceID))
            {
                return new RedirectToPageResult("/Showpiece/List");
            }

            Showpiece = await _context.Showpiece.FirstOrDefaultAsync(f => f.ShowpieceID == showpieceID);
            if (Showpiece == null)
                return new RedirectToPageResult("/Showpiece/List");

            RatingDisabled = !User.Identity.IsAuthenticated;

            if (User.Identity.IsAuthenticated)
            {
                var userID = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                //Get user-specific rating
                var userRating = await _context.ShowpieceRating
                                               .AsNoTracking()
                                               .Where(w => w.UserID == userID && w.ShowpieceID == showpieceID)
                                               .AverageAsync(a => (decimal?)a.Rating);
                Rating = userRating ?? decimal.Zero;
            }

            if (Rating == decimal.Zero)
            {
                //Get average rating
                var totalRating = await _context.ShowpieceRating
                                                .AsNoTracking()
                                                .Where(w => w.ShowpieceID == showpieceID)
                                                .AverageAsync(a => (decimal?)a.Rating);
                Rating = totalRating ?? decimal.Zero;
            }

            var user = await _userManager.FindByIdAsync(Showpiece.UserID.ToString());

            Username = user?.UserName ?? "[Deleted]";

            Genres = await _context.Genre
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
                                    .Where(w => w.ShowpieceID == showpieceID)
                                    .Select(s => new Genre()
                                    {
                                        GenreID = s.GenreID,
                                        Name = s.Name,
                                        Normalized = s.Normalized
                                    }).ToListAsync();
            //https://stackoverflow.com/questions/41822458/linq-group-by-on-multiple-tables-with-nested-group-by-with-join-and-aggregate
            var query = from comment in _context.Comment
                        join usr in _context.Users on comment.UserID.ToString() equals usr.Id
                        where comment.ShowpieceID == showpieceID
                        orderby comment.Created ascending
                        select new CommentWithUser
                        {
                            Comment = comment,
                            Username = usr.UserName
                        };

            Comments = await query.ToListAsync();
            foreach(var comment in Comments)
            {
                var isAdmin = false;
                if(User.Identity.IsAuthenticated)
                {
                    isAdmin = await _userManager.IsInRoleAsync(await _userManager.GetUserAsync(User), "Admin");
                }
                var isSameUser = _userManager.GetUserId(User) == comment.Comment.UserID.ToString();
                comment.CanDelete = isAdmin || isSameUser;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostCommentAsync()
        {
            if (string.IsNullOrEmpty(Comment.Content) || Comment.Content.Length < 3)
            {
                await OnGetAsync();
                ModelState.AddModelError("Comment.Length", "Comments must be at least 3 characters");
                return Page();
            }

            if (string.IsNullOrEmpty(ShowpieceID) || !Guid.TryParse(ShowpieceID, out Guid showpieceID))
            {
                return new RedirectToPageResult("/Showpice/List");
            }

            var user = await _userManager.GetUserAsync(User);
            var userId = Guid.Parse(user.Id);

            Comment.UserID = userId;
            Comment.ShowpieceID = showpieceID;
            Comment.CommentID = Guid.NewGuid();
            Comment.Created = DateTime.Now;

            await _context.Comment.AddAsync(Comment);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Showpiece/View", new { Comment.ShowpieceID });
        }

        public async Task<IActionResult> OnPostDeleteCommentAsync()
        {
            if (Comment.CommentID == Guid.Empty)
            {
                await OnGetAsync();
                return Page();
            }

            if (string.IsNullOrEmpty(ShowpieceID) || !Guid.TryParse(ShowpieceID, out Guid showpieceID))
            {
                await OnGetAsync();
                return Page();
            }

            var comment = await _context.Comment.FirstOrDefaultAsync(f => f.CommentID == Comment.CommentID);
            if (comment == null)
            {
                await OnGetAsync();
                return Page();
            }

            var showpiece = await _context.Showpiece.FirstOrDefaultAsync(f => f.ShowpieceID == showpieceID);
            if (showpiece == null)
            {
                await OnGetAsync();
                return Page();
            }

            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Showpiece/View", new { showpiece.ShowpieceID });
        }


        public class CommentWithUser
        {
            public mtgdm.Data.Comment Comment { get; set; }
            public string Username { get; set; }
            public bool CanDelete { get; set; }
        }

    }
}
