using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Data.Mapping
{
    public class LevelMap : EntityTypeConfiguration<Level>
    {
        public LevelMap()
        {
			// Table
			this.ToTable("Level");

            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);


            // Relationships
            this.HasRequired(t => t.LevelGroup)
                .WithMany(t => t.Levels)
                .HasForeignKey(d => d.GroupId);

        }
    }
}

