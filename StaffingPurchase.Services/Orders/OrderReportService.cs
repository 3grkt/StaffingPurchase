using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.DTOs.Report;
using StaffingPurchase.Core.SearchCriteria;
using StaffingPurchase.Data;
using StaffingPurchase.Services.Configurations;
using StaffingPurchase.Services.ImportExport;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;

namespace StaffingPurchase.Services.Orders
{
    public class OrderReportService : ServiceBase, IOrderReportService
    {
        private readonly IRepository<OrderDetail> _detailRepository;
        private readonly IExportManager _exportManager;
        private readonly IResourceManager _resourceManager;
        private readonly ILogger _logger;

        public OrderReportService(IExportManager exportManager,
            IAppSettings appSettings,
            IRepository<OrderDetail> detailRepository,
            IAppPolicy appPolicy, IResourceManager resourceManager, ILogger logger) : base(appSettings, appPolicy)
        {
            _exportManager = exportManager;
            _detailRepository = detailRepository;
            _resourceManager = resourceManager;
            _logger = logger;
        }

        private List<OrderDetail> GetQueriedOrderDetails(WorkingUser staff,
            OrderAdminSearchCriteria searchCriteria)
        {
            var query = GetIncludedOrderDetail();
            searchCriteria.SessionEndDate = searchCriteria.SessionEndDate ?? GetNearestOrderSessionEndDate();
            IQueryable<OrderDetail> orderDetails = null;
            if (staff.IsInRole(UserRole.HRAdmin) || staff.IsInRole(UserRole.HRManager))
            {
                orderDetails = GetFilteredReportOrderDetail(query, searchCriteria);
            }
            else if (staff.IsInRole(UserRole.Warehouse))
            {
                orderDetails = GetFilteredReportOrderDetail(query, new OrderAdminSearchCriteria
                {
                    SessionEndDate = searchCriteria.SessionEndDate,
                    DepartmentId = searchCriteria.DepartmentId,
                    LocationId = staff.LocationId,
                    OrderStatus = searchCriteria.OrderStatus ?? OrderStatus.Packaged,
                    OrderBatchStatus = searchCriteria.OrderBatchStatus,
                    OrderType = searchCriteria.OrderType
                });
            }

            return orderDetails.ToList();
        }

        private List<OrderDetail> GetReportQueriedOrderDetails(WorkingUser staff,
            OrderAdminSearchCriteria searchCriteria)
        {
            var orderDetails = GetQueriedOrderDetails(staff, searchCriteria);
            if (orderDetails == null || !orderDetails.Any())
            {
                throw new StaffingPurchaseException(_resourceManager.GetString("OrderReport.NoDataToReport"));
            }

            return orderDetails;
        }

        #region Filters

        private static IQueryable<OrderDetail> GetFilteredReportOrderDetail(IQueryable<OrderDetail> query,
            OrderAdminSearchCriteria searchCriteria)
        {
            var isEmptyUserName = string.IsNullOrWhiteSpace(searchCriteria.UserName);
            var isEmptySku = string.IsNullOrWhiteSpace(searchCriteria.Sku);
            if (searchCriteria.DepartmentId.HasValue)
            {
                query = query.Where(c => c.Order.DepartmentId == searchCriteria.DepartmentId.Value);
            }

            if (searchCriteria.LocationId.HasValue)
            {
                query = query.Where(c => c.Order.LocationId == searchCriteria.LocationId.Value);
            }

            if (searchCriteria.OrderType.HasValue)
            {
                query = query.Where(c => c.Order.TypeId == (int)searchCriteria.OrderType.Value);
            }

            if (searchCriteria.OrderBatchStatus.HasValue)
            {
                query = query.Where(c => c.Order.OrderBatch.StatusId == (int)searchCriteria.OrderBatchStatus);
            }

            if (!isEmptyUserName)
            {
                query = query.Where(c => c.Order.User.UserName == searchCriteria.UserName);
            }

            if (!isEmptySku)
            {
                query = query.Where(c => c.Product.Sku == searchCriteria.Sku);
            }

            return query.Where(c =>
                DbFunctions.TruncateTime(c.Order.OrderBatch.EndDate) ==
                DbFunctions.TruncateTime(searchCriteria.SessionEndDate.Value) &&
                c.Order.StatusId == (int)searchCriteria.OrderStatus);
        }

        #endregion

        #region Search Funcs

        public DataTable SearchInternalRequisitionForm(WorkingUser staff, PaginationOptions paginationOptions,
            OrderAdminSearchCriteria searchCriteria)
        {
            searchCriteria.OrderType = OrderType.PV;
            var orderDetails = GetQueriedOrderDetails(staff, searchCriteria);

            var tbl = GetInternalRequisitionFormReportTable(orderDetails);
            if (!string.IsNullOrWhiteSpace(paginationOptions.SortExpression))
            {
                tbl.DefaultView.Sort = paginationOptions.SortExpression;
                tbl = tbl.DefaultView.ToTable();
            }

            return tbl;
        }

