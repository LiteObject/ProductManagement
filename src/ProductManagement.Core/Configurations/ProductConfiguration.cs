using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagement.Core.Entities;

namespace ProductManagement.Core.Configurations
{
    /// <summary>
    /// The ProductConfiguration class allows us to configure the schema for the Product entity.
    /// </summary>
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Price)
                   .IsRequired();

            builder.Property(p => p.Description)
                   .HasMaxLength(1000);

            builder.Property(p => p.Description);

            // Optional: Add indexes for better query performance
            builder.HasIndex(p => p.Name);
        }
    }
}
