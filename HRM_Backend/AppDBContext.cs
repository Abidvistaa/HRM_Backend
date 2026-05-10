using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HRM_Backend
{
    public class AppDBContext:DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }
        public DbSet<Book> Books { get; set; }
    }
}