        public DataTable SearchSummaryDiscountProduct(WorkingUser staff, PaginationOptions paginationOptions,
            OrderAdminSearchCriteria searchCriteria)
        {
            searchCriteria.OrderType = OrderType.Cash;
            var orderDetails = GetQueriedOrderDetails(staff, searchCriteria);

            var tbl = GetSummaryDiscountProductFormTable(orderDetails);
            if (!string.IsNullOrWhiteSpace(paginationOptions.SortExpression))
            {
                tbl.DefaultView.Sort = paginationOptions.SortExpression;
                tbl = tbl.DefaultView.ToTable();
            }

            return tbl;
        }

        public DataTable SearchSummaryOrderByIndividualPV(WorkingUser staff, PaginationOptions paginationOptions,
            OrderAdminSearchCriteria searchCriteria)
        {
            searchCriteria.OrderType = OrderType.PV;
            var orderDetails = GetQueriedOrderDetails(staff, searchCriteria);

            var tbl = GetSummaryOrderByIndividualPVReportTable(orderDetails);
            if (!string.IsNullOrWhiteSpace(paginationOptions.SortExpression))
            {
                tbl.DefaultView.Sort = paginationOptions.SortExpression;
                tbl = tbl.DefaultView.ToTable();
            }

            return tbl;
        }

        public DataTable SearchSummaryOrderByIndividualDiscount(WorkingUser staff, PaginationOptions paginationOptions,
            OrderAdminSearchCriteria searchCriteria)
        {
            searchCriteria.OrderType = OrderType.Cash;
            var orderDetails = GetQueriedOrderDetails(staff, searchCriteria);

            var tbl = GetSummaryOrderByIndividualDiscountReportTable(orderDetails);
            if (!string.IsNullOrWhiteSpace(paginationOptions.SortExpression))
            {
                tbl.DefaultView.Sort = paginationOptions.SortExpression;
                tbl = tbl.DefaultView.ToTable();
            }

            return tbl;
        }

        #endregion

        #region Export Funcs

        public byte[] GetInternalRequisitionFormReport(WorkingUser staff, FileStream template,
            OrderAdminSearchCriteria searchCriteria)
        {
            searchCriteria.OrderType = OrderType.PV;
            var orderDetails = GetReportQueriedOrderDetails(staff, searchCriteria);
            var sessionEndDate = searchCriteria.SessionEndDate.Value.Date;
            var hrManager = GetHRManager(orderDetails);
            var hrAdmin = GetHRAdmin(orderDetails);
            var warehouser = GetWarehouser(orderDetails, searchCriteria);

            var tbl = GetInternalRequisitionFormReportTable(orderDetails);
            IDictionary<string, string> additionalCells = new Dictionary<string, string>();
            additionalCells.Add("C5", hrAdmin?.Name);
            additionalCells.Add("C6", hrAdmin?.FormattedActionDate);
            additionalCells.Add("E5", hrManager?.Name);
            additionalCells.Add("E6", hrManager?.FormattedActionDate);
            additionalCells.Add("I5", warehouser?.Names);
            additionalCells.Add("I6", warehouser?.ActionDates);
            additionalCells.Add("I1", sessionEndDate.ToString("MM.yyyy") + "/PVEntitlement");

            var summaryCols = new List<string> { "Quantity", "UnitPrice", "TotalPrice", "PV", "TotalPV" };
            var summaryRowColor = "#C6E0B4";

            var rawData = _exportManager.ExportToExcelFromDataTable(tbl, additionalCells, summaryCols, summaryRowColor,
                template, "A13", staff.IsInRole(UserRole.Warehouse));

            return rawData;
        }

        public byte[] GetSummaryDiscountProductReport(WorkingUser staff, FileStream template,
            OrderAdminSearchCriteria searchCriteria)
        {
            searchCriteria.OrderType = OrderType.Cash;
            var orderDetails = GetReportQueriedOrderDetails(staff, searchCriteria);
            var sessionEndDate = searchCriteria.SessionEndDate.Value.Date;
            var hrManager = GetHRManager(orderDetails);
            var hrAdmin = GetHRAdmin(orderDetails);
            var warehouser = GetWarehouser(orderDetails, searchCriteria);

            var tbl = GetSummaryDiscountProductFormTable(orderDetails);
            IDictionary<string, string> additionalCells = new Dictionary<string, string>();
            additionalCells.Add("C5", hrAdmin?.Name);
            additionalCells.Add("C6", hrAdmin?.FormattedActionDate);
            additionalCells.Add("E5", hrManager?.Name);
            additionalCells.Add("E6", hrManager?.FormattedActionDate);
            additionalCells.Add("H5", warehouser?.Names);
            additionalCells.Add("H6", warehouser?.ActionDates);
            additionalCells.Add("I1", sessionEndDate.ToString("MM.yyyy") + "/DiscountedProduct");

            var summaryCols = new List<string>
            {
                "Quantity",
                "UnitPrice",
                "TotalPrice",
                "UnitDiscountedPrice",
                "TotalDiscountedPrice"
            };
            var summaryRowColor = "#8EA9DB";

            var rawData = _exportManager.ExportToExcelFromDataTable(tbl, additionalCells, summaryCols, summaryRowColor,
                template, "A9", staff.IsInRole(UserRole.Warehouse));

            return rawData;
        }

