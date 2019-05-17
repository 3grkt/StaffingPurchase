using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Data;
using StaffingPurchase.Services.Configurations;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.PV;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using StaffingPurchase.Services.Logging;

namespace StaffingPurchase.Services.Orders
{
    public class OrderService : ServiceBase, IOrderService
    {
        private readonly IRepository<OrderBatch> _batchRepository;
        private readonly IRepository<OrderDetail> _orderDetailRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<PurchaseLitmit> _purchaseLimitRepository;
        private readonly IPvLogService _pvLogService;
        private readonly IResourceManager _resourceManager;
        private readonly IRepository<User> _userRepository;
        private readonly IDbContext _dbContext;
        private readonly ILogger _logger;

        public OrderService(
            IAppSettings appSettings,
            IAppPolicy appPolicy,
            IResourceManager resourceManager,
            IRepository<Order> orderRepository,
            IRepository<OrderDetail> orderDetailRepository,
            IRepository<User> userRepository,
            IRepository<Product> productRepository,
            IRepository<OrderBatch> batchRepository,
            IRepository<PurchaseLitmit> purchaseLimitRepository,
            IPvLogService pvLogService,
            IDbContext dbContext, ILogger logger) : base(appSettings, appPolicy)
        {
            _resourceManager = resourceManager;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _batchRepository = batchRepository;
            _purchaseLimitRepository = purchaseLimitRepository;
            _pvLogService = pvLogService;
            _dbContext = dbContext;
            _logger = logger;
        }

        public void AddOrderDetail(OrderDetail orderDetail, int userId, OrderType type)
        {
            var user = _userRepository.TableNoTracking.FirstOrDefault(c => c.Id == userId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(orderDetail));
            }

            if (IsLocked(DateTime.Now))
            {
                throw new StaffingPurchaseException(_resourceManager.GetString("Order.Validation.IsLocked"));
            }

            if (!IsEligibleUser(user))
            {
                throw new StaffingPurchaseException(_resourceManager.GetString("Order.Validation.NotEligibleUser"));
            }

            if (GetOrder(user.Id, new[] { OrderStatus.Approved, OrderStatus.Packaged, OrderStatus.Submitted }, type) != null)
            {
                throw new StaffingPurchaseException(_resourceManager.GetString("Order.Validation.OrderInSessionExist"));
            }

