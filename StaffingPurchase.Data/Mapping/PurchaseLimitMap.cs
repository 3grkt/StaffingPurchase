using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Data.Mapping
{
    public class PurchaseLimitMap : EntityTypeConfiguration<PurchaseLitmit>
    {
        public PurchaseLimitMap()
        {
            this.ToTable("PurchaseLimit");
            this.HasKey(t => t.Id);

            this.HasRequired(t => t.User)
                .WithMany(c => c.PurchaseLimits)
                .HasForeignKey(t => t.UserId);
        }
    }
}