        public byte[] GetSummaryOrderByIndividualPVReport(WorkingUser staff, FileStream template,
            OrderAdminSearchCriteria searchCriteria)
        {
            searchCriteria.OrderType = OrderType.PV;
            var orderDetails = GetReportQueriedOrderDetails(staff, searchCriteria);
            var sessionEndDate = searchCriteria.SessionEndDate.Value.Date;
            var hrManager = GetHRManager(orderDetails);
            var hrAdmin = GetHRAdmin(orderDetails);
            var warehouser = GetWarehouser(orderDetails, searchCriteria);

            var tbl = GetSummaryOrderByIndividualPVReportTable(orderDetails);
            IDictionary<string, string> additionalCells = new Dictionary<string, string>();
            additionalCells.Add("B5", hrAdmin?.Name);
            additionalCells.Add("B6", hrAdmin?.FormattedActionDate);
            additionalCells.Add("D5", hrManager?.Name);
            additionalCells.Add("D6", hrManager?.FormattedActionDate);
            additionalCells.Add("G5", warehouser?.Names);
            additionalCells.Add("G6", warehouser?.ActionDates);
            additionalCells.Add("G2", sessionEndDate.ToString("MM.yyyy") + "/PVEntitlement");

            var summaryCols = new List<string> { "Quantity", "UnitPrice", "TotalPrice", "PV", "TotalPV" };
            var summaryRowColor = "#C6E0B4";

            var rawData = _exportManager.ExportToExcelFromDataTable(tbl, additionalCells, summaryCols, summaryRowColor,
                template, "A9", staff.IsInRole(UserRole.Warehouse));

            return rawData;
        }

        public byte[] GetSummaryOrderByIndividualDiscountReport(WorkingUser staff, FileStream template,
            OrderAdminSearchCriteria searchCriteria)
        {
            searchCriteria.OrderType = OrderType.Cash;
            var orderDetails = GetReportQueriedOrderDetails(staff, searchCriteria);
            var sessionEndDate = searchCriteria.SessionEndDate.Value.Date;
            var hrManager = GetHRManager(orderDetails);
            var hrAdmin = GetHRAdmin(orderDetails);
            var warehouser = GetWarehouser(orderDetails, searchCriteria);

            var tbl = GetSummaryOrderByIndividualDiscountReportTable(orderDetails);
            IDictionary<string, string> additionalCells = new Dictionary<string, string>();
            additionalCells.Add("B5", hrAdmin?.Name);
            additionalCells.Add("B6", hrAdmin?.FormattedActionDate);
            additionalCells.Add("D5", hrManager?.Name);
            additionalCells.Add("D6", hrManager?.FormattedActionDate);
            additionalCells.Add("G5", warehouser?.Names);
            additionalCells.Add("G6", warehouser?.ActionDates);
            additionalCells.Add("G2", sessionEndDate.ToString("MM.yyyy") + "/DiscountedProduct");

            var summaryCols = new List<string>
            {
                "Quantity",
                "UnitPrice",
                "TotalPrice",
                "UnitDiscountedPrice",
                "TotalDiscountedPrice"
            };
            var summaryRowColor = "#8EA9DB";

            var rawData = _exportManager.ExportToExcelFromDataTable(tbl, additionalCells, summaryCols, summaryRowColor,
                template, "A9", staff.IsInRole(UserRole.Warehouse));

            return rawData;
        }

