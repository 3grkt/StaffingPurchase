using System;
using System.Collections.Generic;

namespace StaffingPurchase.Core.Domain
{
    public partial class Product : EntityBase
    {
        public Product()
        {
            OrderDetails = new List<OrderDetail>();
        }

        public string Sku { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public Nullable<decimal> Price { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public double? PV { get; set; }
        public string NetWeight { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
