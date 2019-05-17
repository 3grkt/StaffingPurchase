using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using System;
using System.Collections.Generic;

namespace StaffingPurchase.Services.Orders
{
    public partial interface IOrderBatchService
    {
        /// <summary>
        /// Approves order batch.
        /// </summary>
        /// <param name="user">Current working user.</param>
        /// <param name="orderBatch">The batch to approve.</param>
        void Approve(WorkingUser user, OrderBatch orderBatch);

        /// <summary>
        /// Approves all order batches in the nearest order session.
        /// </summary>
        /// <param name="user">Current working user.</param>
        /// <param name="orderType">Type of orders contained in batch.</param>
        /// <param name="failedLocations">List of locations of failed batches.</param>
        void ApproveAll(WorkingUser user, OrderType orderType, out IList<string> failedLocations);

        OrderBatch GetByDate(DateTime date);

        OrderBatch GetById(int id);

        OrderBatch GetByLocation(int locationId, OrderType orderType, WorkingUser user);

        DateTime GetNearestSessionEndDate(DateTime? basedDate = null);
        DateTime GetNearestSessionStartDate(DateTime? basedDate = null);

        string GetNearestOrderSession(DateTime endDate);

        /// <summary>
        /// Rejects order batches.
        /// </summary>
        /// <param name="user">Current working user.</param>
        /// <param name="orderBatch"></param>
        /// <param name="rejectReason"></param>
        void Reject(WorkingUser user, OrderBatch orderBatch, string rejectReason);

        /// <summary>
        /// Reject all order batches in the nearest order session.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="orderType">Type of orders contained in batch.</param>
        /// <param name="rejectReason"></param>
        /// <param name="failedLocations"></param>
        void RejectAll(WorkingUser user, OrderType orderType, string rejectReason, out IList<string> failedLocations);

        /// <summary>
        /// Searches orders by given batch id and other info.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="departmentId"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="paginationOptions"></param>
        /// <returns></returns>
        IPagedList<Order> SearchOrders(int batchId, int departmentId, string userId, string userName, PaginationOptions paginationOptions);

        /// <summary>
        /// Gets all orders in submitted batches.
        /// </summary>
        /// <param name="sessionEndDate"></param>
        /// <param name="departmentId"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="orderType"></param>
        /// <param name="paginationOptions"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        IPagedList<Order> GetAllOrdersInSubmittedBatches(DateTime sessionEndDate, int departmentId, string userId, string userName, OrderType orderType, PaginationOptions paginationOptions, WorkingUser user);

        /// <summary>
        /// Return all Order Batch Start Date and End Date
        /// </summary>
        /// <returns>A list contains Tuple object with Item1 is StartDate and Item2 is EndDate</returns>
        IList<Tuple<DateTime, DateTime>> GetAllSessionPeriod(int? limit = 30);
    }
}
