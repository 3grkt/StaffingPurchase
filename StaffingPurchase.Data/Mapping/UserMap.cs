using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Data.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            // Table
            this.ToTable("User");

            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.FullName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CostCenter)
                .HasMaxLength(50);

            this.Property(t => t.Language)
                .HasMaxLength(20);

            this.Property(t => t.EmailAddress)
                .HasMaxLength(50);

            // Relationships
            this.HasOptional(t => t.Department)
                .WithMany(t => t.Users)
                .HasForeignKey(d => d.DepartmentId);
            this.HasOptional(t => t.Level)
                .WithMany(t => t.Users)
                .HasForeignKey(d => d.LevelId);
            this.HasOptional(t => t.Location)
                .WithMany(t => t.Users)
                .HasForeignKey(d => d.LocationId);
            this.HasRequired(t => t.Role)
                .WithMany(t => t.Users)
                .HasForeignKey(d => d.RoleId);

        }
    }
}

