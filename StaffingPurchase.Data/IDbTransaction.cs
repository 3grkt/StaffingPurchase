using System;

namespace StaffingPurchase.Data
{
    public interface IDbTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}