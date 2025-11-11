using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tudosa_Stefan_Lab2.Data;
using Tudosa_Stefan_Lab2.Models;

namespace Tudosa_Stefan_Lab2.Pages.Books
{
    [Authorize(Roles = "Admin")]

    public class EditModel : BookCategoriesPageModel
    {
        private readonly Tudosa_Stefan_Lab2.Data.Tudosa_Stefan_Lab2Context _context;

        public EditModel(Tudosa_Stefan_Lab2.Data.Tudosa_Stefan_Lab2Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book =  await _context.Book
                .Include(b => b.Publisher)
                .Include(b => b.BookCategories)
                .ThenInclude(c => c.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (book == null)
            {
                return NotFound();
            }

            PopulateAssignedCategoryData(_context, Book);

            Book = book;
            ViewData["AuthorID"] = new SelectList(_context.Set<Author>(), "ID", "LastName");

            ViewData["PublisherID"] = new SelectList(_context.Set<Publisher>(), "ID", "PublisherName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id, string[] selectedCategories)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookToUpdate = await _context.Book
                .Include(b => b.Publisher)
                .Include(b => b.BookCategories)
                    .ThenInclude( i => i.Category)
                .FirstOrDefaultAsync( s =>  s.ID == id);

            if(bookToUpdate == null)
            {
                return NotFound();
            }

            if(await TryUpdateModelAsync<Book>(
                bookToUpdate,
                "Book",
                i => i.Title, i => i.Author,
                i => i.Price, i => i.PublishingDate, i => i.PublisherID))
            {
                UpdateBookCategories(_context, selectedCategories, bookToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            UpdateBookCategories(_context, selectedCategories, bookToUpdate);
            PopulateAssignedCategoryData(_context, bookToUpdate);


            if (!ModelState.IsValid)
            {
                ViewData["AuthorID"] = new SelectList(_context.Set<Author>(), "ID", "LastName");

                ViewData["PublisherID"] = new SelectList(_context.Set<Publisher>(), "ID", "PublisherName", Book?.PublisherID);
                return Page();
            }

            _context.Attach(Book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(Book.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.ID == id);
        }
    }
}
