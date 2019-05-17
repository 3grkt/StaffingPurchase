using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Data.Mapping
{
    public class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
			// Table
			this.ToTable("Product");

            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Sku)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.NameEn)
                .HasMaxLength(150);

            this.Property(t => t.Description)
                .HasMaxLength(255);

            this.Property(t => t.NetWeight)
                .HasMaxLength(50);


            // Relationships
            this.HasRequired(t => t.ProductCategory)
                .WithMany(t => t.Products)
                .HasForeignKey(d => d.CategoryId);

        }
    }
}

