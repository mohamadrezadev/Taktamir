using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taktamir.framework.Common.OrderUtiil
{
    public enum PaymentStatus
    {
        unpaid = 0,
        paid = 1,
    }
    public  class OrderUtills
    {
        public static string StatusPayment(int status)
        {
            switch (status)
            {
                case 0:
                    return PaymentStatus.unpaid.ToString();
                case 1:
                    return PaymentStatus.paid.ToString();
                default:
                    throw new ArgumentException("Invalid status job value.");
            }
        }
    }
}
