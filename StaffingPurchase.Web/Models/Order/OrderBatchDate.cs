using System;

namespace StaffingPurchase.Web.Models.Order
{
    public class OrderBatchDate
    {
        public DateTime? ActionDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
    }
}