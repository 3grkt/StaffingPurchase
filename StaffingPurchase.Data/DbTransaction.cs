using System.Data.Entity;

namespace StaffingPurchase.Data
{
    public class DbTransaction : IDbTransaction
    {
        private readonly DbContextTransaction _dbContextTransaction;

        public DbTransaction(DbContextTransaction dbContextTransaction)
        {
            _dbContextTransaction = dbContextTransaction;
        }

        #region IDbTransaction Members

        public void Commit()
        {
            _dbContextTransaction.Commit();
        }

        public void Rollback()
        {
            _dbContextTransaction.Rollback();
        }

        public void Dispose()
        {
            _dbContextTransaction.Dispose();
        }

        #endregion
    }
}