using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using AutoMapper;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Core.Domain.Logging;
using StaffingPurchase.Core.DTOs;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Web.Models.Awards;
using StaffingPurchase.Web.Models.Common;
using StaffingPurchase.Web.Models.Configurations;
using StaffingPurchase.Web.Models.LevelGroup;
using StaffingPurchase.Web.Models.Order;
using StaffingPurchase.Web.Models.Product;
using StaffingPurchase.Web.Models.Role;
using StaffingPurchase.Web.Models.User;
using StaffingPurchase.Web.Models.PV;
using StaffingPurchase.Web.Models.Logging;
using StaffingPurchase.Web.Models.Report;

namespace StaffingPurchase.Web.Infrastructure
{
    public class AutoMapperConfiguration
    {
        private static readonly Lazy<IResourceManager> _resourceManagerLazyObject =
            new Lazy<IResourceManager>(() => EngineContext.Current.Resolve<IResourceManager>());

        protected static IResourceManager ResourceManager
        {
            get { return _resourceManagerLazyObject.Value; }
        }

        public static void ConfigMapping()
        {
            ConfigCommonMapping();

            ConfigUserMapping();

            ConfigLevelMapping();

            ConfigOrderMapping();

            ConfigProductMapping();

            ConfigAwardMapping();

            ConfigPvLogMapping();

            ConfigReportmapping();
        }

        public static void ConfigCommonMapping()
        {
            ConfigDepartmentMapping();

            ConfigLocationMapping();

            ConfigRoleMapping();

            ConfigConfigurationMapping();

            // Logging
            Mapper.CreateMap<LogEntry, LogEntryModel>()
                .ForMember(x => x.Time, mo => mo.MapFrom(x => x.TimeUtc.ToLocalTime()));
        }

        public static void ConfigOrderMapping()
        {
            Mapper.CreateMap<Order, OrderModel>()
                .ForMember(x => x.UserName, mo => mo.MapFrom(x => x.User.FullName))
                .ForMember(x => x.LocationName, mo => mo.MapFrom(x => x.Location.Name))
                .ForMember(x => x.DepartmentName, mo => mo.MapFrom(x => x.Department.Name));

            Mapper.CreateMap<Order, OrderUpdateModel>()
                .ForMember(x => x.Status, mo => mo.MapFrom(x => (OrderStatus)x.StatusId))
                .ForMember(x => x.Type, mo => mo.MapFrom(x => (OrderType)x.TypeId))
                .ForMember(x => x.User, mo => mo.MapFrom(x => x.User.FullName))
                .ForMember(x => x.Department, mo => mo.MapFrom(x => x.Department.Name))
                .ForMember(x => x.Location, mo => mo.MapFrom(x => x.Location.Name))
                .ForMember(x => x.OrderSession, mo => mo.MapFrom(x => x.OrderBatch.OrderSession));

            Mapper.CreateMap<OrderDetail, OrderDetailGridModel>()
                .ForMember(x => x.OrderDetailId, mo => mo.MapFrom(x => x.Id))
                .ForMember(x => x.PV, mo => mo.MapFrom(x => x.Product.PV))
                .ForMember(x => x.Price, mo => mo.MapFrom(x => x.Product.Price))
                .ForMember(x => x.ProductName, mo => mo.MapFrom(x => x.Product.Name));

            Mapper.CreateMap<OrderUpdateModel, Order>()
                .ForMember(d => d.StatusId, s => s.MapFrom(x => (short)x.Status))
                .ForMember(d => d.TypeId, s => s.MapFrom(x => (short)x.Type))
                .ForMember(d => d.User, s => s.Ignore()) // ignore User
                .ForMember(d => d.UserId, s => s.MapFrom(x => x.UserId))
                .ForMember(d => d.OrderDetails, s => s.MapFrom(x => new Collection<OrderDetail>(x.OrderDetails.Select(
                    c => new OrderDetail
                    {
                        OrderId = x.Id,
                        ProductId = c.ProductId,
                        Volume = c.Volume,
                        Id = c.OrderDetailId
                    }
                    ).ToList())));

            // Order Batch
            Mapper.CreateMap<OrderBatch, OrderBatchModel>();
            Mapper.CreateMap<OrderDetail, OrderDetailGridModel>()
                .ForMember(x => x.OrderDetailId, mo => mo.MapFrom(x => x.Id))
                .ForMember(x => x.ProductSku, mo => mo.MapFrom(x => x.Product.Sku))
                .ForMember(x => x.ProductId, mo => mo.MapFrom(x => x.ProductId))
                .ForMember(x => x.ProductName, mo => mo.MapFrom(x => x.Product.Name))
                .ForMember(x => x.PV, mo => mo.MapFrom(x => x.Product.PV ?? 0))
                .ForMember(x => x.Price, mo => mo.MapFrom(x => x.Product.Price ?? 0))
                .ForMember(x => x.Volume, mo => mo.MapFrom(x => x.Volume))
                .ReverseMap();
            Mapper.CreateMap<OrderBatch, OrderBatchModel>();

            Mapper.CreateMap<Order, OrderHistory>()
                .ForMember(x => x.OrderId, mo => mo.MapFrom(x => x.Id))
                .ForMember(x => x.OrderDate, mo => mo.MapFrom(x => x.OrderDate))
                .ForMember(x => x.OrderType, mo => mo.MapFrom(x => ((OrderType)x.TypeId).ToString()))
                .ForMember(x => x.OrderTypeDescription, mo => mo.MapFrom(x => ResourceManager.GetString((OrderType)x.TypeId == OrderType.Cash ? "OrderType.Cash" : "OrderType.PV")))
                .ForMember(x => x.SessionStartDate, mo => mo.MapFrom(x => x.OrderBatch.StartDate))
                .ForMember(x => x.SessionEndDate, mo => mo.MapFrom(x => x.OrderBatch.EndDate))
                .ForMember(x => x.Status, mo => mo.MapFrom(x => ResourceManager.GetString(((OrderStatus)x.StatusId).ToString())))
                .ForMember(x => x.Value, mo => mo.MapFrom(x => x.Value));
        }

