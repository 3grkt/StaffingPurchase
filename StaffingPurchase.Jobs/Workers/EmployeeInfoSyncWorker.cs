using StaffingPurchase.Core;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Services.Users;

namespace StaffingPurchase.Jobs.Workers
{
    public class EmployeeInfoSyncWorker : IWorker
    {
        private readonly IAppSettings _appSettings;
        private readonly IUserService _userService;
        private readonly ILogger _logger;

        public EmployeeInfoSyncWorker(IAppSettings appSettings, IUserService userService, ILogger logger)
        {
            _appSettings = appSettings;
            _userService = userService;
            _logger = logger;
        }

        public bool CanWork => _appSettings.EmployeeInfoSyncWorkerEnabled;

        public int Order => 0;

        public void DoWork()
        {
            _logger.Info("======================= Runnning EmployeeInfoSyncWorker =======================");

            _logger.Info("***Start synchronizing with Cadena");
            _userService.SyncUserInfoWithCadena();
            _logger.Info("***End synchronizing with Cadena");

            _logger.Info("======================= Stopping EmployeeInfoSyncWorker =======================");
        }
    }
}
