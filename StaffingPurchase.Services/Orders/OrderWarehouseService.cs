using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Hosting;
using RazorEngine;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.DTOs;
using StaffingPurchase.Data;
using StaffingPurchase.Services.Configurations;
using StaffingPurchase.Services.Email;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;

namespace StaffingPurchase.Services.Orders
{
    public class OrderWarehouseService : ServiceBase, IOrderWarehouseService
    {
        private readonly IRepository<Location> _locationRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<PackageLog> _pkgLogRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Department> _departmentRepository;
        private readonly IDbContext _dbContext;
        private readonly ILogger _logger;
        private readonly IEmailDelivery _emailDelivery;
        private readonly IResourceManager _resourceManager;

        public OrderWarehouseService(IRepository<Order> orderRepository,
            IRepository<User> userRepository,
            IRepository<PackageLog> pkgLogRepository,
            IRepository<Location> locationRepository,
            IAppSettings appSettings,
            IAppPolicy appPolicy,
            IDbContext dbContext, IRepository<Department> departmentRepository, ILogger logger, IEmailDelivery emailDelivery, IResourceManager resourceManager) : base(appSettings, appPolicy)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _pkgLogRepository = pkgLogRepository;
            _locationRepository = locationRepository;
            _dbContext = dbContext;
            _departmentRepository = departmentRepository;
            _logger = logger;
            _emailDelivery = emailDelivery;
            _resourceManager = resourceManager;
        }

        public void PackageAllOrder(WorkingUser warehouseStaff, OrderType? orderType = null)
        {
            using (var transation = _dbContext.BeginDbTransaction())
            {
                try
                {
                    var user = _userRepository.TableNoTracking.FirstOrDefault(c => c.Id == warehouseStaff.Id);
                    var departments = GetDepartmentsWithOrderStatus(user.LocationId.Value, new[] { OrderStatus.Approved });
                    var currentDateTime = DateTime.Now;
                    foreach (var department in departments)
                    {
                        var log = new PackageLog
                        {
                            LocationId = user.LocationId.GetValueOrDefault(0),
                            DepartmentId = department.Id,
                            PackedDate = currentDateTime,
                            WarehouseUserId = user.Id,
                            WarehouseUserName = user.UserName,
                            FullPacked = true,
                            Comment = "",
                            OrderType = (int)orderType
                        };

                        _pkgLogRepository.Insert(log);

                        _orderRepository.BulkUpdate(PackageFilter(warehouseStaff, orderType, department.Id),
                        c => new Order { StatusId = (short)OrderStatus.Packaged, PackageLogId = log.Id });
                    }

                    transation.Commit();
                }
                catch (Exception ex)
                {
                    transation.Rollback();
                    _logger.WriteLog($"Cannot package all orders with warehouser {warehouseStaff.UserName} and orderType {orderType}", ex);
                    throw;
                }
            }
        }

