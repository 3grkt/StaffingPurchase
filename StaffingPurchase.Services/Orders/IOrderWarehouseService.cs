using System.Collections.Generic;
using System.Linq;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Services.Orders
{
    public interface IOrderWarehouseService
    {
        /// <summary>
        /// Get departments with existing approved orders.
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="orderStatus"></param>
        /// <param name="orderBatchStatus"></param>
        /// <returns></returns>
        IList<Department> GetDepartmentsWithOrderStatus(int locationId, OrderStatus[] orderStatus, OrderBatchStatus orderBatchStatus = OrderBatchStatus.Approved);

        /// <summary>
        /// Package orders based on selected department and warehouse staff's location.
        /// </summary>
        /// <param name="warehouseStaff"></param>
        /// <param name="departmentId"></param>
        /// <param name="orderType"></param>
        /// <param name="isDeficient"></param>
        /// <param name="note"></param>
        void PackageOrder(WorkingUser warehouseStaff, int departmentId, OrderType orderType, bool isDeficient = false, string note = "");
        /// <summary>
        /// Package all orders based on warehouse staff's location.
        /// </summary>
        /// <param name="warehouseStaff"></param>
        void PackageAllOrder(WorkingUser warehouseStaff, OrderType? orderType = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="warehouseStaff"></param>
        /// <param name="orderType"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        bool IsPackaged(WorkingUser warehouseStaff, OrderType? orderType = null, int? departmentId = null);

        void UpdateOrderAreaBasedOnPackageLog();
    }
}