        public byte[] GetWarehousePackagePVOrderReport(WorkingUser staff, FileStream template,
            OrderAdminSearchCriteria searchCriteria, bool isPreview = false)
        {
            searchCriteria.OrderType = OrderType.PV;
            var orderDetails = GetReportQueriedOrderDetails(staff, searchCriteria);
            var sessionEndDate = searchCriteria.SessionEndDate.Value.Date;
            var hrManager = GetHRManager(orderDetails);
            var hrAdmin = GetHRAdmin(orderDetails);
            var warehouser = GetWarehouser(orderDetails, searchCriteria);
            var isWarehouse = staff.IsInRole(UserRole.Warehouse);
            var internRequisitionFormDataSource = new ExcelWorkSheetDataSource
            {
                Table = GetInternalRequisitionFormReportTable(orderDetails),
                StartingCell = "A13",
                SummaryColumnHeaders = new List<string> { "Quantity", "UnitPrice", "TotalPrice", "PV", "TotalPV" },
                IsProtectedWithPassword = isWarehouse,
                SummaryCellColor = "#C6E0B4",
                AdditionalCells = !isPreview ? new Dictionary<string, string>
                {
                    {"C5", hrAdmin?.Name},
                    {"C6", hrAdmin?.FormattedActionDate},
                    {"E5", hrManager?.Name},
                    {"E6", hrManager?.FormattedActionDate},
                    {"I5", warehouser?.Names},
                    {"I6", warehouser?.ActionDates},
                    {"I1", sessionEndDate.ToString("MM.yyyy") + "/PVEntitlement"}
                } : new Dictionary<string, string>
                {
                    {"I1", sessionEndDate.ToString("MM.yyyy") + "/PVEntitlement"}
                },
                WorkSheetName = "InternalRequisitionForm"
            };

            var orderByIndividualPVDataSource = new ExcelWorkSheetDataSource
            {
                Table = GetSummaryOrderByIndividualPVReportTable(orderDetails),
                StartingCell = "A9",
                SummaryColumnHeaders = new List<string> { "Quantity", "UnitPrice", "TotalPrice", "PV", "TotalPV" },
                IsProtectedWithPassword = isWarehouse,
                SummaryCellColor = "#C6E0B4",
                AdditionalCells = !isPreview ? new Dictionary<string, string>
                {
                    {"B5", hrAdmin?.Name},
                    {"B6", hrAdmin?.FormattedActionDate},
                    {"D5", hrManager?.Name},
                    {"D6", hrManager?.FormattedActionDate},
                    {"G5", warehouser?.Names},
                    {"G6", warehouser?.ActionDates},
                    {"G2", sessionEndDate.ToString("MM.yyyy") + "/PVEntitlement"}
                } : new Dictionary<string, string>
                {
                    {"G2", sessionEndDate.ToString("MM.yyyy") + "/PVEntitlement"}
                },
                WorkSheetName = "OrderByIndividualPV"
            };

            var worksheetDataSources = new List<ExcelWorkSheetDataSource>
            {
                internRequisitionFormDataSource,
                orderByIndividualPVDataSource
            };

            var bin = _exportManager.ExportToExcel(worksheetDataSources, template);
            return bin;
        }


        public byte[] GetWarehousePackageDiscountOrderReport(WorkingUser staff, FileStream template,
            OrderAdminSearchCriteria searchCriteria, bool isPreview = false)
        {
            searchCriteria.OrderType = OrderType.Cash;
            var orderDetails = GetReportQueriedOrderDetails(staff, searchCriteria);
            var sessionEndDate = searchCriteria.SessionEndDate.Value.Date;
            var hrManager = GetHRManager(orderDetails);
            var hrAdmin = GetHRAdmin(orderDetails);
            var warehouser = GetWarehouser(orderDetails, searchCriteria);
            var isWarehouse = staff.IsInRole(UserRole.Warehouse);
            var summaryDiscountProductDataSource = new ExcelWorkSheetDataSource
            {
                Table = GetSummaryDiscountProductFormTable(orderDetails),
                StartingCell = "A9",
                SummaryColumnHeaders =
                    new List<string>
                    {
                        "Quantity",
                        "UnitPrice",
                        "TotalPrice",
                        "UnitDiscountedPrice",
                        "TotalDiscountedPrice"
                    },
                IsProtectedWithPassword = isWarehouse,
                SummaryCellColor = "#8EA9DB",
                AdditionalCells = !isPreview ? new Dictionary<string, string>
                {
                    {"C5", hrAdmin?.Name},
                    {"C6", hrAdmin?.FormattedActionDate},
                    {"E5", hrManager?.Name},
                    {"E6", hrManager?.FormattedActionDate},
                    {"H5", warehouser?.Names},
                    {"H6", warehouser?.ActionDates},
                    {"I1", sessionEndDate.ToString("MM.yyyy") + "/DiscountedProduct"}
                } : new Dictionary<string, string>
                {
                    {"I1", sessionEndDate.ToString("MM.yyyy") + "/DiscountedProduct"}
                },
                WorkSheetName = "SummaryDiscountProduct"
            };
            var orderByIndividualDiscountDataSource = new ExcelWorkSheetDataSource
            {
                Table = GetSummaryOrderByIndividualDiscountReportTable(orderDetails),
                StartingCell = "A9",
                SummaryColumnHeaders =
                    new List<string>
                    {
                        "Quantity",
                        "UnitPrice",
                        "TotalPrice",
                        "UnitDiscountedPrice",
                        "TotalDiscountedPrice"
                    },
                IsProtectedWithPassword = isWarehouse,
                SummaryCellColor = "#8EA9DB",
                AdditionalCells = !isPreview ? new Dictionary<string, string>
                {
                    {"B5", hrAdmin?.Name},
                    {"B6", hrAdmin?.FormattedActionDate},
                    {"D5", hrManager?.Name},
                    {"D6", hrManager?.FormattedActionDate},
                    {"G5", warehouser?.Names},
                    {"G6", warehouser?.ActionDates},
                    {"G2", sessionEndDate.ToString("MM.yyyy") + "/DiscountedProduct"}
                } : new Dictionary<string, string>
                {
                    {"G2", sessionEndDate.ToString("MM.yyyy") + "/DiscountedProduct"}
                },
                WorkSheetName = "OrderByIndividualDiscount"
            };
            var worksheetDataSources = new List<ExcelWorkSheetDataSource>
            {
                summaryDiscountProductDataSource,
                orderByIndividualDiscountDataSource
            };

            var bin = _exportManager.ExportToExcel(worksheetDataSources, template);
            return bin;
        }