        public void PackageOrder(WorkingUser warehouseStaff, int departmentId, OrderType orderType, bool isDeficient = false,
            string note = "")
        {
            using (var transaction = _dbContext.BeginDbTransaction())
            {
                try
                {
                    var user = _userRepository.TableNoTracking.FirstOrDefault(c => c.Id == warehouseStaff.Id);
                    if (user == null)
                    {
                        throw new ArgumentNullException(nameof(warehouseStaff));
                    }

                    var log = new PackageLog
                    {
                        LocationId = user.LocationId.GetValueOrDefault(0),
                        DepartmentId = departmentId,
                        PackedDate = DateTime.Now,
                        WarehouseUserId = user.Id,
                        WarehouseUserName = user.UserName,
                        FullPacked = !isDeficient,
                        Comment = note,
                        OrderType = (int)orderType
                    };

                    _pkgLogRepository.Insert(log);
                    _orderRepository.BulkUpdate(PackageFilter(warehouseStaff, orderType, departmentId),
                        c => new Order { StatusId = (short)OrderStatus.Packaged, PackageLogId = log.Id });

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            // Start sending email to HR Admin and HR Manager when products in packaged orders are deficient.
            if (isDeficient)
            {
                try
                {
                    var hrAdmin = _userRepository.TableNoTracking.FirstOrDefault(c => c.RoleId == (int)UserRole.HRAdmin);
                    var hrManager =
                        _userRepository.TableNoTracking.FirstOrDefault(c => c.RoleId == (int)UserRole.HRManager);
                    if (hrAdmin == null && hrManager == null) throw new NullReferenceException();
                    var packageEmail = new PackageEmailModel
                    {
                        Comment = note,
                        IsFull = false,
                        DepartmentName = _departmentRepository.GetById(departmentId).Name,
                        LocationName = _locationRepository.GetById(warehouseStaff.LocationId).Name,
                        Emails = new[] {new EmailReceiverModel
                        {
                            Id = hrAdmin.Id,
                            Email = hrAdmin.EmailAddress,
                            Name =  hrAdmin.FullName
                        }, new EmailReceiverModel
                        {
                            Id = hrAdmin.Id,
                            Email = hrManager.EmailAddress,
                            Name = hrManager.FullName
                        } }
                    };

                    SendPackageAlert(warehouseStaff, packageEmail);
                }
                catch (Exception ex)
                {
                    _logger.Error("Cannot send order packaging email", ex);
                }
            }

        }

        public IList<Department> GetDepartmentsWithOrderStatus(int locationId, OrderStatus[] orderStatus, OrderBatchStatus orderBatchStatus = OrderBatchStatus.Approved)
        {
            var query =
                _orderRepository
                    .TableNoTracking
                    .IncludeTable(c => c.OrderBatch)
                    .Where(c => c.LocationId == locationId)
                    .Where(c => orderStatus.Any(k => c.StatusId == (int)k) && c.OrderBatch.StatusId == (int)orderBatchStatus)
                    .Where(LatestBatchOrderFilter(GetNearestOrderSessionEndDate()));
            var departments = query.GroupBy(c => c.Department).Select(c => c.Key).Distinct().ToList();

            return departments;
        }
        #region Utils

        private void SendPackageAlert(WorkingUser warehouseStaff, PackageEmailModel packageEmailModel)
        {
            var templatePath = HostingEnvironment.MapPath("~/App_Data/EmailTemplates/PackageEmail.cshtml");
            var content = File.ReadAllText(templatePath);
            var uri = HttpContext.Current.Request.Url;
            var currentHostUrl = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

            foreach (var email in packageEmailModel.Emails)
            {
                var bodyContent = Razor.Parse(content, new
                {
                    ReceiverName = email.Name,
                    SenderName = warehouseStaff.FullName,
                    Content = packageEmailModel.Comment,
                    Host = currentHostUrl,
                    IsPackagedFull = packageEmailModel.IsFull ? "Yes" : "No",
                    packageEmailModel.DepartmentName,
                    packageEmailModel.LocationName
                });
                _emailDelivery.Send("Staffpurchase - Package Alert Message", bodyContent, email.Email, "");
            }

        }

        public bool IsPackaged(WorkingUser warehouseStaff, OrderType? orderType = null, int? departmentId = null)
        {
            var query = _orderRepository
                .TableNoTracking
                .IncludeTable(c => c.OrderBatch);

            if (orderType.HasValue)
            {
                query = query.Where(c => c.TypeId == (int)orderType);
            }

            if (departmentId.HasValue)
            {
                query = query.Where(c => c.DepartmentId == departmentId);
            }

            query = query.Where(c =>
                        c.LocationId == warehouseStaff.LocationId &&
                        c.StatusId == (int)OrderStatus.Packaged);
            query = LatestBatchOrderFilter(query);
            int countPackaged = query.Count();

            return countPackaged > 0;
        }

        public void UpdateOrderAreaBasedOnPackageLog()
        {
            var orderQuery =
                _orderRepository.Table.IncludeTable(c => c.PackageLog)
                    .Where(c => !c.DepartmentId.HasValue && !c.LocationId.HasValue)
                    .Where(c => c.PackageLogId.HasValue).ToList();

            _logger.Info(string.Format("Found {0} orders missing location and department ids", orderQuery.Count));
            _logger.Info("Starting updating order's department and location");

            try
            {
                foreach (var order in orderQuery)
                {
                    order.DepartmentId = order.PackageLog.DepartmentId;
                    order.LocationId = order.PackageLog.LocationId;
                }

                _orderRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to update order location and department based on its package log", ex);
                throw;
            }
        }

        private Expression<Func<Order, bool>> PackageFilter(WorkingUser warehouseStaff, OrderType? orderType = null, int? departmentId = null)
        {
            if (IsPackaged(warehouseStaff, orderType, departmentId))
            {
                throw new StaffingPurchaseException(_resourceManager.GetString("OrderPackage.Validation.AlreadyPackaged"));
            }

            Expression<Func<Order, bool>> filter =
                c => c.LocationId == warehouseStaff.LocationId;
            if (orderType.HasValue)
            {
                filter = filter.AndAlso(c => c.TypeId == (int)orderType);
            }

            if (departmentId.HasValue)
            {
                filter = filter.AndAlso(c => c.DepartmentId == departmentId);
            }

            filter = filter.AndAlso(c => c.OrderBatch.StatusId == (int)OrderBatchStatus.Approved && c.StatusId == (int)OrderStatus.Approved);
            var filter2 = LatestBatchOrderFilter(GetNearestOrderSessionEndDate());
            var body = filter.AndAlso(filter2);
            return body;
        }

        private IQueryable<Order> LatestBatchOrderFilter(IQueryable<Order> query)
        {
            var nearestEndDate = GetNearestOrderSessionEndDate();
            return query.IncludeTable(c => c.OrderBatch).Where(c => DbFunctions.TruncateTime(c.OrderBatch.EndDate) == DbFunctions.TruncateTime(nearestEndDate));
        }

        private static Expression<Func<Order, bool>> LatestBatchOrderFilter(DateTime sessionEndDate)
        {
            Expression<Func<Order, bool>> filter =
                c => DbFunctions.TruncateTime(c.OrderBatch.EndDate) == DbFunctions.TruncateTime(sessionEndDate);

            return filter;
        }
        #endregion Utils
    }
}
