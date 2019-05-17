using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffingPurchase.Services.Email
{
    public interface IEmailDelivery
    {
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="to"></param>
        /// <param name="from">If empty, will use default system email address</param>
        void Send(string subject, string body, string to, string from);
    }
}
