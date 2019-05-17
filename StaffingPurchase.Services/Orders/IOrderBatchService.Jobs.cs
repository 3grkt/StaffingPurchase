namespace StaffingPurchase.Services.Orders
{
    public partial interface IOrderBatchService
    {
        /// <summary>
        /// Inits order batches on the date after the last date of order session. This is called by batchjob.
        /// </summary>
        void InitOrderBatches();
    }
}
