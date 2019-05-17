using System.Collections.Generic;

namespace StaffingPurchase.Services.Localization
{
    public interface IResourceManager
    {
        string GetString(string resourceKey);
        IDictionary<string, string> GetAllTexts(string locale);
    }
}