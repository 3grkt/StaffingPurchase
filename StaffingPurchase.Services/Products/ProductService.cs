using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.DTOs;
using StaffingPurchase.Data;
using StaffingPurchase.Services.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using StaffingPurchase.Core.SearchCriteria;

namespace StaffingPurchase.Services.Products
{
    public class ProductService : IProductService
    {
        #region Fields
        private readonly ICacheService _cacheService;
        private readonly IRepository<ProductCategory> _productCategoryRepo;
        private readonly IRepository<Product> _productRepo;
        #endregion

        #region Ctor.
        public ProductService(IRepository<Product> productRepo, IRepository<ProductCategory> productCategoryRepo, ICacheService cacheService)
        {
            _productRepo = productRepo;
            _productCategoryRepo = productCategoryRepo;
            _cacheService = cacheService;
        }

        public void Delete(Product product)
        {
            _productRepo.Delete(product);
        }
        #endregion

        #region Services
        public IDictionary<int, string> GetAllProductCategories()
        {
            var productCategories = _cacheService.Get<IDictionary<int, string>>(CacheNames.ProductCategories);
            if (productCategories == null)
            {
                productCategories = _productCategoryRepo.TableNoTracking
                    .Select(x => new { x.Id, x.Name })
                    .ToDictionary(t => t.Id, t => t.Name.Trim());
                _cacheService.Set(CacheNames.ProductCategories, productCategories);
            }
            return productCategories;
        }

        public IEnumerable<Product> GetAllProducts(bool activeOnly, int? categoryId)
        {
            var query = _productRepo.TableNoTracking;
            if (activeOnly)
            {
                query = query.Where(x => x.IsActive);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            return query.AsEnumerable();
        }

        public Product GetProductById(int productId)
        {
            return _productRepo.GetById(productId);
        }

        public decimal GetProductPrice(int productId)
        {
            var product = _productRepo.GetById(productId);
            return product.Price ?? 0;
        }

        public void ImportProducts(ProductImportData importData)
        {
            var allCategories = _productCategoryRepo.Table.ToList();

            // Import categories
            foreach (var category in importData.Categories)
            {
                if (allCategories.FindIndex(x => x.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase)) < 0)
                {
                    category.CreatedDate = category.ModifiedDate = DateTime.Now;
                    _productCategoryRepo.Insert(category, false);

                    allCategories.Add(category);
                }
            }

            // Import products
            foreach (var product in importData.Products)
            {
                var retrievedProduct = _productRepo.Table.FirstOrDefault(x => x.Sku == product.Sku);
                if (retrievedProduct != null)
                {
                    CopyProductProperties(product, retrievedProduct);
                    retrievedProduct.ProductCategory = LookupCategory(allCategories, product.ProductCategory.Name);
                    retrievedProduct.ModifiedDate = DateTime.Now;
                    _productRepo.Update(retrievedProduct, false);
                }
                else
                {
                    product.CreatedDate = product.ModifiedDate = DateTime.Now;
                    product.ProductCategory = LookupCategory(allCategories, product.ProductCategory.Name);
                    _productRepo.Insert(product, false);
                }
            }

            _productRepo.SaveChanges(); // commit transactions
        }

        public bool IsSkuExisted(int productId, string sku)
        {
            return _productRepo.TableNoTracking.Any(x => x.Id != productId && x.Sku == sku);
        }

        public IPagedList<Product> Search(ProductListSearchCriteria searchCriteria, PaginationOptions options)
        {
            var query = _productRepo.TableNoTracking.IncludeTable(x => x.ProductCategory);

            // Filter
            if (!string.IsNullOrEmpty(searchCriteria.Sku))
            {
                query = query.Where(x => x.Sku.Contains(searchCriteria.Sku));
            }
            if (!string.IsNullOrEmpty(searchCriteria.Name))
            {
                query = query.Where(x => x.Name.Contains(searchCriteria.Name));
            }
            if (searchCriteria.CategoryId > 0)
            {
                query = query.Where(x => x.CategoryId == searchCriteria.CategoryId);
            }
            if (searchCriteria.ActiveOnly)
            {
                query = query.Where(x => x.IsActive);
            }

            // Sort
            if (string.IsNullOrEmpty(options.Sort))
            {
                query = query.OrderBy(x => x.Sku);
            }
            else
            {
                // HACK: support nested sort
                if (options.Sort.Equals("categoryName", StringComparison.OrdinalIgnoreCase))
                {
                    options.Sort = "ProductCategory.Name";
                }

                query = query.SortBy(options.SortExpression);
            }

            return new PagedList<Product>(query, options.PageIndex, options.PageSize);
        }

        public void Update(Product product)
        {
            product.ModifiedDate = DateTime.Now;
            _productRepo.Update(product);
        }
        #endregion

        #region Utility
        private void CopyProductProperties(Product source, Product target)
        {
            target.Name = source.Name;
            target.NameEn = source.NameEn;
            target.Description = source.Description;
            target.PV = source.PV;
            target.Price = source.Price;
            target.NetWeight = source.NetWeight;
            target.IsActive = source.IsActive;
        }

        private ProductCategory LookupCategory(IList<ProductCategory> allCategories, string categoryName)
        {
            return allCategories
                .Where(x => x.Name.Equals(categoryName))
                .FirstOrDefault();
        }
        #endregion
    }
}
