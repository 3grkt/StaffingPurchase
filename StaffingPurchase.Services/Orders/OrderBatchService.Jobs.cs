using System;
using System.Collections.Generic;
using System.Linq;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Data;

namespace StaffingPurchase.Services.Orders
{
    /// <summary>
    /// Contains services support batchjob.
    /// </summary>
    public partial class OrderBatchService
    {
        #region Services

        public void InitOrderBatches()
        {
            var sessionEndDate = GetNearestSessionEndDate(DateTime.Now);
            if (DateTime.Now.StartOfDate() != sessionEndDate)
            {
                return;
            }

            try
            {
                var cachedBatches = new Dictionary<string, OrderBatch>(); // caches order batches based on locationId & orderTypeId
                var sessionStartDate = GetSessionStartDateBasedOnEndDate(sessionEndDate);
                var pvDeductedOrders = new List<Order>();

                var processedOrders = _orderRepository.Table
                    .IncludeTable(x => x.Location)
                    .IncludeTable(x => x.User)
                    .Where(x => x.BatchId == null).ToList(); // get non batch-allocated orders

                _logger.Info(string.Format("Found {0} order(s).", processedOrders.Count));
                _logger.Debug("Start prcessing orders");
                foreach (var order in processedOrders)
                {
                    if (order.LocationId.HasValue)
                    {
                        _logger.Info(string.Format("Processing order #{0} at location \"{1}\".", order.Id, order.Location.Name));

                        int locationId = order.LocationId.Value;
                        short orderTypeId = (short)order.TypeId;
                        string batchKey = GetBatchKey(locationId, orderTypeId);
                        if (!cachedBatches.ContainsKey(batchKey))
                        {
                            cachedBatches[batchKey] = new OrderBatch
                            {
                                LocationId = locationId,
                                StatusId = (short)OrderBatchStatus.HrAdminPending,
                                TypeId = orderTypeId,
                                StartDate = sessionStartDate,
                                EndDate = sessionEndDate,
                                ActionDate = DateTime.Now
                            };
                        }

                        // Update order status & queue to PV deducted list
                        if (order.StatusId == (short)OrderStatus.Draft)
                        {
                            if (order.TypeId == (int)OrderType.PV)
                            {
                                pvDeductedOrders.Add(order);
                            }
                            order.StatusId = (short)OrderStatus.Submitted;
                        }

                        // Allocate to batch
                        order.OrderBatch = cachedBatches[batchKey];
                        _logger.Info(string.Format("Order #{0} was allocated to new batch", order.Id));
                    }
                    else
                    {
                        _logger.Warn(string.Format("Order {0} is not allocated to any location.", order.Id));
                    }
                }

                // Deduct PV
                _logger.Debug(string.Format("Start deducting PV from {0} order(s).", pvDeductedOrders.Count));
                foreach (var order in pvDeductedOrders)
                {
                    _orderService.DeductUserPvForOrder(order, true);
                }

                // commit transaction
                _logger.Debug("Start committing transaction.");
                _orderRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to init order batches.", ex);
            }
        }

        #endregion Services

        #region Utility

        private string GetBatchKey(int locationId, short orderTypeId)
        {
            return $"{locationId}|{orderTypeId}";
        }

        #endregion Utility
    }
}
