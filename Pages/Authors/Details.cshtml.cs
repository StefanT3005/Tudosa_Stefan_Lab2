using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Tudosa_Stefan_Lab2.Data;
using Tudosa_Stefan_Lab2.Models;

namespace Tudosa_Stefan_Lab2.Pages.Authors
{
    public class DetailsModel : PageModel
    {
        private readonly Tudosa_Stefan_Lab2.Data.Tudosa_Stefan_Lab2Context _context;

        public DetailsModel(Tudosa_Stefan_Lab2.Data.Tudosa_Stefan_Lab2Context context)
        {
            _context = context;
        }

        public Author Author { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authors = await _context.Author.FirstOrDefaultAsync(m => m.ID == id);
            if (authors == null)
            {
                return NotFound();
            }
            else
            {
                Author = authors;
            }
            return Page();
        }
    }
}
