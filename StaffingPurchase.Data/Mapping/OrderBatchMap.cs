using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Data.Mapping
{
    public class OrderBatchMap : EntityTypeConfiguration<OrderBatch>
    {
        public OrderBatchMap()
        {
            // Table
            this.ToTable("OrderBatch");

            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.ActionComment)
                .HasMaxLength(500);


            // Relationships
            this.HasRequired(t => t.Location)
                .WithMany(t => t.OrderBatches)
                .HasForeignKey(d => d.LocationId);

            this.HasOptional(t => t.HrAdminApprover)
                .WithMany(t => t.BatchesApprovedByHrAdmin)
                .HasForeignKey(d => d.HrAdminApproverId);

            this.HasOptional(t => t.HrManagerApprover)
                .WithMany(t => t.BatchesApprovedByHrManager)
                .HasForeignKey(d => d.HrManagerApproverId);
        }
    }
}

