namespace StaffingPurchase.Services.Configurations
{
    public interface IAppPolicy
    {
        float BirthDayAwardedPV { get; }
        short HighValueProductLimit { get; }
        decimal HighValueProductPrice { get; }
        short OrderSessionStartDayOfMonth { get; }
        short OrderSessionEndDayOfMonth { get; }
        string PolicyDocumentFile { get; }
    }
}
