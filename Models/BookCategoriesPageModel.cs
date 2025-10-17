using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Tudosa_Stefan_Lab2.Data;

namespace Tudosa_Stefan_Lab2.Models
{
    public class BookCategoriesPageModel : PageModel
    {
        public List<AssignedCategoryData> AssignedCategoryDataList { get; private set; } = new();

        public void PopulateAssignedCategoryData(Tudosa_Stefan_Lab2Context context, Book? book)
        {
            var allCategories = context.Category.AsNoTracking().ToList();

            if (book is null)
            {
                AssignedCategoryDataList = allCategories
                    .Select(cat => new AssignedCategoryData
                    {
                        CategoryID = cat.ID,
                        Name = cat.CategoryName,
                        Assigned = false
                    }).ToList();
                return;
            }

            context.Entry(book).Collection(b => b.BookCategories).Load();
            book.BookCategories ??= new List<BookCategory>();

            var bookCategories = new HashSet<int>(book.BookCategories.Select(c => c.CategoryID));

            AssignedCategoryDataList = allCategories
                .Select(cat => new AssignedCategoryData
                {
                    CategoryID = cat.ID,
                    Name = cat.CategoryName,
                    Assigned = bookCategories.Contains(cat.ID)
                })
                .ToList();
        }

        public void UpdateBookCategories(
            Tudosa_Stefan_Lab2Context context,
            string[]? selectedCategories,
            Book bookToUpdate)
        {
            context.Entry(bookToUpdate).Collection(b => b.BookCategories).Load();
            bookToUpdate.BookCategories ??= new List<BookCategory>();

            if (selectedCategories is null || selectedCategories.Length == 0)
            {
                foreach (var link in bookToUpdate.BookCategories.ToList())
                    context.Set<BookCategory>().Remove(link);

                bookToUpdate.BookCategories.Clear();
                return;
            }

            
            var selectedIds = new HashSet<int>(selectedCategories.Select(int.Parse));

            var currentIds = bookToUpdate.BookCategories
                .Select(bc => bc.CategoryID)
                .ToHashSet();

            
            var toAdd = selectedIds.Except(currentIds).ToList();
            if (toAdd.Count > 0)
            {
                foreach (var catId in toAdd)
                {
                    bookToUpdate.BookCategories.Add(new BookCategory
                    {
                        BookID = bookToUpdate.ID,
                        CategoryID = catId
                    });
                }
            }

            
            var toRemove = currentIds.Except(selectedIds).ToList();
            if (toRemove.Count > 0)
            {
                var linksToRemove = bookToUpdate.BookCategories
                    .Where(bc => toRemove.Contains(bc.CategoryID))
                    .ToList();

                foreach (var link in linksToRemove)
                {
                    bookToUpdate.BookCategories.Remove(link);
                    context.Set<BookCategory>().Remove(link);
                }
            }
        }
    }
}
