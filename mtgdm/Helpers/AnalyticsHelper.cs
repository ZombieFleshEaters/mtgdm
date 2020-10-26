using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using mtgdm.Data;
using Microsoft.EntityFrameworkCore;

namespace mtgdm.Helpers
{
    public class AnalyticsHelper
    {
        private readonly IConfiguration _config;
        private readonly IServiceProvider _serviceProvider;
        public AnalyticsHelper(IServiceProvider serviceProvider, IConfiguration config)
        {
            _config = config;
            _serviceProvider = serviceProvider;
        }
        public Task RefreshAnalyics()
        {

            using (IServiceScope scope = _serviceProvider.CreateScope())
            using (ApplicationDbContext ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
                var emails = ctx.Users.Select(x => x.Email).ToList();
                
                var rslt = CallAPIGetResults();

                //Do database maintenance
                var summary = new AnalyticsSummary()
                {
                    AnalyticsSummaryID = Guid.NewGuid(),
                    Created = DateTime.Now,
                    RowCount = rslt.Rows.LongCount()
                };

                var details = new List<AnalyticsDetail>();
                foreach(var row in rslt.Rows)
                {
                    details.Add(new AnalyticsDetail()
                    {
                        AnalyticsDetailID = Guid.NewGuid(),
                        AnalyticsSummaryID = summary.AnalyticsSummaryID,
                        PagePath = row.PagePath,
                        Visitors = row.Vistitors,
                        Views = row.PageViews
                    });
                }

                ctx.Database.ExecuteSqlRaw("truncate table analyticssummary");
                ctx.Database.ExecuteSqlRaw("truncate table analyticsdetail");

                ctx.AnalyticsSummary.Add(summary);
                ctx.AnalyticsDetail.AddRange(details);
                ctx.SaveChanges();

                var jjj = "Hello";

            }
            return Task.CompletedTask;
        }

        private GAResults CallAPIGetResults()
        {

            string[] scopes = new string[] { AnalyticsService.Scope.Analytics };
            var cred = GoogleCredential.FromJson(_config["Google.API.ServiceAccount.Key"]).CreateScoped(scopes);

            var service = new Google.Apis.Analytics.v3.AnalyticsService(new BaseClientService.Initializer
            {
                ApplicationName = "mtgdm",
                HttpClientInitializer = cred
            });

            var request = service.Data.Ga.Get("ga:231311528",
                                              new DateTime(2020, 10, 01).ToString("yyy-MM-dd"),
                                              DateTime.Now.ToString("yyy-MM-dd"),
                                              "ga:visitors,ga:uniquePageviews");

            request.Dimensions = "ga:pagePath";
            var result = request.Execute();

            var results = new GAResults()
            {
                Columns = new List<string>(),
                Rows = new List<GAPageViewResults>()
            };

            foreach (var column in result.ColumnHeaders)
            {
                results.Columns.Add(column.Name);
            }

            foreach (var row in result.Rows)
            {
                results.Rows.Add(new GAPageViewResults()
                {
                    PagePath = row[0],
                    Vistitors = long.Parse(row[1]),
                    PageViews = long.Parse(row[2])
                });
            }

            return results;
        }

    }



    public class GAResults
    {
        public List<string> Columns { get; set; }
        public List<GAPageViewResults> Rows { get; set; }
    }

    public class GAPageViewResults
    {
        public string PagePath { get; set; }
        public long Vistitors { get; set; }
        public long PageViews { get; set; }
    }
}
