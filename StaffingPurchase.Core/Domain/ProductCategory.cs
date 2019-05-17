using System;
using System.Collections.Generic;

namespace StaffingPurchase.Core.Domain
{
    public partial class ProductCategory : EntityBase
    {
        public ProductCategory()
        {
            Products = new List<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
