using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tudosa_Stefan_Lab2.Models;

namespace Tudosa_Stefan_Lab2.Data
{
    public class Tudosa_Stefan_Lab2Context : DbContext
    {
        public Tudosa_Stefan_Lab2Context (DbContextOptions<Tudosa_Stefan_Lab2Context> options)
            : base(options)
        {
        }

        public DbSet<Tudosa_Stefan_Lab2.Models.Book> Book { get; set; } = default!;
        public DbSet<Tudosa_Stefan_Lab2.Models.Publisher> Publisher { get; set; } = default!;
        public DbSet<Tudosa_Stefan_Lab2.Models.Author> Author { get; set; } = default!;
    }
}
