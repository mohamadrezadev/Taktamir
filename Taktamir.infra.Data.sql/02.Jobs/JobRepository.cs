using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._07.Suppliess;
using Taktamir.framework.Common.JobsUtill;
using Taktamir.infra.Data.sql._01.Common;
using Taktamir.framework.Common.JobsUtill;
namespace Taktamir.infra.Data.sql._02.Jobs
{
    public class JobRepository : Repository<Job>, IJobRepository
    {
        public JobRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
        public override async ValueTask<Job> GetByIdAsync(CancellationToken cancellationToken, params object[] ids)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    var result =await Entities.Include(p=>p.Customer).FirstOrDefaultAsync(p=>p.Id== (int)ids[0]);
                    transaction.Commit();
                    return result;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
           
        }
        public async override Task AddAsync(Job entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            using (var transaction = await DbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await DbContext.Customers.AddAsync(entity.Customer);
                    DbContext.SaveChanges();
                    await Entities.AddAsync(entity, cancellationToken).ConfigureAwait(false);
                    if (saveNow)
                        await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            
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
            job.StatusJob = statusjob;
            DbContext.SaveChanges();
            return Task.FromResult(job);
           
        }

      
        public async Task ChangeStatusJobbyadmin(int idjob, StatusJob statusJob)
        {
           var job=DbContext.Jobs.FirstOrDefault(p=>p.Id==idjob);
           if (job == null) throw new Exception("Not Found Job .....!");
           
           job.StatusJob = statusJob;
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
            var query = DbContext.Jobs.Where(p => p.StatusJob != StatusJob.waiting);

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
            var result=DbContext.Jobs.Where(p=>p.StatusJob==StatusJob.waiting).ToList();
            return Task.FromResult(result);
        }

        //public Task<List<Job>> GetUserJobs(int userid)
        //{
        //    var walletuser=DbContext.Wallets.Include(p=>p.Orders).FirstOrDefault(p=>p.User.Id==userid);
        //    if (walletuser == null) throw new Exception("this id not wallet ");
        //   // var userJobs = walletuser.Orders.Select(o => o.Job).ToList();
        //    //List<Job> UserJob = new List<Job>();
        //    //var result = walletuser.Orders;
        //    //foreach (var order in result)
        //    //{
        //    //    foreach (var item in order.Jobs) 
        //    //    {
        //    //        UserJob.Add(item);
        //    //    }
        //    //}
        //    return Task.FromResult(userJobs);
            
        //}

        public async Task JobbookingForUser(int userid,Job job)
        {
            var walletUser=DbContext.Wallets.FirstOrDefault(p=>p.User.Id==userid);
            job.StatusJob =StatusJob.Doing;
            //job.User.Id = userid;
            //job.User=await DbContext.Users.FirstOrDefaultAsync(p=>p.Id==userid)?? null;
           // walletUser.Orders.ToList().ForEach(order => order.Job=job);
            await DbContext.SaveChangesAsync();

           
        }

        public Task<List<Job>> GetUserJobs(int userid)
        {
            throw new NotImplementedException();
        }
    }
}
