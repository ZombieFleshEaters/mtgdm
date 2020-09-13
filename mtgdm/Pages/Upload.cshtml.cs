using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TinifyAPI;
using System.IO;
using Microsoft.Extensions.Configuration;
using mtgdm.Data;

namespace mtgdm.Pages
{
    public class UploadModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public UploadModel(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [BindProperty]
        public IFormFile FileUpload { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var fileName = Guid.NewGuid().ToString() + ".png";

            try
            {
                using var input = new MemoryStream();
                FileUpload.CopyTo(input);
                input.Position = 0;

                var bucket = _config["AWS.S3.Bucket.Content"];
                var region = _config["AWS.S3.Region"];

                Tinify.Key = _config["Tinify.APIKey"];
                var source = Tinify.FromBuffer(input.ToArray());
                
                await source.Store(new
                {
                    service = "s3",
                    aws_access_key_id = _config["AWS.S3.AccessKeyId"],
                    aws_secret_access_key = _config["AWS.S3.SecretKey"],
                    region = region,
                    path = bucket + fileName
                });

                var newUrl = String.Concat(_config["AWS.CloudFront.URL"], fileName);
                return new RedirectToPageResult("UploadConfirm", new { Source = newUrl });

            }
            catch (AccountException e)
            {
                System.Console.WriteLine("The error message is: " + e.Message);
                // Verify your API key and account limit.
            }
            catch (ClientException e)
            {
                // Check your source image and request options.
                var h = "";
            }
            catch (ServerException e)
            {
                // Temporary issue with the Tinify API.
            }
            catch (ConnectionException e)
            {
                // A network connection error occurred.
            }
            catch (System.Exception e)
            {
                // Something else went wrong, unrelated to the Tinify API.
            }


            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {


            return Page();
        }

        private void CleanupDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                foreach (var file in Directory.GetFiles(directory))
                {
                    System.IO.File.Delete(file);
                }
                Directory.Delete(directory);
            }
        }

    }
}
