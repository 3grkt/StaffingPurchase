using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.DTOs;
using StaffingPurchase.Core.SearchCriteria;
using System.Collections.Generic;

namespace StaffingPurchase.Services.Products
{
    public interface IProductService
    {
        IDictionary<int, string> GetAllProductCategories();

        IEnumerable<Product> GetAllProducts(bool activeOnly = false, int? categoryId = null);

        Product GetProductById(int productId);

        decimal GetProductPrice(int productId);

        void ImportProducts(ProductImportData importData);

        IPagedList<Product> Search(ProductListSearchCriteria searchCriteria, PaginationOptions options);

        bool IsSkuExisted(int productId, string sku);

        void Update(Product product);

        void Delete(Product product);
    }
}