        public static void ConfigReportmapping()
        {
            Mapper.CreateMap<DataRow, InternalRequisitionRowModel>()
                .ForMember(x => x.No, mo => mo.MapFrom(x => x["No"]))
                .ForMember(x => x.SKU, mo => mo.MapFrom(x => x["SKU"]))
                .ForMember(x => x.SKUName, mo => mo.MapFrom(x => x["SKUName"]))
                .ForMember(x => x.UnitPrice, mo => mo.MapFrom(x => x["UnitPrice"]))
                .ForMember(x => x.TotalPrice, mo => mo.MapFrom(x => x["TotalPrice"]))
                .ForMember(x => x.PV, mo => mo.MapFrom(x => x["PV"]))
                .ForMember(x => x.TotalPV, mo => mo.MapFrom(x => x["TotalPV"]))
                .ForMember(x => x.Quantity, mo => mo.MapFrom(x => x["Quantity"]))
                .ForMember(x => x.Location, mo => mo.MapFrom(x => x["Location"]))
                .ForMember(x => x.Department, mo => mo.MapFrom(x => x["Department"]));

            Mapper.CreateMap<DataRow, SummaryOrderByIndividualPVRowModel>()
                .ForMember(x => x.No, mo => mo.MapFrom(x => x["No"]))
                .ForMember(x => x.SKU, mo => mo.MapFrom(x => x["SKU"]))
                .ForMember(x => x.SKUName, mo => mo.MapFrom(x => x["SKUName"]))
                .ForMember(x => x.UnitPrice, mo => mo.MapFrom(x => x["UnitPrice"]))
                .ForMember(x => x.TotalPrice, mo => mo.MapFrom(x => x["TotalPrice"]))
                .ForMember(x => x.PV, mo => mo.MapFrom(x => x["PV"]))
                .ForMember(x => x.TotalPV, mo => mo.MapFrom(x => x["TotalPV"]))
                .ForMember(x => x.Quantity, mo => mo.MapFrom(x => x["Quantity"]))
                .ForMember(x => x.Location, mo => mo.MapFrom(x => x["Location"]))
                .ForMember(x => x.Department, mo => mo.MapFrom(x => x["Department"]))
                .ForMember(x => x.Id, mo => mo.MapFrom(x => x["Id"]))
                .ForMember(x => x.Name, mo => mo.MapFrom(x => x["Name"]));

            Mapper.CreateMap<DataRow, SummaryOrderByIndividualDiscountRowModel>()
               .ForMember(x => x.No, mo => mo.MapFrom(x => x["No"]))
               .ForMember(x => x.SKU, mo => mo.MapFrom(x => x["SKU"]))
               .ForMember(x => x.SKUName, mo => mo.MapFrom(x => x["SKUName"]))
               .ForMember(x => x.UnitPrice, mo => mo.MapFrom(x => x["UnitPrice"]))
               .ForMember(x => x.TotalPrice, mo => mo.MapFrom(x => x["TotalPrice"]))
               .ForMember(x => x.UnitDiscountedPrice, mo => mo.MapFrom(x => x["UnitDiscountedPrice"]))
               .ForMember(x => x.TotalDiscountedPrice, mo => mo.MapFrom(x => x["TotalDiscountedPrice"]))
               .ForMember(x => x.Quantity, mo => mo.MapFrom(x => x["Quantity"]))
               .ForMember(x => x.Location, mo => mo.MapFrom(x => x["Location"]))
               .ForMember(x => x.Department, mo => mo.MapFrom(x => x["Department"]))
               .ForMember(x => x.Id, mo => mo.MapFrom(x => x["Id"]))
               .ForMember(x => x.Name, mo => mo.MapFrom(x => x["Name"]));

            Mapper.CreateMap<DataRow, SummaryDiscountProductRowModel>()
                .ForMember(x => x.No, mo => mo.MapFrom(x => x["No"]))
                .ForMember(x => x.SKU, mo => mo.MapFrom(x => x["SKU"]))
                .ForMember(x => x.SKUName, mo => mo.MapFrom(x => x["SKUName"]))
                .ForMember(x => x.UnitPrice, mo => mo.MapFrom(x => x["UnitPrice"]))
                .ForMember(x => x.TotalPrice, mo => mo.MapFrom(x => x["TotalPrice"]))
                .ForMember(x => x.UnitDiscountedPrice, mo => mo.MapFrom(x => x["UnitDiscountedPrice"]))
                .ForMember(x => x.TotalDiscountedPrice, mo => mo.MapFrom(x => x["TotalDiscountedPrice"]))
                .ForMember(x => x.Quantity, mo => mo.MapFrom(x => x["Quantity"]))
                .ForMember(x => x.Location, mo => mo.MapFrom(x => x["Location"]))
                .ForMember(x => x.Department, mo => mo.MapFrom(x => x["Department"]));
        }

