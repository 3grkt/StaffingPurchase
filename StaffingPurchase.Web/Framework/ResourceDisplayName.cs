using System.ComponentModel;
using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Services.Localization;

namespace StaffingPurchase.Web.Framework
{
    public class ResourceDisplayName : DisplayNameAttribute
    {
        public string ResourceKey { get; set; }

        private static readonly IResourceManager _resoureManager = EngineContext.Current.Resolve<IResourceManager>();

        public ResourceDisplayName(string resourceKey)
        {
            this.ResourceKey = resourceKey;
        }

        public override string DisplayName
        {
            get { return _resoureManager.GetString(ResourceKey); }
        }
    }
}