using Microsoft.EntityFrameworkCore;

namespace Packt.Shared {
    public class Northwind : DbContext {
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Product>? Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Category>()
                .Property(category => category.CategoryName)
                .IsRequired()
                .HasMaxLength(15);
            modelBuilder.Entity<Product>()
                .HasQueryFilter(product => !product.Discontinued);
            if (ProjectConstant.DatabaseProvider == "SQLite") {
                modelBuilder.Entity<Product>()
                    .Property(product => product.Cost)
                    .HasConversion<double>();
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseLazyLoadingProxies();
            if (ProjectConstant.DatabaseProvider == "SQLite") {
                string path = Path.Combine(Environment.CurrentDirectory, "Northwind.db");
                Console.WriteLine($"Using {path} database file.");
                optionsBuilder.UseSqlite($"Filename={path}");
            }
            else {
                string connection = "Data Source=(localdb)\\mssqllocaldb;" +
                    "Initial Catalog=Northwind;" +
                    "Integrated Security=true;" +
                    "MultipleActiveResultSets=true;";
                optionsBuilder.UseSqlServer(connection);
            }
        }
    }
}
