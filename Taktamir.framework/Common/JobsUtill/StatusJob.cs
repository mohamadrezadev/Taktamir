using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taktamir.framework.Common.JobsUtill
{
    public enum StatusJob
    {
        NotReserved = 0,
        /// <summary>
        /// کامل شده
        /// </summary>
        Completed = 1,
        /// <summary>
        /// کنسل شده
        /// </summary>
        Cancel = 2,
        /// <summary>
        /// درحال انجام 
        /// </summary>
        Doing = 3,
        /// <summary>
        /// حمل به کارگاه 
        /// </summary>
        Carry_off = 4,
        /// <summary>
        /// کار زمان بالا 
        /// </summary>
        High_time = 5,
        /// <summary>
        /// در انتظار برای رزرو شدن توسط تکنسین 
        /// </summary>
        waiting = 6,
    }
}
