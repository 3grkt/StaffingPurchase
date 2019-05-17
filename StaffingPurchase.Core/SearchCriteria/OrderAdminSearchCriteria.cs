using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffingPurchase.Core.SearchCriteria
{
    public class OrderAdminSearchCriteria: SearchCriteriaBase
    {
        public int? LocationId { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime? SessionEndDate { get; set; }
        public string Sku { get; set; }
        public string UserName { get; set; }
        public OrderStatus? OrderStatus { get; set; }
        public OrderType? OrderType { get; set; }
        public OrderBatchStatus? OrderBatchStatus { get; set; }
    }
}
