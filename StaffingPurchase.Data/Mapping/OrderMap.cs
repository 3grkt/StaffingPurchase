using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Data.Mapping
{
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
			// Table
			this.ToTable("Order");

            // Primary Key
            this.HasKey(t => t.Id);

            // Properties

            // Relationships
            this.HasRequired(t => t.User)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.UserId);
            this.HasOptional(t => t.OrderBatch)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.BatchId);
            this.HasOptional(t => t.PackageLog)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.PackageLogId);
            this.HasOptional(t => t.Department)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.DepartmentId);
            this.HasOptional(t => t.Location)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.LocationId);

        }
    }
}

