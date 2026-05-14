using HRM_Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace HRM_Backend
{
    public class AppDBContext:DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
    }
}
