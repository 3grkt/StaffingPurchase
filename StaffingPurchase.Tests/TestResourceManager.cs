using StaffingPurchase.Services.Localization;

namespace StaffingPurchase.Tests
{
    public class TestResourceManager : IResourceManager
    {
        #region IResourceManager Members

        public string GetString(string resourceKey)
        {
            return resourceKey;
        }

        public System.Collections.Generic.IDictionary<string, string> GetAllTexts(string locale)
        {
            throw new System.NotImplementedException();
        }

        #endregion
}
}
