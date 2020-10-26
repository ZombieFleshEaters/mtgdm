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
using mtgdm.Services;

using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Analytics.v3;

using mtgdm.Helpers;

namespace mtgdm.Pages.Admin
{
    [Authorize(Roles="Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public IndexModel(ApplicationDbContext context,
                          IConfiguration config,
                          UserManager<IdentityUser> userManager,
                          RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public long TotalUsers { get; set; }

        [BindProperty]
        public long TotalShowpieces { get; set; }

        [BindProperty]
        public long TotalRatings { get; set; }



                     
        public async Task<IActionResult> OnGetAsync()
        {

            TotalUsers = await _context.Users.LongCountAsync();
            TotalRatings = await _context.ShowpieceRating.LongCountAsync();
            TotalShowpieces = await _context.Showpiece.LongCountAsync();

            //Google API testing

            string[] scopes = new string[] { AnalyticsService.Scope.Analytics };
            var cred = GoogleCredential.FromJson(_config["Google.API.ServiceAccount.Key"]).CreateScoped(scopes);

            var service = new Google.Apis.Analytics.v3.AnalyticsService(new BaseClientService.Initializer
            {
                ApplicationName = "mtgdm",
                HttpClientInitializer = cred
            });

            //var request = service.Data.Ga.Get("ga:231311528", 
            //                                  new DateTime(2020, 10, 01).ToString("yyy-MM-dd"), 
            //                                  DateTime.Now.ToString("yyy-MM-dd"),
            //                                  "ga:visitors,ga:uniquePageviews");

            //request.Dimensions = "ga:pagePath";
            //var result = request.Execute();

            //Results = new GAResults()
            //{
            //    Columns = new List<string>(),
            //    Rows = new List<GAPageViewResults>()
            //};

            //foreach(var column in result.ColumnHeaders)
            //{
            //    Results.Columns.Add(column.Name);
            //}

            //foreach (var row in result.Rows)
            //{
            //    Results.Rows.Add(new GAPageViewResults()
            //    {
            //        PagePath = row[0],
            //        Vistitors = long.Parse(row[1]),
            //        PageViews = long.Parse(row[2])
            //    });
            //}

            //var hghg = "hello";

            return Page();
        }
    }

}
