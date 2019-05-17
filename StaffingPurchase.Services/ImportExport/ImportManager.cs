using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.DTOs;
using StaffingPurchase.Services.Localization;

namespace StaffingPurchase.Services.ImportExport
{
    public class ImportManager : IImportManager
    {
        #region Fields
        private readonly IResourceManager _resourceManager;
        private readonly IExcelParamManager _excelParamManager;

        #endregion

        #region Ctors
        public ImportManager(IResourceManager resourceManager, IExcelParamManager excelParamManager)
        {
            _resourceManager = resourceManager;
            _excelParamManager = excelParamManager;
        }
        #endregion

        #region Services
        public ProductImportData ImportProductList(Stream stream, string sheetName)
        {
            ProductImportData importData;
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                ExcelWorksheet sheet = GetWorksheet(package.Workbook, sheetName);
                if (sheet == null)
                    throw new StaffingPurchaseException(_resourceManager.GetString("Common.SheetNotFound"));

                importData = GetProductListFromSheet(sheet);
            }
            return importData;
        }

        public IList<string> ImportAwardList(Stream stream, string sheetName)
        {
            IList<string> userList;
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                ExcelWorksheet sheet = GetWorksheet(package.Workbook, sheetName);
                if (sheet == null)
                    throw new StaffingPurchaseException(_resourceManager.GetString("Common.SheetNotFound"));

                userList = GetUserListFromSheet(sheet);
            }
            return userList;
        }
        #endregion

        #region Utility
        private ExcelWorksheet GetWorksheet(ExcelWorkbook workbook, string sheetName)
        {
            return string.IsNullOrEmpty(sheetName) ? workbook.Worksheets[1] : workbook.Worksheets[sheetName];
        }

        private ProductImportData GetProductListFromSheet(ExcelWorksheet sheet)
        {
            var productList = new List<Product>();
            var categoryDictionary = new Dictionary<string, ProductCategory>();

            var excelParams = _excelParamManager.GetExcelParams(ExcelImportTypes.ProductList);
            int colSku = Convert.ToInt32(excelParams["ColSku"]);
            int colPv = Convert.ToInt32(excelParams["ColPV"]);
            int colPrice = Convert.ToInt32(excelParams["ColPrice"]);
            int colNetWeight = Convert.ToInt32(excelParams["ColNetWeight"]);
            int colDescriptionVi = Convert.ToInt32(excelParams["ColDescriptionVi"]);
            int colDescriptionEn = Convert.ToInt32(excelParams["ColDescriptionEn"]);
            int colNote = Convert.ToInt32(excelParams["ColNote"]);
            int colIsActive = Convert.ToInt32(excelParams["ColIsActive"]);
            int startRow = Convert.ToInt32(excelParams["RowStart"]);

            string currentCategory = string.Empty;

            for (int rowIndex = startRow; ; rowIndex++)
            {
                var sku = GetCellValue<string>(sheet, rowIndex, colSku);
                var pv = GetCellValue<double>(sheet, rowIndex, colPv);
                var price = GetCellValue<decimal>(sheet, rowIndex, colPrice);
                var netWeight = GetCellValue<string>(sheet, rowIndex, colNetWeight);
                var desciptionVi = GetCellValue<string>(sheet, rowIndex, colDescriptionVi);
                var descriptionEn = GetCellValue<string>(sheet, rowIndex, colDescriptionEn);
                var note = GetCellValue<string>(sheet, rowIndex, colNote);
                var isActive = (GetCellValue<string>(sheet, rowIndex, colIsActive) ?? string.Empty).Equals("yes", StringComparison.OrdinalIgnoreCase);

                if (string.IsNullOrEmpty(sku)) // empty row, break
                {
                    break;
                }

                if (string.IsNullOrEmpty(desciptionVi)) // category row
                {
                    currentCategory = sku;
                    categoryDictionary[currentCategory] = new ProductCategory() { Name = currentCategory };
                }
                else
                {
                    productList.Add(new Product()
                    {
                        Sku = sku,
                        Name = desciptionVi,
                        NameEn = descriptionEn,
                        Description = note,
                        PV = pv,
                        Price = price,
                        NetWeight = netWeight,
                        ProductCategory = categoryDictionary[currentCategory],
                        IsActive = isActive,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    });
                }
            }

            return new ProductImportData(productList, categoryDictionary.Select(x => x.Value).ToList());
        }

        private IList<string> GetUserListFromSheet(ExcelWorksheet sheet)
        {
            var userList = new List<string>();

            var excelParams = _excelParamManager.GetExcelParams(ExcelImportTypes.AwardList);
            int colUser = Convert.ToInt32(excelParams["ColUser"]);
            int startRow = Convert.ToInt32(excelParams["RowStart"]);

            string currentCategory = string.Empty;

            for (int rowIndex = startRow; ; rowIndex++)
            {
                var username = GetCellValue<string>(sheet, rowIndex, colUser);

                if (string.IsNullOrEmpty(username)) // empty row, break
                {
                    break;
                }

                userList.Add(username);
            }
            return userList;
        }

        private T GetCellValue<T>(ExcelWorksheet sheet, int rowIndex, int colIndex)
            where T : IConvertible
        {
            try
            {
                return (T)Convert.ChangeType(sheet.Cells[rowIndex, colIndex].Value, typeof(T));
            }
            catch { }
            return default(T);
        }

        #endregion
    }
}