        public byte[] GetWarehousePackageReport(WorkingUser staff, FileStream template,
            OrderAdminSearchCriteria searchCriteria)
        {
            var orderDetails = GetReportQueriedOrderDetails(staff, searchCriteria);
            var sessionStartDate = searchCriteria.SessionEndDate.Value.Date.AddMonths(-2);
            var hrManager = GetHRManager(orderDetails);
            var hrAdmin = GetHRAdmin(orderDetails);
            var isWarehouse = staff.IsInRole(UserRole.Warehouse);
            var internRequisitionFormDataSource = new ExcelWorkSheetDataSource
            {
                Table = GetInternalRequisitionFormReportTable(orderDetails),
                StartingCell = "A13",
                SummaryColumnHeaders = new List<string> { "Quantity", "UnitPrice", "TotalPrice", "PV", "TotalPV" },
                IsProtectedWithPassword = isWarehouse,
                SummaryCellColor = "#C6E0B4",
                AdditionalCells = new Dictionary<string, string>
                {
                    {"C5", hrAdmin?.Name},
                    {"E5", hrManager?.Name},
                    {"I1", sessionStartDate.ToString("MM.yyyy") + "/PVEntitlement"}
                },
                WorkSheetName = "InternalRequisitionForm"
            };
            var summaryDiscountProductDataSource = new ExcelWorkSheetDataSource
            {
                Table = GetSummaryDiscountProductFormTable(orderDetails),
                StartingCell = "A9",
                SummaryColumnHeaders =
                    new List<string>
                    {
                        "Quantity",
                        "UnitPrice",
                        "TotalPrice",
                        "UnitDiscountedPrice",
                        "TotalDiscountedPrice"
                    },
                IsProtectedWithPassword = isWarehouse,
                SummaryCellColor = "#8EA9DB",
                AdditionalCells = new Dictionary<string, string>
                {
                    {"C5", hrAdmin?.Name},
                    {"E5", hrManager?.Name},
                    {"I1", sessionStartDate.ToString("MM.yyyy") + "/DiscountedProduct"}
                },
                WorkSheetName = "SummaryDiscountProduct"
            };
            var orderByIndividualPVDataSource = new ExcelWorkSheetDataSource
            {
                Table = GetSummaryOrderByIndividualPVReportTable(orderDetails),
                StartingCell = "A9",
                SummaryColumnHeaders = new List<string> { "Quantity", "UnitPrice", "TotalPrice", "PV", "TotalPV" },
                IsProtectedWithPassword = isWarehouse,
                SummaryCellColor = "#C6E0B4",
                AdditionalCells = new Dictionary<string, string>
                {
                    {"B5", hrAdmin?.Name},
                    {"D5", hrManager?.Name},
                    {"G2", sessionStartDate.ToString("MM.yyyy") + "/PVEntitlement"}
                },
                WorkSheetName = "OrderByIndividualPV"
            };

            var orderByIndividualDiscountDataSource = new ExcelWorkSheetDataSource
            {
                Table = GetSummaryOrderByIndividualDiscountReportTable(orderDetails),
                StartingCell = "A9",
                SummaryColumnHeaders =
                    new List<string>
                    {
                        "Quantity",
                        "UnitPrice",
                        "TotalPrice",
                        "UnitDiscountedPrice",
                        "TotalDiscountedPrice"
                    },
                IsProtectedWithPassword = isWarehouse,
                SummaryCellColor = "#8EA9DB",
                AdditionalCells = new Dictionary<string, string>
                {
                    {"B5", hrAdmin?.Name},
                    {"D5", hrManager?.Name},
                    {"G2", sessionStartDate.ToString("MM.yyyy") + "/DiscountedProduct"}
                },
                WorkSheetName = "OrderByIndividualDiscount"
            };
            var worksheetDataSources = new List<ExcelWorkSheetDataSource>
            {
                internRequisitionFormDataSource,
                orderByIndividualPVDataSource,
                summaryDiscountProductDataSource,
                orderByIndividualDiscountDataSource
            };

            var bin = _exportManager.ExportToExcel(worksheetDataSources, template);
            return bin;
        }

