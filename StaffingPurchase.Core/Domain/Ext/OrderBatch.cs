using StaffingPurchase.Core.Infrastructure;

namespace StaffingPurchase.Core.Domain
{
    public partial class OrderBatch
    {
        public string OrderSession
        {
            get
            {
                IWebHelper webHelper = EngineContext.Current.Resolve<IWebHelper>();
                return $"{StartDate.ToString(webHelper.DateFormat)} - {EndDate.ToString(webHelper.DateFormat)}";
            }
        }
    }
}
