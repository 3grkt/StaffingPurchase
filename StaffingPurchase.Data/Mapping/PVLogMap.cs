using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Data.Mapping
{
    public class PVLogMap : EntityTypeConfiguration<PVLog>
    {
        public PVLogMap()
        {
            // Table
            this.ToTable("PVLog");

            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Description)
                .HasMaxLength(255);

            this.Property(t => t.OrderSession)
                .HasMaxLength(8);

            // Relationships
            this.HasRequired(t => t.User)
                .WithMany(t => t.PVLogs)
                .HasForeignKey(d => d.UserId);

        }
    }
}