        #endregion

        #region Get Datatables

        private static DataTable GetInternalRequisitionFormReportTable(List<OrderDetail> orderDetails)
        {
            var tbl = new DataTable();
            var orderIndexCounter = 1;
            var userBasedDetails = orderDetails
                .Where(c => c.Order.TypeId == (int)OrderType.PV)
                .GroupBy(c => new { c.Order.LocationId, c.Order.DepartmentId, c.Product.Sku })
                .Select(c => new { c.Key, Row = c.FirstOrDefault(), SumVolume = c.Sum(k => k.Volume) })
                .Select(c => new
                {
                    UserId = c.Row.Order.User.UserName,
                    c.Key.DepartmentId,
                    DepartmentName = c.Row.Order.Department.Name,
                    c.Key.LocationId,
                    LocationName = c.Row.Order.Location.Name,
                    c.Row.Order.OrderDate,
                    SKU = c.Key.Sku,
                    SKUName = c.Row.Product.Name,
                    Quantity = c.SumVolume,
                    UnitPrice = c.Row.Product.Price ?? 0,
                    TotalPrice = (c.Row.Product.Price ?? 0) * c.SumVolume,
                    PV = c.Row.Product.PV ?? 0,
                    TotalPV = (c.Row.Product.PV ?? 0) * c.SumVolume
                })
                .OrderBy(c => c.LocationId).ThenBy(c => c.DepartmentId).ThenBy(c => c.SKU);
            tbl.Columns.Add("No", typeof(int));
            tbl.Columns.Add("SKU", typeof(string));
            tbl.Columns.Add("SKUName", typeof(string));
            tbl.Columns.Add("Quantity", typeof(int));
            tbl.Columns.Add("UnitPrice", typeof(decimal));
            tbl.Columns.Add("TotalPrice", typeof(decimal));
            tbl.Columns.Add("PV", typeof(decimal));
            tbl.Columns.Add("TotalPV", typeof(decimal));
            tbl.Columns.Add("Department", typeof(string));
            tbl.Columns.Add("Location", typeof(string));

            foreach (var detail in userBasedDetails)
            {
                tbl.Rows.Add(orderIndexCounter++, detail.SKU, detail.SKUName,
                    detail.Quantity, detail.UnitPrice, detail.TotalPrice, detail.PV, detail.TotalPV,
                    detail.DepartmentName,
                    detail.LocationName);
            }

            return tbl;
        }

        private static DataTable GetSummaryDiscountProductFormTable(List<OrderDetail> orderDetails)
        {
            var tbl = new DataTable();
            var orderIndexCounter = 1;
            var userBasedDetails = orderDetails
                .Where(c => c.Order.TypeId == (int)OrderType.Cash)
                .GroupBy(c => new { c.Order.LocationId, c.Order.DepartmentId, c.Product.Sku })
                .Select(c => new { c.Key, Row = c.FirstOrDefault(), SumVolume = c.Sum(k => k.Volume) })
                .Select(c => new
                {
                    UserId = c.Row.Order.User.UserName,
                    c.Key.DepartmentId,
                    DepartmentName = c.Row.Order.Department.Name,
                    c.Key.LocationId,
                    LocationName = c.Row.Order.Location.Name,
                    c.Row.Order.OrderDate,
                    SKU = c.Key.Sku,
                    SKUName = c.Row.Product.Name,
                    Quantity = c.SumVolume,
                    UnitPrice = c.Row.Product.Price ?? 0,
                    TotalPrice = (c.Row.Product.Price ?? 0) * c.SumVolume,
                    UnitDiscountedPrice = (c.Row.Product.Price ?? 0) * c.SumVolume * OrderSetting.Discount,
                    TotalDiscountedPrice = (c.Row.Product.Price ?? 0) * c.SumVolume * (1 - OrderSetting.Discount)
                }).OrderBy(c => c.LocationId).ThenBy(c => c.DepartmentId).ThenBy(c => c.UserId);
            tbl.Columns.Add("No", typeof(int));
            tbl.Columns.Add("SKU", typeof(string));
            tbl.Columns.Add("SKUName", typeof(string));
            tbl.Columns.Add("Quantity", typeof(int));
            tbl.Columns.Add("UnitPrice", typeof(decimal));
            tbl.Columns.Add("TotalPrice", typeof(decimal));
            tbl.Columns.Add("UnitDiscountedPrice", typeof(decimal));
            tbl.Columns.Add("TotalDiscountedPrice", typeof(decimal));
            tbl.Columns.Add("Department", typeof(string));
            tbl.Columns.Add("Location", typeof(string));

            foreach (var detail in userBasedDetails)
            {
                tbl.Rows.Add(orderIndexCounter++, detail.SKU, detail.SKUName,
                    detail.Quantity, detail.UnitPrice, detail.TotalPrice, detail.UnitDiscountedPrice,
                    detail.TotalDiscountedPrice, detail.DepartmentName,
                    detail.LocationName);
            }

            return tbl;
        }

