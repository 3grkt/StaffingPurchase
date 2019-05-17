using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using StaffingPurchase.Core;
using StaffingPurchase.Core.SearchCriteria;

namespace StaffingPurchase.Services.Orders
{
    public interface IOrderReportService
    {
        // byte[] GetAllOrderSummary(WorkingUser warehouseStaff, FileStream template = null, OrderStatus orderStatus = OrderStatus.Packaged);
        // byte[] GetOrderSummary(WorkingUser warehouseStaff, int departmentId, FileStream template = null, OrderStatus orderStatus = OrderStatus.Packaged);
        // byte[] GetOrderReport(WorkingUser staff, FileStream template, DateTime? sessionEndDate = null, OrderStatus orderStatus = OrderStatus.Packaged, int? departmentId = default(int?), int? locationId = default(int?));

       
        //byte[] GetOrderDetailReport(WorkingUser staff, FileStream template, DateTime? sessionEndDate = null,
        //    OrderStatus orderStatus = OrderStatus.Packaged, int? departmentId = default(int?),
        //    int? locationId = default(int?));

        DataTable SearchInternalRequisitionForm(WorkingUser staff, PaginationOptions paginationOptions,
            OrderAdminSearchCriteria searchCriteria);
        DataTable SearchSummaryOrderByIndividualPV(WorkingUser staff, PaginationOptions paginationOptions, OrderAdminSearchCriteria searchCriteria);
        DataTable SearchSummaryOrderByIndividualDiscount(WorkingUser staff, PaginationOptions paginationOptions, OrderAdminSearchCriteria searchCriteria);
        DataTable SearchSummaryDiscountProduct(WorkingUser staff, PaginationOptions paginationOptions,
            OrderAdminSearchCriteria searchCriteria);
        byte[] GetInternalRequisitionFormReport(WorkingUser staff, FileStream template, OrderAdminSearchCriteria searchCriteria);
        byte[] GetSummaryDiscountProductReport(WorkingUser staff, FileStream template,
            OrderAdminSearchCriteria searchCriteria);
        byte[] GetSummaryOrderByIndividualPVReport(WorkingUser staff, FileStream template, OrderAdminSearchCriteria searchCriteria);
        byte[] GetSummaryOrderByIndividualDiscountReport(WorkingUser staff, FileStream template, OrderAdminSearchCriteria searchCriteria);

        byte[] GetWarehousePackageDiscountOrderReport(WorkingUser staff, FileStream template,
            OrderAdminSearchCriteria searchCriteria, bool isPreview = false);
        byte[] GetWarehousePackagePVOrderReport(WorkingUser staff, FileStream template,
            OrderAdminSearchCriteria searchCriteria, bool isPreview = false);
        byte[] GetWarehousePackageReport(WorkingUser staff, FileStream template, OrderAdminSearchCriteria searchCriteria);
    }
}