using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Services.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffingPurchase.Services.Departments
{
    public class DepartmentService : ServiceBase, IDepartmentService
    {
        private readonly IRepository<Department> _departmentRepository;
        private readonly ICacheService _cacheService;

        public DepartmentService(IRepository<Department> departmentRepository, ICacheService cacheService)
        {
            _departmentRepository = departmentRepository;
            _cacheService = cacheService;
        }

        public IDictionary<int, string> GetAllDepartments()
        {
            var departments = _cacheService.Get<IDictionary<int, string>>(CacheNames.Departments);
            if (departments == null)
            {
                departments = _departmentRepository.TableNoTracking
                    .Select(x => new { x.Id, x.Name })
                    .ToDictionary(x => x.Id, x => x.Name.Trim());

                _cacheService.Set(CacheNames.Departments, departments);
            }
            return departments;
        }

        public Department GetByName(string departmentName)
        {
            var query = _departmentRepository.TableNoTracking;
            return query.FirstOrDefault(x => x.Name == departmentName); // TODO: consider adding DepartmentCode to database
        }

        public void Add(Department department, bool inTransaction = false)
        {
            _departmentRepository.Insert(department, !inTransaction);
        }

        public string GetDepartmentName(int departmentId)
        {
            var department = _departmentRepository.TableNoTracking.FirstOrDefault(x => x.Id == departmentId);
            return department != null ? department.Name : string.Empty;
        }
    }
}
