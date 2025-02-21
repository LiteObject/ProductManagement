using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagement.Core.Entities;

namespace ProductManagement.Core.Configurations
{
    /// <summary>
    /// The BaseConfiguration class allows us to centralize common properties and behaviors that all domain entities will share.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.CreatedOn)
                   .IsRequired()
                   .ValueGeneratedOnAdd();

            builder.Property(e => e.LastModifiedOn)
                     .IsRequired()
                     .ValueGeneratedOnAddOrUpdate();

            builder.Property(e => e.CreatedBy)
                     .IsRequired()
                     .ValueGeneratedOnAdd();

            builder.Property(e => e.LastModifiedBy)
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate();
        }
    }
}
