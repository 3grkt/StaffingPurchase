using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Data.Mapping
{
    public class PackageLogMap : EntityTypeConfiguration<PackageLog>
    {
        public PackageLogMap()
        {
			// Table
			this.ToTable("PackageLog");

            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.WarehouseUserName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Comment)
                .HasMaxLength(255);
            this.Property(t => t.OrderType)
                .IsRequired();

            // Relationships
            this.HasRequired(t => t.Department)
                .WithMany(t => t.PackageLogs)
                .HasForeignKey(d => d.DepartmentId);
            this.HasRequired(t => t.Location)
                .WithMany(t => t.PackageLogs)
                .HasForeignKey(d => d.LocationId);
            this.HasRequired(t => t.WarehouseUser)
                .WithMany(t => t.PackageLogs)
                .HasForeignKey(d => d.WarehouseUserId);

        }
    }
}