            using (var transaction = _dbContext.BeginDbTransaction())
            {
                try
                {
                    // If there is no order existing, create new order and add into it this order detail
                    var order = GetOrder(user.Id, new[] { OrderStatus.Draft }, type);
                    if (order != null)
                    {
                        AddOrderDetailExistOrder(orderDetail, user, order);
                    }
                    else
                    {
                        AddOrderDetailNoOrder(orderDetail, type, user);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

        }

        /// <summary>
        /// Deducts user's PV based on total value of given order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="inTransaction">The flag indicating if this is called in another transaction or note.</param>
        public void DeductUserPvForOrder(Order order, bool inTransaction = false)
        {
            //var order = _orderRepository.GetById(orderId);
            var deductedUser = _userRepository.GetById(order.UserId);
            deductedUser.CurrentPV -= (double)order.Value;
            _userRepository.Update(deductedUser, !inTransaction);

            _pvLogService.Log(
                deductedUser.Id,
                deductedUser.UserName,
                -(double)order.Value, // use negative number
                _resourceManager.GetString("PVLog.Description.PayOrderPV"),
                DateTime.Now,
                PvLogType.Ordering,
                deductedUser.CurrentPV,
                inTransaction
            );
        }

        public IDictionary<int, string> AllOrderStatus
        {
            get
            {
                IDictionary<int, string> orderStatusLst = new Dictionary<int, string>();
                foreach (OrderStatus orderStatus in Enum.GetValues(typeof(OrderStatus)))
                {
                    orderStatusLst.Add((int)orderStatus, orderStatus.ToString());
                }

                return orderStatusLst;
            }
        }

        public Order GetById(int id, bool includeOrderDetails = false, bool includeProducts = false, bool includeMetadata = false)
        {
            var query = _orderRepository.TableNoTracking;

            if (includeOrderDetails)
            {
                query = query.IncludeTable(x => x.OrderDetails);
            }

            if (includeProducts)
            {
                query = query.IncludeTable(x => x.OrderDetails.Select(x2 => x2.Product));
            }

            if (includeMetadata)
            {
                query = query
                    .IncludeTable(x => x.OrderBatch)
                    .IncludeTable(x => x.User)
                    .IncludeTable(x => x.Location)
                    .IncludeTable(x => x.Department);
            }

            query = query.Where(x => x.Id == id);
            var order = query.FirstOrDefault();

            // Calculating OrderBatch date automatically if Order hasn't been included into any OrderBatch
            if (order != null && order.OrderBatch == null)
            {
                order.OrderBatch = new OrderBatch
                {
                    StartDate = base.GetCurrentOrderSessionStartDate(order.OrderDate),
                    EndDate = base.GetCurrentOrderSessionEndDate(order.OrderDate)
                };
            }
            return order;
        }

        public IList<Order> GetOrderInNearestOrderSessionDate(int userId, OrderStatus[] status)
        {
            var query = FilterByNearestSessionDate(_orderRepository.TableNoTracking.Where(c => c.UserId == userId && !status.Any(k => (int)k == c.StatusId)));
            return query.IncludeTable(c => c.OrderDetails).ToList();
        }

        public Order GetOrder(int userId, OrderStatus status, OrderType type, bool inNearestBatch = true)
        {
            return GetOrder(userId, new[] { status }, type, inNearestBatch);
        }

        public Order GetOrder(int userId, OrderStatus[] status, OrderType type, bool inNearestBatch = true)
        {
            var query = _orderRepository
                .TableNoTracking
                .Where(c => c.UserId == userId && status.Contains((OrderStatus)c.StatusId) && c.TypeId == (int)type);
            if (inNearestBatch)
            {
                var currentSessionStartDate = GetCurrentOrderSessionStartDate().Date;
                var currentSessionEndDate = GetCurrentOrderSessionEndDate().Date;
                query = query.Where(c => DbFunctions.TruncateTime(c.OrderDate) >= currentSessionStartDate && DbFunctions.TruncateTime(c.OrderDate) <= currentSessionEndDate);
            }
            return query.IncludeTable(c => c.OrderDetails).FirstOrDefault();
        }

        public IEnumerable<OrderDetail> GetOrderDetails(int orderId)
        {
            return _orderDetailRepository.TableNoTracking.Where(c => c.OrderId == orderId).IncludeTable(c => c.Product).AsEnumerable();
        }

        public void UpdateMissingAreaOrders()
        {
            try
            {
                var orders = _orderRepository
                    .Table
                    .IncludeTable(c => c.OrderBatch)
                    .IncludeTable(c => c.User)
                    .Where(c => !c.LocationId.HasValue && !c.DepartmentId.HasValue)
                    .Where(c => !c.PackageLogId.HasValue);

                _logger.Info("Starting update missing orders' area");
                _logger.Info("Number of order: " + orders.Count());

                foreach (var order in orders)
                {
                    if (order.BatchId.HasValue)
                    {
                        order.LocationId = order.OrderBatch.LocationId;
                    }
                    else
                    {
                        order.LocationId = order.User.LocationId;
                    }

                    order.DepartmentId = order.User.DepartmentId;
                }

                _orderRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurs when updating missing orders' area", ex);
                throw;
            }

            _logger.Info("Updating missing orders' area completedly");
        }

        public IPagedList<Order> GetOrders(PaginationOptions pagingOptions, int userId, DateTime startDate, DateTime endDate, OrderStatus? status)
        {
            var query = _orderRepository
                .TableNoTracking
                .IncludeTable(c => c.OrderBatch)
                .Where(c => c.UserId == userId && (!status.HasValue || status.Value == (OrderStatus)c.StatusId))
                .Where(c => DbFunctions.TruncateTime(c.OrderDate) >= startDate.Date && DbFunctions.TruncateTime(c.OrderDate) <= endDate.Date)
                .ToList()
                .OrderBy(c => c.Id);
            foreach (var order in query)
            {
                if (order.OrderBatch == null)
                {
                    order.OrderBatch = new OrderBatch
                    {
                        StartDate = GetCurrentOrderSessionStartDate(order.OrderDate),
                        EndDate = GetCurrentOrderSessionEndDate(order.OrderDate)
                    };
                }
            }
            return new PagedList<Order>(query.AsQueryable(), pagingOptions.PageIndex, pagingOptions.PageSize);
        }

        public bool IsEligibleUser(int userId)
        {
            var user = _userRepository.GetById(userId);
            var userEndDate = user.EndDate;
            return IsEligibleUser(userEndDate, DateTime.Now);
        }

        public bool IsEligibleUser(User user)
        {
            return IsEligibleUser(user.EndDate);
        }

        public bool IsEligibleUser(DateTime? userEndDate)
        {
            return IsEligibleUser(userEndDate, DateTime.Now);
        }

        public bool IsEligibleUser(DateTime? userEndDate, DateTime currentDate)
        {
            if (userEndDate.HasValue == false)
            {
                return true;
            }

            // if current order batch is found, use this order batch's end date
            // if not, calculate order batch's end date manually
            //var batch = _batchRepository
            //    .TableNoTracking
            //    .Where(c => DateTime.Compare(currentDate, c.StartDate) >= 0 && DateTime.Compare(currentDate, c.EndDate) <= 0)
            //    .Select(c => new { c.EndDate })
            //    .FirstOrDefault();

            var batchEndDate = GetCurrentOrderSessionEndDate();

            return IsUserEndDateEligible(userEndDate.Value, batchEndDate);
        }

        public bool IsUserEndDateEligible(DateTime userEndDate, DateTime batchEndDate)
        {
            if (userEndDate.Year < batchEndDate.Year)
            {
                return false;
            }

            if (userEndDate.Month < batchEndDate.Month && userEndDate.Year == batchEndDate.Year)
            {
                return false;
            }

            // Count number of working days in odd month.
            DateTime countWorking = GetEligibleWorkingDate(batchEndDate);

            return userEndDate.Day > countWorking.Day || userEndDate.Month != countWorking.Month;
        }

        private static DateTime GetEligibleWorkingDate(DateTime batchEndDate)
        {
            int workingDay = 1;
            DateTime countWorking = new DateTime(batchEndDate.Year, batchEndDate.Month, 1);
            while (workingDay < 3)
            {
                if (countWorking.DayOfWeek >= DayOfWeek.Monday && countWorking.DayOfWeek <= DayOfWeek.Friday)
                {
                    workingDay++;
                }

                countWorking = countWorking.AddDays(1);
            }

            return countWorking;
        }

        /// <summary>
        /// Check whether or not user has enough PV to update current order details
        /// Used in case of order details are updated or deleted with no new product
        /// </summary>
        /// <param name="updateDetails">The updated order details from order with PV mode</param>
        /// <param name="userId"></param>
        /// <param name="removeDetails">The deleted order details from order with PV mode</param>
        /// <returns></returns>
        public bool IsEnoughPV(IEnumerable<OrderDetail> updateDetails, int userId, IEnumerable<OrderDetail> removeDetails)
        {
            int updateCount = updateDetails.Count();
            int removeCount = removeDetails.Count();
            if (updateCount == 0 && removeCount == 0)
            {
                throw new ArgumentNullException(nameof(updateDetails));
            }

            var user = _userRepository.GetById(userId);
            if (user == null) throw new ArgumentNullException(nameof(user));
            int orderId = updateCount > 0 ? updateDetails.ElementAt(0).OrderId : removeDetails.ElementAt(0).OrderId;

            var order = _orderRepository.TableNoTracking.Where(c => c.Id == orderId)
                .Select(c => new { c.Value })
                .FirstOrDefault();

            decimal remainPV = (decimal)user.CurrentPV;
            remainPV -= order.Value;
            remainPV += (decimal)TotalPVOfRemovedDetails(removeDetails.ToList());
            remainPV += (decimal)TotalPVOfUpdateDetails(updateDetails.ToList());
            return remainPV >= 0;
        }

        public bool IsEnoughPV(int productId, int volume, int userId)
        {
            var user = _userRepository.GetById(userId);
            var order = GetOrder(userId, OrderStatus.Draft, OrderType.PV);
            var product = _productRepository.GetById(productId);
            var currentUserPv = user.CurrentPV;
            var batchStartDate = GetCurrentOrderSessionStartDate().Date;
            var batchEndDate = GetCurrentOrderSessionEndDate().Date;
            var existOrderDetail = _orderDetailRepository
                .TableNoTracking
                .IncludeTable(c => c.Order)
                .Where(c => c.Order.UserId == userId && c.Order.TypeId == (int)OrderType.PV && c.ProductId == productId && c.Order.StatusId == (int)OrderStatus.Draft)
                .Where(c => DbFunctions.TruncateTime(c.Order.OrderDate) >= batchStartDate && DbFunctions.TruncateTime(c.Order.OrderDate) <= batchEndDate)
                .Select(c => new { c.Volume })
                .FirstOrDefault();

            decimal remainPv = (decimal)currentUserPv;
            remainPv -= order?.Value ?? 0;
            if (existOrderDetail != null)
            {
                remainPv += (decimal)(existOrderDetail.Volume * product.PV.GetValueOrDefault(0));
            }
            remainPv -= (decimal)(product.PV.GetValueOrDefault(0) * volume);

            return remainPv >= 0;
        }

        public bool IsLocked(DateTime currentDate)
        {
            if (currentDate.Month % 2 == 0)
            {
                return false;
            }

            if (currentDate.Month % 2 == 1 && (currentDate.Day < _appPolicy.OrderSessionEndDayOfMonth || currentDate.Day >= _appPolicy.OrderSessionStartDayOfMonth))
            {
                return false;
            }

            return true;
        }

        public bool IsOrderSubmitted(int userId, OrderType orderType)
        {
            return _orderRepository
                    .TableNoTracking
                    .Any(c => c.UserId == userId && c.TypeId == (int)orderType && c.StatusId == (int)OrderStatus.Submitted);
        }

        /// <summary>
        /// Recalculate value of the order
        /// </summary>
        /// <param name="targetOrder">Just need to create an empty order with specific order Id and user Id</param>
        private void RecalculateOrderValue(Order targetOrder)
        {
            if (targetOrder == null || targetOrder.Id <= 0 || targetOrder.UserId <= 0)
            {
                throw new ArgumentNullException(nameof(targetOrder));
            }

            var order = _orderRepository
                .Table
                .Where(c => c.Id == targetOrder.Id)
                .IncludeTable(c => c.OrderDetails)
                .FirstOrDefault();
            if (order != null)
            {
                order.Value = 0;
                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = _productRepository.GetById(orderDetail.ProductId);
                    if ((OrderType)order.TypeId == OrderType.Cash)
                    {
                        decimal productPrice = product.Price.GetValueOrDefault(0);
                        order.Value += orderDetail.Volume * productPrice * (1 - OrderSetting.Discount);
                    }
                    else
                    {
                        var productPv = product.PV.GetValueOrDefault(0);
                        order.Value += orderDetail.Volume * (decimal)productPv;
                    }
                }

                _orderRepository.Update(order);
            }
        }

        public void RemoveOrder(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (IsLocked(DateTime.Now))
            {
                throw new StaffingPurchaseException(_resourceManager.GetString("Order.Validation.IsLocked"));
            }

            using (var transaction = _dbContext.BeginDbTransaction())
            {
                try
                {
                    var orderDetails = _orderDetailRepository.
                    TableNoTracking.
                    Where(c => c.OrderId == order.Id).
                    Select(c => c.Id).
                    ToArray();
                    foreach (var id in orderDetails)
                    {
                        this.RemoveOrderDetail(id);
                    }

                    var dbOrder = _orderRepository.GetById(order.Id);
                    if (dbOrder != null)
                    {
                        _orderRepository.Delete(dbOrder);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new StaffingPurchaseException($"Error when removing order {order.Id}");
                }
            }

        }

        private void RemoveOrderDetail(int orderDetailId)
        {
            var orderDetail = _orderDetailRepository
                                .Table
                                .Where(c => c.Id == orderDetailId)
                                .IncludeTable(c => c.Product)
                                .IncludeTable("Order.User")
                                .FirstOrDefault();
            if (orderDetail == null)
            {
                throw new ArgumentNullException(nameof(orderDetailId));
            }

            if (orderDetail.Product.Price > _appPolicy.HighValueProductPrice)
            {
                UpdatePurchaseLimit(orderDetail.Order, new OrderDetail
                {
                    ProductId = orderDetail.ProductId,
                    Volume = 0,
                    OrderId = orderDetail.OrderId
                }, orderDetail, orderDetail.Order.User);
            }

            var order = orderDetail.Order;
            _orderDetailRepository.Delete(orderDetail);
            RecalculateOrderValue(order);
        }

        public void SubmitOrder(int userId, OrderType type)
        {
            if (IsLocked(DateTime.Now))
            {
                throw new StaffingPurchaseException(_resourceManager.GetString("Order.Validation.IsLocked"));
            }

            using (var transaction = _dbContext.BeginDbTransaction())
            {
                try
                {
                    var order = GetOrder(userId, OrderStatus.Draft, type);
                    if (order != null)
                    {
                        order = _orderRepository.GetById(order.Id);
                        order.StatusId = (int)OrderStatus.Submitted;
                        _orderRepository.Update(order);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

        }

        public void UpdateOrder(Order order)
        {
            if (!_userRepository.TableNoTracking.Any(c => c.Id == order.UserId))
            {
                throw new ArgumentNullException(nameof(order));
            }

            using (var transaction = _dbContext.BeginDbTransaction())
            {
                try
                {
                    List<OrderDetail> deleteLst, updateLst;
                    FilterUpdatedOrderChanges(order, out deleteLst, out updateLst);

                    if (order.TypeId == (int)OrderType.PV && updateLst.Count > 0)
                    {
                        if (!IsEnoughPV(updateLst, order.UserId, deleteLst))
                        {
                            throw new StaffingPurchaseException(_resourceManager.GetString("OrderUpdate.Validation.NotEnoughPV"));
                        }
                    }

                    // Regain PV to user if the order was submitted
                    if (order.StatusId >= (short)OrderStatus.Submitted && order.TypeId == (int)OrderType.PV)
                    {
                        UpdateUserPV(updateLst, order.UserId, deleteLst);
                    }

                    RemoveOrderDetails(deleteLst);
                    UpdateOrderDetails(updateLst);

                    // After updating order detail's volume, the order's value have to be recalculated
                    if (updateLst.Count > 0)
                    {
                        RecalculateOrderValue(order);
                    }

                    // Removing order if no order details are remaining.
                    var dbOrder = _orderRepository.Table.Where(c => c.Id == order.Id)
                        .IncludeTable(c => c.OrderDetails)
                        .FirstOrDefault();
                    var remainingOrderDetails = dbOrder?.OrderDetails;
                    if (remainingOrderDetails?.Count == 0)
                    {
                        _orderRepository.Delete(dbOrder);
                    }


                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }

        }

        private void FilterUpdatedOrderChanges(Order order, out List<OrderDetail> deleteLst, out List<OrderDetail> updateLst)
        {
            var oldOrderDetailList = _orderDetailRepository
                .TableNoTracking
                .IncludeTable(c => c.Order)
                .IncludeTable(c => c.Product)
                .Where(c => c.Order.Id == order.Id && c.Order.TypeId == order.TypeId)
                .Select(c => new
                {
                    c.Id,
                    c.Product,
                    c.Volume,
                    c.ProductId
                })
                .ToList();
            deleteLst = new List<OrderDetail>();
            updateLst = new List<OrderDetail>();
            foreach (var oldOrderDetail in oldOrderDetailList)
            {
                var newOrderDetail = order.OrderDetails.FirstOrDefault(c => c.Id == oldOrderDetail.Id);
                if (newOrderDetail == null)
                {
                    deleteLst.Add(
                        new OrderDetail
                        {
                            Id = oldOrderDetail.Id,
                            ProductId = oldOrderDetail.ProductId,
                            Volume = oldOrderDetail.Volume,
                            Product = oldOrderDetail.Product,
                            OrderId = order.Id
                        });
                }
                else
                {
                    if (oldOrderDetail.Volume == newOrderDetail.Volume)
                    {
                        continue;
                    }

                    if (oldOrderDetail.Product.Price >= _appPolicy.HighValueProductPrice &&
                        IsOutOfPriceLimit(newOrderDetail, order.UserId))
                    {
                        throw new StaffingPurchaseException(
                            string.Format(_resourceManager.GetString("OrderUpdate.Validation.ExistOverPrice"),
                                _appPolicy.HighValueProductLimit,
                                _appPolicy.HighValueProductPrice.ToCurrencyString()));
                    }

                    updateLst.Add(new OrderDetail
                    {
                        Id = newOrderDetail.Id,
                        ProductId = oldOrderDetail.ProductId,
                        Volume = newOrderDetail.Volume,
                        Product = oldOrderDetail.Product,
                        OrderId = order.Id
                    });
                }
            }
        }

        private void UpdateOrderDetails(List<OrderDetail> updateLst)
        {
            foreach (OrderDetail t in updateLst)
            {
                UpdateOrderDetail(t);
            }
        }

        private void RemoveOrderDetails(List<OrderDetail> deleteLst)
        {
            foreach (OrderDetail t in deleteLst)
            {
                // this method recalculate order and deletes order automatically if it meets the requirement
                RemoveOrderDetail(t.Id);
            }
        }

        public void UpdateOrderDetail(OrderDetail orderDetail)
        {
            if (orderDetail.Product == null)
            {
                throw new ArgumentNullException(nameof(orderDetail));
            }

            var dbOrderDetail = _orderDetailRepository.GetById(orderDetail.Id);
            if (dbOrderDetail != null)
            {
                if (orderDetail.Product.Price > _appPolicy.HighValueProductPrice)
                {
                    // This order detail has already existed
                    // We have to update the user's purchase limit base on that
                    var order = _orderRepository.TableNoTracking.FirstOrDefault(c => c.Id == dbOrderDetail.OrderId);
                    UpdatePurchaseLimit(order, orderDetail, existDetail: dbOrderDetail);
                }

                dbOrderDetail.Volume = orderDetail.Volume;
                _orderDetailRepository.Update(dbOrderDetail);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        private Order AddOrderDetailExistOrder(OrderDetail orderDetail, User user, Order order)
        {
            var product = _productRepository.GetById(orderDetail.ProductId);
            decimal productPrice = product.Price.GetValueOrDefault(0);
            order = _orderRepository.Table.Where(c => c.Id == order.Id).IncludeTable(c => c.OrderDetails).FirstOrDefault();

            var existDetail = order.OrderDetails.FirstOrDefault(c => c.ProductId == orderDetail.ProductId);
            if (existDetail != null)
            {
                if (productPrice > _appPolicy.HighValueProductPrice)
                {
                    UpdatePurchaseLimit(order, orderDetail, existDetail, user);
                }

                existDetail.Volume = orderDetail.Volume;
            }
            else
            {
                if (productPrice > _appPolicy.HighValueProductPrice)
                {
                    UpdatePurchaseLimit(order, orderDetail, user: user);
                }
                order.OrderDetails.Add(orderDetail);
            }

            _orderRepository.Update(order);
            RecalculateOrderValue(new Order { Id = order.Id, UserId = order.UserId });
            return order;
        }

        private Order AddOrderDetailNoOrder(OrderDetail orderDetail, OrderType type, User user)
        {


            var order = new Order
            {
                OrderDate = DateTime.Now,
                StatusId = (int)OrderStatusType.Draft,
                UserId = user.Id,
                TypeId = (int)type,
                Value = 0,
                DepartmentId = user.DepartmentId,
                LocationId = user.LocationId
            };
            order.OrderDetails.Add(orderDetail);
            _orderRepository.Insert(order);

            var product = _productRepository.GetById(orderDetail.ProductId);
            decimal productPrice = product.Price.GetValueOrDefault(0);

            if (productPrice > _appPolicy.HighValueProductPrice)
            {
                UpdatePurchaseLimit(order, orderDetail, user: user);
            }

            RecalculateOrderValue(new Order { Id = order.Id, UserId = order.UserId });
            return order;
        }

        private IQueryable<Order> FilterByNearestSessionDate(IQueryable<Order> query)
        {
            var nearestSessionStartDate = GetNearestOrderSessionStartDate();
            var nearestSessionEndDate = GetNearestOrderSessionEndDate();
            return query.Where(c => c.OrderDate >= nearestSessionStartDate && c.OrderDate <= nearestSessionEndDate);
        }

        private bool IsOutOfPriceLimit(OrderDetail newOrderDetail, int userId)
        {
            var userLimit = _purchaseLimitRepository
                                .TableNoTracking
                                .Where(c => c.UserId == userId && c.Year == DateTime.Now.Year)
                                .Select(c => new { c.BoughtCount })
                                .FirstOrDefault();
            if (userLimit == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            int total = userLimit.BoughtCount;
            var oldOrderDetail = _orderDetailRepository
                                    .TableNoTracking
                                    .Where(c => c.OrderId == newOrderDetail.OrderId && c.ProductId == newOrderDetail.ProductId)
                                    .Select(c => new { c.Volume })
                                    .FirstOrDefault();

            if (oldOrderDetail == null)
            {
                throw new ArgumentNullException(nameof(newOrderDetail));
            }

            total += newOrderDetail.Volume - oldOrderDetail.Volume;
            return total > _appPolicy.HighValueProductLimit;
        }

        /// <summary>
        /// Update purchase limit of user when order detail contains over price product has volume is changed
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderDetail">Order detail with over price product</param>
        /// <param name="existDetail">If order detail exists, the purchase limit will be updated based on that</param>
        /// <param name="user"></param>
        private void UpdatePurchaseLimit(Order order, OrderDetail orderDetail, OrderDetail existDetail = null, User user = null)
        {
            if (existDetail == null)
            {
                existDetail = new OrderDetail
                {
                    Volume = 0
                };
            }

            if (user == null)
            {
                user = _orderRepository
                    .TableNoTracking
                    .IncludeTable(c => c.OrderDetails)
                    .Where(c => c.OrderDetails.Any(k => k.Id == orderDetail.Id))
                    .Select(c => c.User)
                    .FirstOrDefault();
            }

            var orderDate = order.OrderDate.Date;
            var userLimit = _purchaseLimitRepository
                .TableNoTracking
                .Where(c => c.UserId == user.Id && c.Year == orderDate.Date.Year)
                .Select(c => new { c.Id, c.BoughtCount })
                .FirstOrDefault();
            if (orderDetail.Volume > existDetail.Volume)
            {
                // If userLimit is null, that means there is no order details with over price before
                // We only need to take order detail's volume as total due to this is the first time in this year user orders over-price product
                int totalOver = orderDetail.Volume - existDetail.Volume + userLimit?.BoughtCount ?? orderDetail.Volume;
                if (totalOver > _appPolicy.HighValueProductLimit)
                {
                    throw new StaffingPurchaseException(
                        string.Format(_resourceManager.GetString("OrderDetail.Validation.OutOfOverPrice"), _appPolicy.HighValueProductPrice.ToCurrencyString()));
                }
                if (userLimit == null)
                {
                    var limit = new PurchaseLitmit
                    {
                        UserId = user.Id,
                        BoughtCount = (short)totalOver,
                        Year = orderDate.Year
                    };
                    _purchaseLimitRepository.Insert(limit);
                }
                else
                {
                    var limit = _purchaseLimitRepository.GetById(userLimit.Id);
                    limit.BoughtCount += (short)(orderDetail.Volume - existDetail.Volume);
                    _purchaseLimitRepository.Update(limit);
                }
            }
            else
            {
                var limit = _purchaseLimitRepository.GetById(userLimit.Id);
                limit.BoughtCount -= (short)(existDetail.Volume - orderDetail.Volume);
                _purchaseLimitRepository.Update(limit);
            }
        }

        /// <summary>
        /// Only run this method if the order has status Packaged and type PV
        /// </summary>
        /// <param name="updateLst"></param>
        /// <param name="userId"></param>
        /// <param name="removeLst"></param>
        private void UpdateUserPV(IEnumerable<OrderDetail> updateLst, int userId, IEnumerable<OrderDetail> removeLst)
        {
            var updatedDetails = updateLst as IList<OrderDetail> ?? updateLst.ToList();
            var removedDetails = removeLst as IList<OrderDetail> ?? removeLst.ToList();
            if (updatedDetails.Count == 0 && removedDetails.Count == 0)
            {
                throw new StaffingPurchaseException(_resourceManager.GetString("OrderUpdate.Validation.NoChange"));
            }

            double adjustedPV = 0;
            adjustedPV += TotalPVOfRemovedDetails(removedDetails.ToList());
            adjustedPV += TotalPVOfUpdateDetails(updatedDetails.ToList());

            // Update user's PV
            var user = _userRepository.GetById(userId);
            user.CurrentPV += adjustedPV;
            _userRepository.Update(user);

            // Log PV changes
            _pvLogService.Log(
                userId,
                user.UserName,
                adjustedPV,
                _resourceManager.GetString("PVLog.Description.UpdatedByAdmin"),
                DateTime.Now,
                PvLogType.Ordering,
                user.CurrentPV);
        }

        public double TotalPVOfRemovedDetails(IList<OrderDetail> removeLst)
        {
            var rmDetails = _productRepository
                     .TableNoTracking
                     .AsEnumerable()
                     .Join(removeLst, p => p.Id, o => o.ProductId, (p, o) => new { p.PV, o.Volume });

            return rmDetails.Sum(detail => detail.Volume * detail.PV.GetValueOrDefault(0));
        }

        public double TotalPVOfUpdateDetails(IList<OrderDetail> updateLst)
        {
            double total = 0;
            foreach (var orderDetail in updateLst)
            {
                var oldOrderDetail = _orderDetailRepository
                    .TableNoTracking
                    .IncludeTable(c => c.Product)
                    .Where(c => c.Id == orderDetail.Id)
                    .Select(c => new { c.Volume, c.Product.PV })
                    .FirstOrDefault();
                total += oldOrderDetail.PV.GetValueOrDefault(0) * oldOrderDetail.Volume;
                total -= oldOrderDetail.PV.GetValueOrDefault(0) * orderDetail.Volume;
            }

            return total;
        }

        public void RemoveOrderDetails(int[] orderDetailIds)
        {
            using (var transaction = _dbContext.BeginDbTransaction())
            {
                try
                {
                    Order ord = null;
                    if (orderDetailIds.Length > 0)
                    {
                        var id = orderDetailIds[0];
                        ord =
                            _orderDetailRepository.TableNoTracking.IncludeTable(c => c.Order)
                                .Where(c => c.Id == id)
                                .Select(c => c.Order)
                                .FirstOrDefault();
                    }

                    foreach (var id in orderDetailIds)
                    {
                        this.RemoveOrderDetail(id);
                    }

                    if (ord != null && !_orderDetailRepository.TableNoTracking.Any(c => c.OrderId == ord.Id))
                    {
                        ord = _orderRepository.GetById(ord.Id);
                        _orderRepository.Delete(ord);

                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new StaffingPurchaseException("Error when removing order details");
                }
            }

        }
    }
}
