using System.Collections.Generic;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Core.DTOs
{
    public class ProductImportData
    {
        public IList<Product> Products { get; set; }
        public IList<ProductCategory> Categories { get; set; }

        public ProductImportData(IList<Product> products, IList<ProductCategory> categories)
        {
            Products = products;
            Categories = categories;
        }
    }
}
