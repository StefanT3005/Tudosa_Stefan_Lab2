using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Tudosa_Stefan_Lab2.Data;
using Tudosa_Stefan_Lab2.Models;

namespace Tudosa_Stefan_Lab2.Pages.Categories
{
    [Authorize(Roles = "Admin")]

    public class IndexModel : PageModel
    {
        private readonly Tudosa_Stefan_Lab2Context _context;
        public IndexModel(Tudosa_Stefan_Lab2Context context) => _context = context;

        public BookData CategoryD { get; set; } = new();
        public int? CategoryID { get; set; }

        public async Task OnGetAsync(int? id)
        {
            var categories = await _context.Category
                .Include(c => c.BookCategories)
                    .ThenInclude(bc => bc.Book)
                        .ThenInclude(b => b.Author)
                .AsNoTracking()
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            CategoryD.Categories = categories;

            if (id.HasValue)
            {
                CategoryID = id.Value;
                var cat = categories.FirstOrDefault(c => c.ID == id.Value);

                CategoryD.Books = cat?.BookCategories?.Select(bc => bc.Book)
                                   ?? Enumerable.Empty<Book>();
            }
            else
            {
                
                CategoryD.Books = Enumerable.Empty<Book>();
            }
        }
    }
}
