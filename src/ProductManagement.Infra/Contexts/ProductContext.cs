using Microsoft.EntityFrameworkCore;
using ProductManagement.Core.Entities;

namespace ProductManagement.Infra.Contexts
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.HasDefaultSchema(modelBuilder.Model.GetDefaultSchema());

            // Configure entity properties and relationships here
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                var product = await context.Set<Product>().FirstOrDefaultAsync(cancellationToken);

                // Seed the database with a default product if none exists
                if (product == null)
                {
                    context.Set<Product>().Add(Product.Create("Test Product Name", 0.99m, "Test product description"));
                    await context.SaveChangesAsync(cancellationToken);
                }

            })
            .UseSeeding((context, _) =>
            {
                var product = context.Set<Product>().FirstOrDefault();
                // Seed the database with a default product if none exists
                if (product == null)
                {
                    context.Set<Product>().Add(Product.Create("Test Product Name", 0.99m, "Test product description"));
                    context.SaveChanges();
                }
            });

            // Configure the database connection here
            base.OnConfiguring(optionsBuilder);
        }
    }
}
