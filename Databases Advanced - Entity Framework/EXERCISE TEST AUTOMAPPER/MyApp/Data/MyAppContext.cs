namespace MyApp.Data
{
    using Microsoft.EntityFrameworkCore;
    using MyApp.Models;

    public class MyAppContext : DbContext
    {
        public MyAppContext(DbContextOptions options)
            :base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