        private DataTable GetSummaryOrderByIndividualPVReportTable(List<OrderDetail> orderDetails)
        {
            var tbl = new DataTable();
            var orderIndexCounter = 1;
            var userBasedDetails = orderDetails
                .Where(c => c.Order.TypeId == (int)OrderType.PV)
                .Select(c => new
                {
                    UserId = c.Order.User.UserName,
                    Name = c.Order.User.FullName,
                    c.Order.DepartmentId,
                    DepartmentName = c.Order.Department.Name,
                    c.Order.LocationId,
                    LocationName = c.Order.Location.Name,
                    c.Order.OrderDate,
                    SKU = c.Product.Sku,
                    SKUName = c.Product.Name,
                    Quantity = c.Volume,
                    UnitPrice = c.Product.Price ?? 0,
                    TotalPrice = (c.Product.Price ?? 0) * c.Volume,
                    PV = c.Product.PV ?? 0,
                    TotalPV = (c.Product.PV ?? 0) * c.Volume
                }).OrderBy(c => c.LocationId).ThenBy(c => c.DepartmentId).ThenBy(c => c.UserId);

            tbl.Columns.Add("No", typeof(int));
            tbl.Columns.Add("Id", typeof(string));
            tbl.Columns.Add("Name", typeof(string));
            tbl.Columns.Add("Department", typeof(string));
            tbl.Columns.Add("Location", typeof(string));
            tbl.Columns.Add("SKU", typeof(string));
            tbl.Columns.Add("SKUName", typeof(string));
            tbl.Columns.Add("Quantity", typeof(int));
            tbl.Columns.Add("UnitPrice", typeof(decimal));
            tbl.Columns.Add("TotalPrice", typeof(decimal));
            tbl.Columns.Add("PV", typeof(decimal));
            tbl.Columns.Add("TotalPV", typeof(decimal));


            foreach (var detail in userBasedDetails)
            {
                tbl.Rows.Add(orderIndexCounter++, detail.UserId, detail.Name, detail.DepartmentName,
                    detail.LocationName, detail.SKU, detail.SKUName,
                    detail.Quantity, detail.UnitPrice, detail.TotalPrice, detail.PV, detail.TotalPV);
            }

            return tbl;
        }

        private DataTable GetSummaryOrderByIndividualDiscountReportTable(List<OrderDetail> orderDetails)
        {
            var tbl = new DataTable();
            var orderIndexCounter = 1;
            var userBasedDetails = orderDetails
                .Where(c => c.Order.TypeId == (int)OrderType.Cash)
                .Select(c => new
                {
                    UserId = c.Order.User.UserName,
                    Name = c.Order.User.FullName,
                    c.Order.DepartmentId,
                    DepartmentName = c.Order.Department.Name,
                    c.Order.LocationId,
                    LocationName = c.Order.Location.Name,
                    c.Order.OrderDate,
                    SKU = c.Product.Sku,
                    SKUName = c.Product.Name,
                    Quantity = c.Volume,
                    UnitPrice = c.Product.Price ?? 0,
                    TotalPrice = (c.Product.Price ?? 0) * c.Volume,
                    DiscountedPrice = (c.Product.Price ?? 0) * c.Volume * OrderSetting.Discount,
                    TotalDiscountedPrice = (c.Product.Price ?? 0) * c.Volume * (1 - OrderSetting.Discount),
                    c.Order.User.CostCenter
                }).OrderBy(c => c.LocationId).ThenBy(c => c.DepartmentId).ThenBy(c => c.UserId);

            tbl.Columns.Add("No", typeof(int));
            tbl.Columns.Add("Id", typeof(string));
            tbl.Columns.Add("Name", typeof(string));
            tbl.Columns.Add("Department", typeof(string));
            tbl.Columns.Add("Location", typeof(string));
            tbl.Columns.Add("CostCenter", typeof(string));
            tbl.Columns.Add("SKU", typeof(string));
            tbl.Columns.Add("SKUName", typeof(string));
            tbl.Columns.Add("Quantity", typeof(int));
            tbl.Columns.Add("UnitPrice", typeof(decimal));
            tbl.Columns.Add("TotalPrice", typeof(decimal));
            tbl.Columns.Add("UnitDiscountedPrice", typeof(decimal));
            tbl.Columns.Add("TotalDiscountedPrice", typeof(decimal));


            foreach (var detail in userBasedDetails)
            {
                tbl.Rows.Add(orderIndexCounter++, detail.UserId, detail.Name, detail.DepartmentName,
                    detail.LocationName, detail.CostCenter, detail.SKU, detail.SKUName,
                    detail.Quantity, detail.UnitPrice, detail.TotalPrice, detail.DiscountedPrice,
                    detail.TotalDiscountedPrice);
            }

            return tbl;
        }

