using System.Data.Entity.ModelConfiguration.Configuration;
namespace StaffingPurchase.Services.Configurations
{
    public class AppPolicy : IAppPolicy
    {
        private readonly IConfigurationService _configurationService;

        public AppPolicy(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public float BirthDayAwardedPV
        {
            get
            {
                return float.Parse(_configurationService.GetWithCache("BirthDayAwardedPV"));
            }
        }

        public short HighValueProductLimit
        {
            get
            {
                return short.Parse(_configurationService.GetWithCache("HighValueProductLimit"));
            }
        }

        public decimal HighValueProductPrice
        {
            get
            {
                return decimal.Parse(_configurationService.GetWithCache("HighValueProductPrice"));
            }
        }

        public short OrderSessionEndDayOfMonth
        {
            get
            {
                return short.Parse(_configurationService.GetWithCache("OrderSessionEndDayOfMonth"));
            }
        }

        public short OrderSessionStartDayOfMonth
        {
            get
            {
                return short.Parse(_configurationService.GetWithCache("OrderSessionStartDayOfMonth"));
            }
        }

        public string PolicyDocumentFile
        {
            get
            {
                return _configurationService.GetWithCache("PolicyDocumentFile");
            }
        }
    }
}