        public static void ConfigProductMapping()
        {
            Mapper.CreateMap<Product, ProductModel>()
                .ForMember(x => x.Price, mo => mo.MapFrom(x => x.Price ?? 0))
                .ForMember(x => x.PV, mo => mo.MapFrom(x => x.PV ?? 0))
                .ForMember(x => x.CategoryName, mo => mo.MapFrom(x => x.ProductCategory != null ? x.ProductCategory.Name : string.Empty))
                .ReverseMap();
        }

        private static void ConfigAwardMapping()
        {
            // Awards
            Mapper.CreateMap<Award, AwardModel>();
            Mapper.CreateMap<AwardModel, Award>();
        }

        private static void ConfigLevelMapping()
        {
            Mapper.CreateMap<LevelGroup, LevelGroupModel>()
                .ForMember(x => x.Id, mo => mo.MapFrom(x => x.Id))
                .ForMember(x => x.Name, mo => mo.MapFrom(x => x.Name))
                .ForMember(x => x.PV, mo => mo.MapFrom(x => x.PV)).ReverseMap();

            Mapper.CreateMap<Level, LevelModel>()
                .ForMember(x => x.Id, mo => mo.MapFrom(x => x.Id))
                .ForMember(x => x.Name, mo => mo.MapFrom(x => x.Name))
                .ForMember(x => x.GroupId, mo => mo.MapFrom(x => x.GroupId))
                .ForMember(x => x.GroupName, mo => mo.MapFrom(x => x.LevelGroup.Name))
                .ReverseMap();
        }

        private static void ConfigUserMapping()
        {
            // User
            Mapper.CreateMap<User, UserGridModel>()
                .ForMember(x => x.UserId, mo => mo.MapFrom(x => x.Id))
                .ForMember(x => x.UserName, mo => mo.MapFrom(x => x.UserName))
                .ForMember(x => x.FullName, mo => mo.MapFrom(x => x.FullName))
                .ForMember(x => x.LocationName, mo => mo.MapFrom(x => x.Location.Name))
                .ForMember(x => x.DepartmentName, mo => mo.MapFrom(x => x.Department.Name))
                .ForMember(x => x.RoleName, mo => mo.MapFrom(x => x.Role.Name))
                .ForMember(x => x.Department, m => m.MapFrom(x => x.Department))
                .ForMember(x => x.Location, m => m.MapFrom(x => x.Location))
                .ForMember(x => x.Role, m => m.MapFrom(x => x.Role))
                .ReverseMap();

            Mapper.CreateMap<User, UserModel>()
                .ForMember(x => x.Id, mo => mo.MapFrom(x => x.Id))
                .ForMember(x => x.UserName, mo => mo.MapFrom(x => x.UserName))
                .ForMember(x => x.FullName, mo => mo.MapFrom(x => x.FullName))
                .ForMember(x => x.LocationId, mo => mo.MapFrom(x => x.LocationId))
                .ForMember(x => x.DepartmentId, mo => mo.MapFrom(x => x.DepartmentId))
                .ForMember(x => x.RoleId, mo => mo.MapFrom(x => x.RoleId))
                .ForMember(x=> x.RoleName, mo => mo.MapFrom(x=>x.Role.Name))
                .ReverseMap();
        }

        private static void ConfigDepartmentMapping()
        {
            Mapper.CreateMap<Department, DepartmentModel>();
        }

        private static void ConfigLocationMapping()
        {
            Mapper.CreateMap<Location, LocationModel>();
        }

        private static void ConfigRoleMapping()
        {
            Mapper.CreateMap<Role, RoleModel>();
        }

        private static void ConfigConfigurationMapping()
        {
            Mapper.CreateMap<Configuration, ConfigurationModel>().ReverseMap();
        }

        private static void ConfigPvLogMapping()
        {
            Mapper.CreateMap<PVLog, PvLogModel>().ReverseMap();
            Mapper.CreateMap<PvLogSummaryDto, PvLogSummaryModel>();
        }
    }
}
