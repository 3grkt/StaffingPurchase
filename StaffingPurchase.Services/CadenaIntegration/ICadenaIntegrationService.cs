using System.Collections.Generic;
using StaffingPurchase.Core.Domain.Cadena;

namespace StaffingPurchase.Services.CadenaIntegration
{
    public interface ICadenaIntegrationService
    {
        IList<EmployeeInfo> GetCadenaEmployeeInfo();
    }
}
