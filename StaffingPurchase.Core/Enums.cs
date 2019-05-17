namespace StaffingPurchase.Core
{
    /// <summary>
    /// Roles of user. Value is similar to PK of tblRole.
    /// </summary>
    public enum UserRole : short
    {
        Employee = 1,
        Warehouse = 2,
        HRAdmin = 3,
        HRManager = 4,
        IT = 5
    }

    /// <summary>
    /// Permission of user. Value is similar to PK of Permisison table.
    /// </summary>
    public enum UserPermission : short
    {
        ViewPolicy = 1,
        SubmitOrder = 2,
        ViewOrderHistory = 3,
        UpdateOrder = 4,
        ApproveOrder = 5,
        PackOrder = 6,
        CreateAward = 7,
        UploadAwardList = 8,
        MaintainEmployeeList = 9,
        MaintainProductList = 10,
        MaintainLevelGroup = 11,
        MaintainPolicy = 12,
        MaintainUser = 13,
        ReportEmployeeOrders = 14,
        ReportPackagedOrders = 15,
        ReportAllOrders = 16,
        ResetUserPassword = 17,
        ViewBatchjobLog = 18,
        RejectOrder = 19,
        /// <summary>
        /// Views PV log of current user.
        /// </summary>
        ViewPvLog = 20,
        /// <summary>
        /// Views PV log of current and other users.
        /// </summary>
        ViewUserPvLog = 21,
    }

    /// <summary>
    /// Level of logging.
    /// </summary>
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
    }

    /// <summary>
    /// Defines source of log - e.g. Web, BatchJob, etc.
    /// </summary>
    public enum LogSource
    {
        /// <summary>
        /// Source undefined.
        /// </summary>
        None,
        /// <summary>
        /// General source. This is default value if no source specified.
        /// </summary>
        General,
        /// <summary>
        /// Logged from website.
        /// </summary>
        Web,
        /// <summary>
        /// Logged from BatchJob.
        /// </summary>
        BatchJob
    }

    /// <summary>
    /// Status of order.
    /// </summary>
    public enum OrderStatus
    {
        Draft = 101,
        Submitted,
        Approved,
        Packaged,
    }

    /// <summary>
    /// Status of order batch.
    /// </summary>
    public enum OrderBatchStatus : short
    {
        HrAdminPending = 201,
        HrManagerPending,
        RejectedByHrManager,
        Approved
    }

    public enum OrderType
    {
        Cash = 0,
        PV
    }

    public enum PvLogType
    {
        /// <summary>
        /// Default value
        /// </summary>
        None = 0,
        /// <summary>
        /// Rewarded monthly
        /// </summary>
        MonthlyReward = 1,
        /// <summary>
        /// Rewarded on birthday
        /// </summary>
        Birthday = 2,
        /// <summary>
        /// Receive award
        /// </summary>
        Award = 3,
        /// <summary>
        /// Use PV to order
        /// </summary>
        Ordering = 4,
        /// <summary>
        /// Reset PV at end of year
        /// </summary>
        Reset = 5
    }
}
