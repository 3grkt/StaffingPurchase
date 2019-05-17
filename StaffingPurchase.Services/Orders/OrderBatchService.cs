using System;
using System.Collections.Generic;
using System.Linq;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Data;
using StaffingPurchase.Services.Configurations;
using StaffingPurchase.Services.Logging;

namespace StaffingPurchase.Services.Orders
{
    public partial class OrderBatchService : ServiceBase, IOrderBatchService
    {
        #region Fields

        private readonly IDbContext _dbContext;
        private readonly ILogger _logger;
        private readonly IRepository<OrderBatch> _orderBatchRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IOrderService _orderService;

        #endregion Fields

        #region Ctor.

        public OrderBatchService(
            IRepository<OrderBatch> orderBatchRepository,
            IRepository<Order> orderRepository,
            IOrderService orderService,
            IAppSettings appSettings,
            IAppPolicy appPolicy,
            ILogger logger,
            IDbContext dbContext)
            : base(appSettings, appPolicy)
        {
            _orderBatchRepository = orderBatchRepository;
            _orderRepository = orderRepository;
            _orderService = orderService;
            _logger = logger;
            _dbContext = dbContext;
        }

        #endregion Ctor.

        #region Services

        public void Approve(WorkingUser user, OrderBatch orderBatch)
        {
            using (var dbTransaction = _dbContext.BeginDbTransaction())
            {
                try
                {
                    ApproveBatch(user, orderBatch);

                    dbTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    _logger.WriteLog(string.Format("Failed to approve orderBatch {0}", orderBatch.Id), ex);
                }
            }
        }

        public void ApproveAll(WorkingUser user, OrderType orderType, out IList<string> failedLocations)
        {
            failedLocations = new List<string>();

            // Search batches
            var query = _orderBatchRepository.Table.IncludeTable(x => x.Location);
            query = FilterByDate(query, GetNearestOrderSessionEndDate(DateTime.Now));
            query = FilterByBatchStatus(query, user.Roles);
            query = query.Where(x => x.TypeId == (short)orderType);

            foreach (var orderBatch in query.ToList())
            {
                using (var dbTransaction = _dbContext.BeginDbTransaction())
                {
                    try
                    {
                        ApproveBatch(user, orderBatch, false);

                        dbTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        failedLocations.Add(orderBatch.Location.Name);
                        dbTransaction.Rollback();
                        _logger.WriteLog(string.Format("Failed to approve orderBatch {0}", orderBatch.Id), ex);
                    }
                }
            }
        }

        public OrderBatch GetByDate(DateTime date)
        {
            var query = _orderBatchRepository
                .TableNoTracking;

            query = FilterByDate(query, date);

            return query.FirstOrDefault();
        }

        public OrderBatch GetById(int id)
        {
            return _orderBatchRepository.Table.FirstOrDefault(x => x.Id == id);
        }

        public OrderBatch GetByLocation(int locationId, OrderType orderType, WorkingUser user)
        {
            var query = _orderBatchRepository.TableNoTracking;

            query = FilterByDate(query, GetNearestSessionEndDate(DateTime.Now));
            query = FilterByBatchStatus(query, user.Roles);
            query = query.Where(x => x.LocationId == locationId && x.TypeId == (short)orderType);

            return query.FirstOrDefault();
        }

        public void Reject(WorkingUser user, OrderBatch orderBatch, string rejectReason)
        {
            RejectBatch(user, orderBatch, rejectReason);
        }

        public void RejectAll(WorkingUser user, OrderType orderType, string rejectReason, out IList<string> failedLocations)
        {
            failedLocations = new List<string>();

            // Search batches
            var query = _orderBatchRepository.Table.IncludeTable(x => x.Location);
            query = FilterByDate(query, GetNearestOrderSessionEndDate(DateTime.Now));
            query = query.Where(x => x.StatusId == (short)OrderBatchStatus.HrManagerPending);
            query = query.Where(x => x.TypeId == (short)orderType);

            foreach (var orderBatch in query.ToList())
            {
                try
                {
                    RejectBatch(user, orderBatch, rejectReason, false);
                }
                catch (Exception ex)
                {
                    failedLocations.Add(orderBatch.Location.Name);
                    _logger.WriteLog(string.Format("Failed to reject orderBatch {0}", orderBatch.Id), ex);
                }
            }
        }

