using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Data
{
	public class FastFoodDbContext : DbContext
	{
		public FastFoodDbContext()
		{
		}

		public FastFoodDbContext(DbContextOptions options)
			: base(options)
		{
		}

        public DbSet<Category> Categories { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Position> Positions { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder builder)
		{
			if (!builder.IsConfigured)
			{
				builder.UseSqlServer(Configuration.ConnectionString);
			}
		}

		protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Employee>()
                .HasMany(x => x.Orders)
                .WithOne(x => x.Employee);

            model.Entity<Position>()
                .HasMany(x => x.Employees)
                .WithOne(x => x.Position);
            model.Entity<Position>()
                .HasIndex(x => x.Name).IsUnique();

            model.Entity<Category>()
                .HasMany(x => x.Items)
                .WithOne(x => x.Category);

            model.Entity<Item>()
                .HasIndex(x => x.Name).IsUnique();
            model.Entity<Item>()
                .HasMany(x => x.OrderItems)
                .WithOne(x => x.Item);

            model.Entity<Order>()
                .HasMany(x => x.OrderItems)
                .WithOne(x => x.Order);

            model.Entity<OrderItem>()
                .HasKey(x => new {x.OrderId, x.ItemId});
        }
	}
}