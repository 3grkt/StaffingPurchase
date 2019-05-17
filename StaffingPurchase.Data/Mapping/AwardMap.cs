using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Data.Mapping
{
    public class AwardMap : EntityTypeConfiguration<Award>
    {
        public AwardMap()
        {
			// Table
			this.ToTable("Award");

            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);


            // Relationships
            this.HasMany(t => t.Users)
                .WithMany(t => t.Awards)
                .Map(m =>
                    {
                        m.ToTable("UserAward");
                        m.MapLeftKey("AwardId");
                        m.MapRightKey("UserId");
                    });


        }
    }
}

