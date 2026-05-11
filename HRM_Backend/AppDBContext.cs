using HRM_Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace HRM_Backend
{
    public class AppDBContext:DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }
        public DbSet<Employee> Employee { get; set; }


    }
}
