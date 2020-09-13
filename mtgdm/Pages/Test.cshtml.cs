using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using mtgdm.Data;

namespace mtgdm.Pages
{
    [AllowAnonymous]
    public class TestModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public TestModel(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public async Task<IActionResult> OnPostAsync()
        {

            var x = new Test()
            {
                TestID = Guid.NewGuid()
            };

            _context.Test.Add(x);
            await _context.SaveChangesAsync();

            return Page();
        }
    }
}
