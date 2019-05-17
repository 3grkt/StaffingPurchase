namespace StaffingPurchase.Services.Users
{
    public partial interface IUserService
    {
        /// <summary>
        /// Updates PV of users on their bithday.
        /// </summary>
        void UpdatePvOnBirthday();

        /// <summary>
        /// Resets PV of users at end of year.
        /// </summary>
        void ResetPvOnYearEnds();

        /// <summary>
        /// Rewards user PV monthly.
        /// </summary>
        void RewardPvMonthly();

        /// <summary>
        /// Synchronizes user info with Cadena system.
        /// </summary>
        void SyncUserInfoWithCadena();
    }
}
