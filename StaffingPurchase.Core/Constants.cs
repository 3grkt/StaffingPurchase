namespace StaffingPurchase.Core
{
    /// <summary>
    ///     Contains name of caches used in system.
    /// </summary>
    public struct CacheNames
    {
        public const string Awards = "Awards";
        public const string Departments = "Departments";
        public const string Locations = "Locations";
        public const string ProductCategories = "ProductCategories";
        public const string Roles = "Roles";
        public const string WebResources = "WebResources";
        public const string SessionDates = "SessionDates";
        public const string Configurations = "Configurations";
    }

    /// <summary>
    ///     Contains custom claim types.
    /// </summary>
    public struct CustomClaimTypes
    {
        public const string FullName = "FullName";
        public const string DepartmentId = "DepartmentId";
        public const string LocationId = "LocationId";
        public const string Permissions = "Permissions";
        public const string Language = "Language";
    }

    public struct ExcelImportTypes
    {
        public const string AwardList = "AwardList";
        public const string ProductList = "ProductList";
    }

    public struct ExceptionResources
    {
        public const string GeneralException = "Exception";
    }

    public struct OrderSetting
    {
        public const decimal Discount = 0.21M;
    }
}