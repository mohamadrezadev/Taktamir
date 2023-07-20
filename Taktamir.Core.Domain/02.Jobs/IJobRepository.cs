using System.Diagnostics.SymbolStore;
using System.Security.Principal;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._07.Suppliess;

namespace Taktamir.Core.Domain._01.Jobs
{
    public interface IJobRepository : IRepository<Job>
    {
       Task<Job> GetJobBtid(int id);

        /// <summary>
        /// افزودن لوازم برای کار انجام شده 
        /// </summary>
        /// <param name="idjob"></param>
        /// <param name="newsupplies"></param>
        /// <returns></returns>
       Task<bool> AddSupplies(int idjob, Supplies newsupplies);

        /// <summary>
        /// تغیر وضعیت کار توسط تکنسین 
        /// </summary>
        /// <param name="idjob"></param>
        /// <param name="statusjob"></param>
        /// <returns></returns>
       Task<Job> ChangeStatusjob(int idjob,StatusJob statusjob);

        /// <summary>
        /// لیست همه کارها 
        /// </summary>
        /// <returns></returns>
       Task<List<Job>> GetAllJobs();

        /// <summary>
        /// گرفتن لیسن کار های رزرو شده توسط  تکنسین 
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
       Task<List<Job>> GetUserJobs(int userid);

        /// <summary>
        /// رزرو کار توسط تکنسین 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="job"></param>
        /// <returns></returns>
       Task JobbookingForUser(int userid, Job job);

       /// <summary>
       /// تغیر وضعیت کار توسط ادمین 
       /// </summary>
       /// <param name="idjob"></param>
       /// <param name="statusJob"></param>
       /// <returns></returns>
       Task ChangeStatusJobbyadmin(int idjob ,StatusJob statusJob);

       /// <summary>
        /// لیست کارها در حال انتظار برای رزو شدن توسط ادمین 
        /// </summary>
        /// <returns></returns>
       Task<List<Job>> GetJobwaiting();

        /// <summary>
        /// لیست کارهای در حال انجام برای پنل ادمین 
        /// </summary>
        /// <returns></returns>
        Task<List<Job>> GetAllJobsByAdmin();

       
    }
}
