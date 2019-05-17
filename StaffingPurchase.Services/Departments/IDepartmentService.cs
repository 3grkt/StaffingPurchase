using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Services.Departments
{
    public interface IDepartmentService
    {
        IDictionary<int, string> GetAllDepartments();
        Department GetByName(string departmentName);
        void Add(Department department, bool inTransaction = false);

        string GetDepartmentName(int departmentId);
    }
}
