using System;
using System.Collections.Generic;
using System.IO;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain.Cadena;
using StaffingPurchase.Data;
using StaffingPurchase.Services.Configurations;

namespace StaffingPurchase.Services.CadenaIntegration
{
    public class CadenaIntegrationService : ServiceBase, ICadenaIntegrationService
    {
        private readonly IDbContext _cadenaDataContext;

        public CadenaIntegrationService(IDbContext cadenaDataContext, IAppSettings appSettings, IAppPolicy appPolicy)
            : base(appSettings, appPolicy)
        {
            _cadenaDataContext = cadenaDataContext;
        }

        public IList<EmployeeInfo> GetCadenaEmployeeInfo()
        {
            string queryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _appSettings.CadenaQueryPath);
            string query = File.ReadAllText(queryPath);

            return _cadenaDataContext.ExecuteStoredProcedureList<EmployeeInfo>(query);
        }
    }
}
