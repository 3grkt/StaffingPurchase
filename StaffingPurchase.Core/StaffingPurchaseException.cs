using System;

namespace StaffingPurchase.Core
{
    [Serializable]
    public class StaffingPurchaseException : Exception
    {
        public StaffingPurchaseException() : base()
        {
        }

        public StaffingPurchaseException(string message) : base(message)
        {
        }

        public StaffingPurchaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}