        public IPagedList<Order> SearchOrders(int batchId, int departmentId, string userId, string userName, PaginationOptions paginationOptions)
        {
            var query = GetOrderQuery();

            // Filter
            query = query.Where(x => x.BatchId == batchId);
            query = FilterByDepartmentAndUser(query, departmentId, userId, userName);

            // Sort
            if (string.IsNullOrEmpty(paginationOptions.Sort))
            {
                query = query.OrderBy(x => x.Id);
            }
            else
            {
                query = GetOrderSortQuery(query, paginationOptions);
            }

            return new PagedList<Order>(query, paginationOptions.PageIndex, paginationOptions.PageSize);
        }

        public IPagedList<Order> GetAllOrdersInSubmittedBatches(DateTime sessionEndDate, int departmentId, string userId, string userName, OrderType orderType, PaginationOptions paginationOptions, WorkingUser user)
        {
            var query = GetOrderQuery();

            // Filter
            query = query.Where(x => x.OrderBatch != null && x.OrderBatch.EndDate == sessionEndDate && x.TypeId == (short)orderType);
            query = FilterByBatchStatus(query, user.Roles);
            query = FilterByDepartmentAndUser(query, departmentId, userId, userName);

            // Sort
            if (string.IsNullOrEmpty(paginationOptions.Sort))
            {
                query = query.OrderBy(x => x.Id);
            }
            else
            {
                query = GetOrderSortQuery(query, paginationOptions);
            }

            return new PagedList<Order>(query, paginationOptions.PageIndex, paginationOptions.PageSize);
        }

        public DateTime GetNearestSessionEndDate(DateTime? basedDate = null)
        {
            return GetNearestOrderSessionEndDate(basedDate);
        }

        public DateTime GetNearestSessionStartDate(DateTime? basedDate = null)
        {
            return GetNearestOrderSessionStartDate(basedDate);
        }

        public string GetNearestOrderSession(DateTime endDate)
        {
            return string.Format("{0} - {1}",
                GetSessionStartDateBasedOnEndDate(endDate).ToString(_appSettings.DateFormat),
                endDate.ToString(_appSettings.DateFormat));
        }

        public IList<Tuple<DateTime, DateTime>> GetAllSessionPeriod(int? limit = 30)
        {
            var query =
                _orderBatchRepository.TableNoTracking.GroupBy(c => new { c.StartDate, c.EndDate })
                    .Select(c => new { c.Key.StartDate, c.Key.EndDate });

            var dates = query.AsEnumerable().Select(c => new Tuple<DateTime, DateTime>(c.StartDate, c.EndDate)).ToList();
            dates.Sort((a, b) => b.Item2.CompareTo(a.Item2));
            return limit != null ? dates.Take(limit.GetValueOrDefault()).ToList() : dates.Take(30).ToList();
        }
        #endregion Services

        #region Utility

        private void ApproveBatch(WorkingUser user, OrderBatch orderBatch, bool validateStatus = true)
        {
            var orderBatchStatus = (OrderBatchStatus)orderBatch.StatusId;

            // Validate batch status
            if (validateStatus && orderBatchStatus == OrderBatchStatus.Approved)
            {
                throw new StaffingPurchaseException(string.Format("Could not approve orderBatch {0}; statusId: {1}",
                    orderBatch.Id, orderBatch.StatusId));
            }

            // Validate permission of user
            if (((orderBatchStatus == OrderBatchStatus.HrAdminPending || orderBatchStatus == OrderBatchStatus.RejectedByHrManager)
                    && !user.Roles.Contains(UserRole.HRAdmin)) ||
                (orderBatchStatus == OrderBatchStatus.HrManagerPending && !user.Roles.Contains(UserRole.HRManager)))
            {
                throw new StaffingPurchaseException(string.Format("User {0} does not have permission to approve orderBatch {1}",
                    user.UserName, orderBatch.Id));
            }

            if (orderBatchStatus == OrderBatchStatus.HrAdminPending || orderBatchStatus == OrderBatchStatus.RejectedByHrManager)
            {
                orderBatch.StatusId = (short)OrderBatchStatus.HrManagerPending;
                orderBatch.HrAdminApproverId = user.Id;
                orderBatch.HrAdminApprovalDate = DateTime.Now;
            }
            else if (orderBatchStatus == OrderBatchStatus.HrManagerPending)
            {
                orderBatch.StatusId = (short)OrderBatchStatus.Approved;
                orderBatch.HrManagerApproverId = user.Id;
                orderBatch.HrManagerApprovalDate = DateTime.Now;
            }

            orderBatch.ActionDate = DateTime.Now;
            orderBatch.ActionComment = string.Empty;
            _orderBatchRepository.Update(orderBatch);

            // Update order
            _orderRepository.BulkUpdate(
                f => f.OrderBatch.Id == orderBatch.Id,
                u => new Order { StatusId = (short)OrderStatus.Approved });
        }

