using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using TinifyAPI;
using mtgdm.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mtgdm.Areas.Identity.Pages.Showpiece
{
    [Authorize]
    public class AddModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AddModel(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [BindProperty]
        public mtgdm.Data.Showpiece Showpiece { get; set; }

        [BindProperty]
        public IFormFile FileUpload { get; set; }

        [BindProperty]
        public IEnumerable<string> Genres { get; set; }


        [BindProperty]
        public List<SelectListItem> GenreChoices { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            GenreChoices = await _context.Genre.Select(s => new SelectListItem
            {
                Value = s.GenreID.ToString(),
                Text = s.Name
            }).ToListAsync();

            return Page();

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                GenreChoices = _context.Genre.Select(s => new SelectListItem
                {
                    Value = s.GenreID.ToString(),
                    Text = s.Name
                }).ToList();
                return Page();
            }

            if (FileUpload == null)
            {
                ModelState.AddModelError("Validation.FileUpload.FileIsBlank", "Please choose a file to upload");
                GenreChoices = _context.Genre.Select(s => new SelectListItem
                {
                    Value = s.GenreID.ToString(),
                    Text = s.Name
                }).ToList();
                return Page();
            }

            //Validate the Genres
            foreach(var genre in Genres)
            {
                if(!await _context.Genre.AnyAsync(a => a.GenreID.ToString() == genre))
                {
                    ModelState.AddModelError("Validation.Genre.IsInvalid", "An unknown Genre was provided"); 
                }
            }

            var showpieceID = Guid.NewGuid();
            var fileName = showpieceID.ToString() + ".png";
            
            Showpiece.Slug = mtgdm.Helpers.Slugify.ToUrlSlug(Showpiece.Title);
            if (await _context.Showpiece.AnyAsync(a => a.Slug.Equals(Showpiece.Slug, StringComparison.OrdinalIgnoreCase) && a.ShowpieceID != Showpiece.ShowpieceID ))
            {
                ModelState.AddModelError("Validation.Title.IsUsed", $"The title '{Showpiece.Title}' has already been taken");
                return Page();
            }
                       
            try
            {
                using var input = new MemoryStream();
                FileUpload.CopyTo(input);
                input.Position = 0;

                var bucket = _config["AWS.S3.Bucket.Content"];
                var region = _config["AWS.S3.Region"];

                Tinify.Key = _config["Tinify.APIKey"];

                var source = Tinify.FromBuffer(input.ToArray());

                //var head = new Dictionary<string, string>() {
                //    { "Cache-Control", "max-age=31536000" }
                //};

                //var jsonHeader = System.Text.Json.JsonSerializer.Serialize(head);

                await source.Store(new
                {
                    service = "s3",
                    aws_access_key_id = _config["AWS.S3.AccessKeyId"],
                    aws_secret_access_key = _config["AWS.S3.SecretKey"],
                    region = region,
                    path = bucket + fileName
                });
            }
            catch (AccountException e)
            {
                // Verify your API key and account limit.
                ModelState.AddModelError("", e.Message);
                return Page();
            }
            catch (ClientException e)
            {
                switch (e.Status)
                {
                    case 415:
                        ModelState.AddModelError("", "File type is not supported");
                        return Page();
                    default:
                        ModelState.AddModelError("", e.Message);
                        return Page();
                }
            }
            catch (ServerException e)
            {
                // Temporary issue with the Tinify API.
                ModelState.AddModelError("", e.Message);
                return Page();
            }
            catch (ConnectionException e)
            {
                // A network connection error occurred.
                ModelState.AddModelError("", e.Message);
                return Page();
            }
            catch (System.Exception e)
            {
                // Something else went wrong, unrelated to the Tinify API.
                ModelState.AddModelError("", e.Message);
                return Page();
            }

            if (Showpiece.Synopsis == null)
                Showpiece.Synopsis = "";
            Showpiece.ShowpieceID = showpieceID;
            Showpiece.Created = DateTime.Now;
            Showpiece.Published = true;
            Showpiece.URL = String.Concat(_config["AWS.CloudFront.URL.Resize"], "content/", fileName);
            Showpiece.UserID = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var genres = Genres.Select(s => new ShowpieceToGenre()
            {
                ShowpieceToGenreID = Guid.NewGuid(),
                ShowpieceID = showpieceID,
                GenreID = Guid.Parse(s)
            });

            await _context.ShowpieceToGenre.AddRangeAsync(genres);
            await _context.Showpiece.AddAsync(Showpiece);
            await _context.SaveChangesAsync();

            return new RedirectToPageResult("/Showpiece/View", new { name = Showpiece.Slug });
        }
    }
}
