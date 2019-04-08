using Cinema.Data.Models;

namespace Cinema.Data
{
    using Microsoft.EntityFrameworkCore;

    public class CinemaContext : DbContext
    {
        public CinemaContext()  { }

        public CinemaContext(DbContextOptions options)
            : base(options)   { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Projection> Projections { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Movie>()
                .HasMany(x => x.Projections)
                .WithOne(x => x.Movie);

            model.Entity<Hall>()
                .HasMany(x => x.Projections)
                .WithOne(x => x.Hall);
            model.Entity<Hall>()
                .HasMany(x => x.Seats)
                .WithOne(x => x.Hall);


            model.Entity<Projection>()
                .HasMany(x => x.Tickets)
                .WithOne(x => x.Projection);

            model.Entity<Customer>()
                .HasMany(x => x.Tickets)
                .WithOne(x => x.Customer);
        }
    }
}