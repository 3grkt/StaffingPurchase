using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Data.Mapping
{
    public class ProductCategoryMap : EntityTypeConfiguration<ProductCategory>
    {
        public ProductCategoryMap()
        {
			// Table
			this.ToTable("ProductCategory");

            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

        }
    }
}

