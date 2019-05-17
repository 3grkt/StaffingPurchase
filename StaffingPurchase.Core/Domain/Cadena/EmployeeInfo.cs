using System;

namespace StaffingPurchase.Core.Domain.Cadena
{
    public class EmployeeInfo : EntityBase
    {
        public string EmployeeID { get; set; }
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? ServiceStartDate { get; set; }
        public DateTime? ResignationDate { get; set; }
        public short? LevelScale { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string CodeCenterCode { get; set; }
        public string EmailAddress { get; set; }
    }
}
