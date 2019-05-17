using StaffingPurchase.Core;
using StaffingPurchase.Web.Extensions;
using System;
using System.Collections.Generic;

namespace StaffingPurchase.Web.Models.Order
{
    public class OrderBatchModel : ViewModelBase
    {
        #region Properties
        
        public int LocationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public short StatusId { get; set; }
        public DateTime? ActionDate { get; set; }
        public string ActionComment { get; set; }

        //public virtual Location Location { get; set; }
        public IList<OrderModel> Orders { get; set; }

        public int TotalOrders { get; set; }

        public decimal TotalValueWithFilter { get; set; }

        public string OrderSession
        {
            get { return string.Format("{0} - {1}", StartDate.ToLocalizedDate(), EndDate.ToLocalizedDate()); }
        }

        public string UpdatedDate
        {
            get { return ActionDate.ToLocalizedDate(); }
        }

        public string Status
        {
            get { return WebUtility.GetLocalizedStringForEnum((OrderBatchStatus)StatusId); }
        }

        #endregion Properties

        #region Ctors

        public OrderBatchModel()
        {
            Orders = new List<OrderModel>();
        }

        #endregion Ctors
    }
}
