using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using mtgdm.Data;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace mtgdm.Pages.User.Report
{
    public class AddModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        public AddModel(ApplicationDbContext context,
                        IConfiguration config,
                        UserManager<IdentityUser> userManager,
                        IEmailSender emailSender)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        [BindProperty]
        public string Username { get; set; }
        

        [BindProperty]
        public mtgdm.Data.Showpiece Showpiece { get; set; }

        [BindProperty]
        public mtgdm.Data.Report Report { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if(string.IsNullOrEmpty(Name))
                return new RedirectToPageResult("/Showpiece/List");

            Showpiece = await _context.Showpiece.FirstOrDefaultAsync(f => f.Slug == Name);
            if(Showpiece == null)
                return new RedirectToPageResult("/Showpiece/List");

            var user = await _userManager.FindByIdAsync(Showpiece.UserID.ToString());
            Username = user.UserName;


            return Page();

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            Showpiece = await _context.Showpiece.FirstOrDefaultAsync(f => f.ShowpieceID == Showpiece.ShowpieceID);
            if (Showpiece == null)
                return new RedirectToPageResult("/Showpiece/List");

            var reportedBy = await _userManager.GetUserAsync(User);

            Report.ReportID = Guid.NewGuid();
            Report.Source = "Showpiece";
            Report.SourceID = Showpiece.ShowpieceID;
            Report.ReportedBy = Guid.Parse(reportedBy.Id);
            Report.Reported = DateTime.Now;
            Report.Resolved = new DateTime(9999, 12, 31);
            Report.ResolvedBy = Guid.Empty;

            await _context.Report.AddAsync(Report);
            await _context.SaveChangesAsync();
            return new RedirectToPageResult("/Showpiece/View", new { name = Showpiece.Slug });
        }
    }
}
