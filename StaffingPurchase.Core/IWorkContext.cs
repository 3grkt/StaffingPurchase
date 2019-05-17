using System.Globalization;

namespace StaffingPurchase.Core
{
    public interface IWorkContext
    {
        /// <summary>
        /// Gets or sets working culture.
        /// </summary>
        CultureInfo WorkingCulture { get; set; }

        /// <summary>
        /// Gets or sets current working user.
        /// </summary>
        WorkingUser User { get; set; }
    }
}
