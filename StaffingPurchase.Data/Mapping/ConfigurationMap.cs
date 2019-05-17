using System.Data.Entity.ModelConfiguration;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Data.Mapping
{
    public class ConfigurationMap : EntityTypeConfiguration<Configuration>
    {
        public ConfigurationMap()
        {
            // Table
            this.ToTable("Configuration");

            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Value)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Description)
                .HasMaxLength(1000);
        }
    }
}
