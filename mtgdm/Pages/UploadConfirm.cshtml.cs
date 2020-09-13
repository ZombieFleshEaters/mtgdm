using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace mtdgm.Pages
{
    public class UploadConfirmModel : PageModel
    {

        [BindProperty]
        public string Source { get; set; }

        public void OnGet(string source)
        {
            Source = "https://s3-ap-southeast-2.amazonaws.com/img.mtgdm/content/6bbe6083-3d75-4ee5-8a6b-84834fd00fa4.png";
            if (!string.IsNullOrEmpty(source))
            {
                Source = source;
            }
        }
    }
}
