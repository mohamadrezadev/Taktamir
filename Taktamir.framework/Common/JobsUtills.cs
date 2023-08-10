using Taktamir.framework.Common.JobsUtill;

namespace Taktamir.framework.Common
{
    public static class JobsUtills
    {
        public static string SetStatusJob(int statusjob)
        {
            switch (statusjob)
            {
                case 0:
                    return StatusJob.NotReserved.ToString();
                case 1:
                    return StatusJob.Completed.ToString();
                case 2:
                    return StatusJob.Cancel.ToString();
                case 3:
                    return StatusJob.Doing.ToString();
                case 4:
                    return StatusJob.Carry_off.ToString();
                case 5:
                    return StatusJob.High_time.ToString();
                case 6:
                    return StatusJob.waiting.ToString();
                default:
                    throw new ArgumentException("Invalid status job value.");
            }
        }
        public static string SetReservationStatus(int status)
        {
            switch (status)
            {
                case 0:
                    return ReservationStatus.WatingforReserve.ToString();
                case 1:
                    return ReservationStatus.ReservedByTec.ToString();
                case 2:
                    return ReservationStatus.ConfirmeByAdmin.ToString();
                default:
                    throw new ArgumentException("Invalid status job value.");
            }
        }
    }
}