        private void RejectBatch(WorkingUser user, OrderBatch orderBatch, string rejectReason, bool validateStatus = true)
        {
            if (validateStatus)
            {
                var invalidStatuses = new[] { OrderBatchStatus.Approved, OrderBatchStatus.HrAdminPending, OrderBatchStatus.RejectedByHrManager };
                if (invalidStatuses.Contains((OrderBatchStatus)orderBatch.StatusId))
                {
                    throw new StaffingPurchaseException(string.Format("Could not reject orderBatch {0}; statusId: {1}",
                        orderBatch.Id, orderBatch.StatusId));
                }
            }

            // Validate permission of user
            if (!user.Roles.Contains(UserRole.HRManager))
            {
                throw new StaffingPurchaseException(string.Format("User {0} does not have permission to reject orderBatch {1}",
                    user.UserName, orderBatch.Id));
            }

            orderBatch.StatusId = (short)OrderBatchStatus.RejectedByHrManager;
            orderBatch.ActionDate = DateTime.Now;
            orderBatch.ActionComment = rejectReason;
            _orderBatchRepository.Update(orderBatch);
        }

        private IQueryable<Order> GetOrderQuery()
        {
            return _orderRepository.TableNoTracking
                            .IncludeTable(x => x.User)
                            .IncludeTable(x => x.Department)
                            .IncludeTable(x => x.Location);
        }

        private IQueryable<Order> GetOrderSortQuery(IQueryable<Order> query, PaginationOptions options)
        {
            if (options.Sort.Equals("UserName", StringComparison.OrdinalIgnoreCase))
            {
                options.Sort = "User.UserName";
            }
            else if (options.Sort.Equals("DepartmentName", StringComparison.OrdinalIgnoreCase))
            {
                options.Sort = "User.Department.Name";
            }
            else if (options.Sort.Equals("LocationName", StringComparison.OrdinalIgnoreCase))
            {
                options.Sort = "OrderBatch.Location.Name";
            }
            return query.SortBy(options.SortExpression);
        }

        private IQueryable<OrderBatch> FilterByDate(IQueryable<OrderBatch> query, DateTime date)
        {
            return query.Where(c => DateTime.Compare(date, c.StartDate) >= 0 && DateTime.Compare(date, c.EndDate) <= 0);
        }

        private static IQueryable<OrderBatch> FilterByBatchStatus(IQueryable<OrderBatch> query, IList<UserRole> userRoles)
        {
            if (userRoles.Contains(UserRole.HRManager))
            {
                query = query.Where(x => x.StatusId == (short)OrderBatchStatus.HrManagerPending);
            }
            else if (userRoles.Contains(UserRole.HRAdmin))
            {
                query = query.Where(x => x.StatusId == (short)OrderBatchStatus.HrAdminPending || x.StatusId == (short)OrderBatchStatus.RejectedByHrManager);
            }
            return query;
        }

        private static IQueryable<Order> FilterByBatchStatus(IQueryable<Order> query, IList<UserRole> userRoles)
        {
            if (userRoles.Contains(UserRole.HRManager))
            {
                query = query.Where(x => x.OrderBatch.StatusId == (short)OrderBatchStatus.HrManagerPending);
            }
            else if (userRoles.Contains(UserRole.HRAdmin))
            {
                query = query.Where(x => x.OrderBatch.StatusId == (short)OrderBatchStatus.HrAdminPending || x.OrderBatch.StatusId == (short)OrderBatchStatus.RejectedByHrManager);
            }
            return query;
        }

        private DateTime GetSessionStartDateBasedOnEndDate(DateTime sessionEndDate)
        {
            var date = sessionEndDate.AddMonths(-1 * _appSettings.OrderSessionDurationInMoth);
            return new DateTime(date.Year, date.Month, _appPolicy.OrderSessionStartDayOfMonth);
        }

        private IQueryable<Order> FilterByDepartmentAndUser(IQueryable<Order> query, int departmentId, string userId, string userName)
        {
            if (departmentId > 0)
            {
                query = query.Where(x => x.DepartmentId == departmentId);
            }
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(x => x.User.UserName.Contains(userId));
            }
            if (!string.IsNullOrEmpty(userName))
            {
                query = query.Where(x => x.User.FullName.Contains(userName));
            }
            return query;
        }
        #endregion Utility
    }
}
