using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using mtgdm.Data;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using mtgdm.Services;
using mtgdm.Helpers;

namespace mtgdm
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.EnableForHttps = true;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddHostedService<HostedService>();

            services.AddDefaultIdentity<IdentityUser>(options =>
                    {
                        options.SignIn.RequireConfirmedAccount = true;
                        options.SignIn.RequireConfirmedEmail = true;
                        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ";
                        options.User.RequireUniqueEmail = true;
                    })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>();
            services.Configure<Services.EmailSettings>(options=>
                    {
                        options.ApiKey = Configuration["Mailgun.Private.APIKey"];
                        options.RequestUri = Configuration["Mailgun.Uri.Request"];
                        options.ApiBaseUri = Configuration["Mailgun.Uri.Base"];
                        options.From = Configuration["Mailgun.From"];
                    });
            services.Configure<CookiePolicyOptions>(options =>
                    {
                        options.CheckConsentNeeded = context => true;
                        options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
                    });
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddAuthentication()
                    .AddGoogle(options =>
                    {
                        options.ClientId = Configuration["Google.OAuth.ClientID"];
                        options.ClientSecret = Configuration["Google.OAuth.ClientSecret"];
                        options.AccessDeniedPath = "/AccessDenied";
                    })
                    .AddFacebook(options =>
                    {
                        options.AppId = Configuration["Facebook.OAuth.AppID"];
                        options.AppSecret = Configuration["Facebook.OAuth.AppSecret"];
                        options.AccessDeniedPath = "/AccessDenied";
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            
            //RecurringJob.AddOrUpdate(UserStoreBase, Cron.Minutely);
            app.UseMiddleware<ResponseCompressionQualityMiddleware>(new Dictionary<string, double>
            {
                { "br", 1.0 },
                { "gzip", 0.9 }
            });
            app.UseResponseCompression(); //Must come before UseStaticFiles

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24; //24 hours
                    ctx.Context.Response.Headers.Append(HeaderNames.CacheControl, $"public, max-age={durationInSeconds}");
                }
            });
            app.UseCookiePolicy();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}

public class ResponseCompressionQualityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDictionary<string, double> _encodingQuality;
    public ResponseCompressionQualityMiddleware(RequestDelegate next, IDictionary<string, double> encodingQuality)
    {
        _next = next;
        _encodingQuality = encodingQuality;
    }
    public async Task Invoke(HttpContext context)
    {
        StringValues encodings = context.Request.Headers[HeaderNames.AcceptEncoding];
        IList<StringWithQualityHeaderValue> encodingsList;
        if (!StringValues.IsNullOrEmpty(encodings)
            && StringWithQualityHeaderValue.TryParseList(encodings, out encodingsList)
            && (encodingsList != null) && (encodingsList.Count > 0))
        {
            string[] encodingsWithQuality = new string[encodingsList.Count];
            for (int encodingIndex = 0; encodingIndex < encodingsList.Count; encodingIndex++)
            {
                // If there is any quality value provided don't change anything
                if (encodingsList[encodingIndex].Quality.HasValue)
                {
                    encodingsWithQuality = null;
                    break;
                }
                else
                {
                    string encodingValue = encodingsList[encodingIndex].Value.Value;
                    encodingsWithQuality[encodingIndex] = (new StringWithQualityHeaderValue(encodingValue,
                        _encodingQuality.ContainsKey(encodingValue) ? _encodingQuality[encodingValue] : 0.1)).ToString();
                }
            }
            if (encodingsWithQuality != null)
                context.Request.Headers[HeaderNames.AcceptEncoding] = new StringValues(encodingsWithQuality);
        }
        await _next(context);
    }
}