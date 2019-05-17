using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Data.Mapping
{
    public class PermissionMap : EntityTypeConfiguration<Permission>
    {
        public PermissionMap()
        {
			// Table
			this.ToTable("Permission");

            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Description)
                .HasMaxLength(255);


            // Relationships
            this.HasMany(t => t.Roles)
                .WithMany(t => t.Permissions)
                .Map(m =>
                    {
                        m.ToTable("RolePermission");
                        m.MapLeftKey("PermissionId");
                        m.MapRightKey("RoleId");
                    });


        }
    }
}

