namespace StaffingPurchase.Services.Caching
{
    public interface ICacheService
    {
        T Get<T>(string key) where T : class;
        void Set<T>(string key, T value, int? cacheDuration = null, bool absoluteExpiration = false);
        void Remove(string key);
    }
}
