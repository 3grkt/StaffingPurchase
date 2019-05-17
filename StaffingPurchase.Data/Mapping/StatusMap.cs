using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Data.Mapping
{
    public class StatusMap : EntityTypeConfiguration<Status>
    {
        public StatusMap()
        {
			// Table
			this.ToTable("Status");

            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Description)
                .HasMaxLength(100);

            this.Property(t => t.StatusType)
                .IsRequired()
                .HasMaxLength(50);

        }
    }
}

