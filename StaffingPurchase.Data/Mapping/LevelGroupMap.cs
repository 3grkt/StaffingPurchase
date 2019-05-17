using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Data.Mapping
{
    public class LevelGroupMap : EntityTypeConfiguration<LevelGroup>
    {
        public LevelGroupMap()
        {
			// Table
			this.ToTable("LevelGroup");

            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

        }
    }
}

