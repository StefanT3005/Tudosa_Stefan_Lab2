using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Tudosa_Stefan_Lab2.Data;
using Tudosa_Stefan_Lab2.Models;

namespace Tudosa_Stefan_Lab2.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly Tudosa_Stefan_Lab2Context _context;

        public IndexModel(Tudosa_Stefan_Lab2Context context)
        {
            _context = context;
        }

        public IList<Book> Book { get; set; } = new List<Book>();

        public BookData BookD { get; set; } = new();

        public int BookID { get; set; }
        public int CategoryID { get; set; }

        public string TitleSort { get; set; }
        public string AuthorSort { get; set; }

        public string CurrentFilter {  get; set; }


        public async Task OnGetAsync(int? id, int? categoryID, string sortOrder, string searchString)
        {
            TitleSort = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            AuthorSort = sortOrder == "author" ? "author_desc" : "author";

            CurrentFilter = searchString;

            Book = await _context.Book
                .Include(b => b.Publisher)
                .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
                .Include(b => b.Author)
                .AsNoTracking()
                .ToListAsync();

            BookD.Books = Book;

            if (!String.IsNullOrEmpty(searchString))
            {
                BookD.Books = BookD.Books.Where(s => s.Author.FirstName.Contains(searchString)
                                                  || s.Author.FullName.Contains(searchString)
                                                  || s.Author.LastName.Contains(searchString)
                                                  || s.Title.Contains(searchString));
            }

                if (id.HasValue)
            {
                BookID = id.Value;

                var book = BookD.Books.FirstOrDefault(i => i.ID == id.Value);
                if (book != null)
                {
                    BookD.Categories = book.BookCategories
                        .Select(s => s.Category!)
                        .ToList();
                }
            }

            switch (sortOrder)
            {
                case "title_desc":
                    BookD.Books = BookD.Books.OrderByDescending(s =>
                   s.Title);
                    break;
                case "author_desc":
                    BookD.Books = BookD.Books.OrderByDescending(s =>
                   s.Author.FullName);
                    break;
                case "author":
                    BookD.Books = BookD.Books.OrderBy(s =>
                   s.Author.FullName);
                    break;
                default:
                    BookD.Books = BookD.Books.OrderBy(s => s.Title);
                    break;

            }


            if (categoryID.HasValue)
            {
                CategoryID = categoryID.Value;
            }
        }
    }
}
