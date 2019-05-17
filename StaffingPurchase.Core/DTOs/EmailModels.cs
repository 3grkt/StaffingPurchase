using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffingPurchase.Core.DTOs
{

    public class PackageEmailModel
    {
        public string Comment { get; set; }
        public bool IsFull { get; set; }
        public string DepartmentName { get; set; }
        public string LocationName { get; set; }
        public EmailReceiverModel[] Emails { get; set; }
        
    }

    public class EmailReceiverModel
    {
        public string Email { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
