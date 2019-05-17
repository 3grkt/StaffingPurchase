using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using System;
using System.Collections.Generic;

namespace StaffingPurchase.Services.Orders
{
    public interface IOrderService
    {
        void AddOrderDetail(OrderDetail orderDetail, int userId, OrderType type);

        /// <summary>
        /// Deducts user's PV based on total value of given order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="inTransaction">The flag indicating if this is called in another transaction or note.</param>
        void DeductUserPvForOrder(Order order, bool inTransaction = false);

        Order GetById(int id, bool includeOrderDetails = false, bool includeProducts = false, bool includeMetadata = false);

        IList<Order> GetOrderInNearestOrderSessionDate(int userId, OrderStatus[] exceptStatuses);

        Order GetOrder(int userId, OrderStatus status, OrderType type, bool inNearestBatch = true);

        Order GetOrder(int userId, OrderStatus[] status, OrderType type, bool inNearestBatch = true);

        IEnumerable<OrderDetail> GetOrderDetails(int orderId);

        bool IsEligibleUser(int userId);

        bool IsEligibleUser(User user);

        bool IsEligibleUser(DateTime? userEndDate);

        /// <summary>
        /// Check whether or not user is allowed to order based on user's end date
        /// </summary>
        /// <param name="userEndDate">User's end date to perform checking</param>
        /// <param name="currentDate">Checking date</param>
        /// <returns></returns>
        bool IsEligibleUser(DateTime? userEndDate, DateTime currentDate);

        /// <summary>
        /// Check whether or not user has enough PV to update current order details
        /// Used in case of order details are updated or deleted with no new product
        /// </summary>
        /// <param name="updateDetails">The updated order details from order with PV mode</param>
        /// <param name="userId"></param>
        /// <param name="removeDetails">The deleted order details from order with PV mode</param>
        /// <returns></returns>
        bool IsEnoughPV(IEnumerable<OrderDetail> updateDetails, int userId, IEnumerable<OrderDetail> removeDetails);

        /// <summary>
        /// Check whether or not user has enough PV to buy new product
        /// Used this function in case of adding new product into user's order
        /// </summary>
        /// <param name="productId">New product Id</param>
        /// <param name="volume">Product's quantity</param>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        bool IsEnoughPV(int productId, int volume, int userId);

        bool IsLocked(DateTime currentDate);

        bool IsOrderSubmitted(int userId, OrderType orderType);

        void RemoveOrder(Order order);
        void RemoveOrderDetails(int[] orderDetailIds);

        void SubmitOrder(int userId, OrderType type);

        void UpdateOrder(Order order);

        void UpdateOrderDetail(OrderDetail orderDetail);
        void UpdateMissingAreaOrders();

        /// <summary>
        /// Get orders with pagination.
        /// </summary>
        /// <param name="pagingOptions"></param>
        /// <param name="userId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        IPagedList<Order> GetOrders(PaginationOptions pagingOptions, int userId, DateTime startDate, DateTime endDate, OrderStatus? status);

        IDictionary<int, string> AllOrderStatus { get; }
    }
}