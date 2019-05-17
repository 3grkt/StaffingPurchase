using StaffingPurchase.Core;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Services.Orders;
using StaffingPurchase.Services.Users;

namespace StaffingPurchase.Jobs.Workers
{
    public class DataUpdateWorker : IWorker
    {
        private readonly IAppSettings _appSettings;
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly IOrderBatchService _orderBatchService;

        public DataUpdateWorker(
            IAppSettings appSettings, 
            ILogger logger, 
            IUserService userService, 
            IOrderBatchService orderBatchService)
        {
            _appSettings = appSettings;
            _logger = logger;
            _userService = userService;
            _orderBatchService = orderBatchService;
        }

        public bool CanWork => _appSettings.DataUpdateWorkerEnabled;

        public int Order => 1; // lower priority

        public void DoWork()
        {
            _logger.Info("======================= Runnning DataUpdateWorker =======================");

            _logger.Info("***Start initing order batches");
            _orderBatchService.InitOrderBatches();
            _logger.Info("***End initing order batches");

            _logger.Info("***Start resetting user's PV at end of year");
            _userService.ResetPvOnYearEnds();
            _logger.Info("***End resetting user's PV at end of year");

            _logger.Info("***Start rewarding user PV monthly");
            _userService.RewardPvMonthly();
            _logger.Info("***End rewarding user PV monthly");

            _logger.Info("***Start updating user's PV on birthday");
            _userService.UpdatePvOnBirthday();
            _logger.Info("***End updating user's PV on birthday");

            _logger.Info("======================= Stopping DataUpdateWorker =======================");

        }
    }
}