        #endregion

        #region Utils

        private IQueryable<OrderDetail> GetIncludedOrderDetail()
        {
            return _detailRepository
                .TableNoTracking
                .IncludeTable(c => c.Order)
                .IncludeTable(c=> c.Order.Department)
                .IncludeTable(c=> c.Order.Location)
                .IncludeTable(c => c.Order.User)
                .IncludeTable(c => c.Order.OrderBatch)
                .IncludeTable(c => c.Order.OrderBatch.HrAdminApprover)
                .IncludeTable(c => c.Order.OrderBatch.HrManagerApprover)
                .IncludeTable(c => c.Order.PackageLog)
                .IncludeTable(c => c.Order.PackageLog.WarehouseUser)
                .IncludeTable(c => c.Product);
        }

        private ReportUser GetHRAdmin(List<OrderDetail> orderDetails)
        {
            ReportUser hrAdmin = null;
            try
            {
                var firstOrderDetail =
                    orderDetails.OrderByDescending(c => c.Order.OrderBatch.HrAdminApprovalDate).FirstOrDefault();
                if (firstOrderDetail?.Order.OrderBatch?.HrAdminApprover != null)
                {
                    var hrAdminName = firstOrderDetail.Order.OrderBatch.HrAdminApprover.FullName;
                    hrAdmin = new ReportUser
                    {
                        Name = hrAdminName,
                        ActionDate = firstOrderDetail.Order.OrderBatch.HrAdminApprovalDate.GetValueOrDefault()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Get HR Admin will be ignored although cannot be retrieved", ex);
            }

            return hrAdmin;
        }

        private ReportUser GetHRManager(List<OrderDetail> orderDetails)
        {
            ReportUser hrManager = null;
            try
            {
                var firstOrderDetail =
                orderDetails.OrderByDescending(c => c.Order.OrderBatch.HrManagerApprovalDate).FirstOrDefault();

                if (firstOrderDetail?.Order.OrderBatch?.HrManagerApprover != null)
                {
                    var hrManagerName = firstOrderDetail.Order.OrderBatch.HrManagerApprover.FullName;
                    hrManager = new ReportUser
                    {
                        Name = hrManagerName,
                        ActionDate = firstOrderDetail.Order.OrderBatch.HrManagerApprovalDate.GetValueOrDefault()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Get HR Manager will be ignored although cannot be retrieved", ex);
            }


            return hrManager;
        }

        private ReportUsers GetWarehouser(List<OrderDetail> orderDetails, OrderAdminSearchCriteria searchCriteria)
        {
            ReportUsers warehousers = null;
            if (searchCriteria.OrderStatus.GetValueOrDefault() != OrderStatus.Packaged)
            {
                return null;
            }

            try
            {
                var packageLogs = orderDetails.GroupBy(c => c.Order.PackageLogId).Select(c =>
                    c.FirstOrDefault()?.Order.PackageLog).ToList();

                if (packageLogs.Count >= 1)
                {
                    warehousers = new ReportUsers();
                    var names = new StringBuilder();
                    var actionDates = new StringBuilder();
                    for (int i = 0; i < packageLogs.Count; i++)
                    {
                        string splitChar;
                        if (packageLogs.Count == 1 || i == packageLogs.Count - 1)
                        {
                            splitChar = "";
                        }
                        else
                        {
                            splitChar = " | ";
                        }

                        var packageLog = packageLogs[i];
                        names.Append(packageLog.WarehouseUser.FullName + splitChar);
                        actionDates.Append(packageLog.PackedDate.ToString("HH:mm dd/MM/yyyy") + splitChar);
                    }

                    warehousers.Names = names.ToString();
                    warehousers.ActionDates = actionDates.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("GetWarehouses will be ignored although warehouse cannot be retrieved", ex);
            }

            return warehousers;
        }

        #endregion

        private class ReportUser
        {
            public string Name { get; set; }
            public DateTime ActionDate { private get; set; }
            public string FormattedActionDate => ActionDate.ToString("HH:mm dd/MM/yyyy");
        }

        private class ReportUsers
        {
            public string Names { get; set; }
            public string ActionDates { get; set; }
        }
    }
}