using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffingPurchase.Web.Models.Common
{
    public class JsonList<T> where T : ViewModelBase
    {
        public int TotalItems { get; set; }
        public IEnumerable<T> Data { get; set; }
        public object Metadata { get; set; }
    }
}
