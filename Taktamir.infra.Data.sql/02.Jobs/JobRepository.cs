using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._07.Suppliess;
using Taktamir.infra.Data.sql._01.Common;

namespace Taktamir.infra.Data.sql._02.Jobs
{
    public class JobRepository : Repository<Job>, IJobRepository
    {
        public JobRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<int> AddNewJob(Job job)
        {
            //var exitjob=DbContext.Jobs.Any(p=>p.Id==job.Id);
            //if (exitjob) return null
            DbContext.Jobs.Add(job);
            await DbContext.SaveChangesAsync();
            return job.Id;
            
        }

        public Task<bool> AddSupplies(int idjob, Supplies newsupplies)
        {
            var job =DbContext.Jobs.Include(p=>p.Supplies).SingleOrDefault(p=>p.Id==idjob);
            if (job == null) throw new Exception("Not Found Job .....!");
            try
            {
                job.Supplies.Add(newsupplies);
                DbContext.SaveChanges();
                return Task.FromResult( true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Task.FromResult( false);
                
            }
            
        }

        public Task<Job> ChangeStatusjob(int idjob, StatusJob statusjob)
        {
            var job=DbContext.Jobs.FirstOrDefault(p=>p.Id==idjob);
            if (job == null) throw new Exception("Not Found Job .....!");
            job.StatusJob = (int)statusjob;
            DbContext.SaveChanges();
            return Task.FromResult(job);
           
        }

      
        public async Task ChangeStatusJobbyadmin(int idjob, StatusJob statusJob)
        {
           var job=DbContext.Jobs.FirstOrDefault(p=>p.Id==idjob);
           if (job == null) throw new Exception("Not Found Job .....!");
           
           job.StatusJob = (int)statusJob;
           job.Reservation = true;
           await DbContext.SaveChangesAsync();

        }

        public async Task<List<Job>> GetAllJobs()
        {
            return await DbContext.Jobs.ToListAsync();
        }

        public Task<List<Job>> GetAllJobsByAdmin()
        {
            //var result=DbContext.Jobs.Where(p=>p.StatusJob!=(int)StatusJob.waiting).ToList();
            var result=DbContext.Jobs.ToList();

            return Task.FromResult(result);
        }
        public Task<List<Job>> GetAllJobsByAdmin(int pageIndex, int pageSize)
        {
            var query = DbContext.Jobs.Where(p => p.StatusJob != (int)StatusJob.waiting);

            var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return Task.FromResult(result);
        }

        public async Task<Job> GetJobBtid(int id)
        {
            var job=await DbContext.Jobs.Include(c=>c.Customer).FirstOrDefaultAsync(p=>p.Id==id);
            return job;
        }

        public Task<List<Job>> GetJobwaiting()
        {
            var result=DbContext.Jobs.Where(p=>p.StatusJob==(int) StatusJob.waiting).ToList();
            return Task.FromResult(result);
        }

        public Task<List<Job>> GetUserJobs(int userid)
        {
            var walletuser=DbContext.Wallets.Include(p=>p.Orders).FirstOrDefault(p=>p.UserId==userid);
            if (walletuser == null) throw new Exception("this id not wallet ");
            var userJobs = walletuser.Orders.Select(o => o.Jobs).ToList();
            //List<Job> UserJob = new List<Job>();
            //var result = walletuser.Orders;
            //foreach (var order in result)
            //{
            //    foreach (var item in order.Jobs) 
            //    {
            //        UserJob.Add(item);
            //    }
            //}
            return Task.FromResult(userJobs);
            
        }

        public async Task JobbookingForUser(int userid,Job job)
        {
            var walletUser=DbContext.Wallets.FirstOrDefault(p=>p.UserId==userid);
            job.StatusJob =(int) StatusJob.Doing;
            job.User.Id = userid;
            job.User=await DbContext.Users.FirstOrDefaultAsync(p=>p.Id==userid)?? null;
            walletUser.Orders.ToList().ForEach(order => order.Jobs=job);
            await DbContext.SaveChangesAsync();

           
        }

        
    }
}
