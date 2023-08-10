using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.framework.Common;
using Taktamir.framework.Common.JobsUtill;
using Taktamir.infra.Data.sql._01.Common;
using Taktamir.infra.Data.sql._03.Users;

namespace Taktamir.infra.Data.sql._06.Wallets
{
    public class WalletsRepository : Repository<Wallet>, IWalletRepository
    {
        public WalletsRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        
  
        public async Task<bool> AddNewOrder(int walletId, Order order)
        {
            var wallet = await DbContext.Wallets.FindAsync(walletId);

            if (wallet == null)
                return false;

            if (order == null)
                return false;

            try
            {
                wallet.Orders.Add(order);
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        public  Task<bool> CreateWallet(Wallet wallet)
        {
            if (wallet==null)
            {
                return Task.FromResult(false);
            }
            var userwallet = DbContext.Users
                .Include(p => p.Wallet).FirstOrDefault(p => p.Id == wallet.User.Id);
            if (userwallet == null)
            {
                 DbContext.Wallets.Add(wallet);
                 DbContext.SaveChanges();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public async Task<List<Order>> GetAllOrders(int userid)
        {
            var wallet=await DbContext.Wallets.Include(p => p.Orders).FirstOrDefaultAsync(p => p.User.Id == userid);
            return wallet.Orders.ToList();
        }

        public async Task<Tuple<List<User>, PaginationMetadata>> GetAllWorksbyAdmin(int page = 1, int pageSize = 10)
        {
            var query = DbContext.Users
                    .Include(p => p.Specialties)
                    .Include(w => w.Wallet)
                    .Include(o => o.Wallet.Orders)
                    .ThenInclude(p=>p.OrderJobs)
                            .ThenInclude(j => j.Job)
                                        .ThenInclude(c => c.Customer);


            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var paginationMetadata = new PaginationMetadata(totalCount, totalPages, CurrentPage: page, pageSize);
            return Tuple.Create(users, paginationMetadata);
        }

        public async Task<Tuple<List<User>, PaginationMetadata>> GetAll_Work_pending_Orders(int page = 1, int pageSize = 10)
        {
            var query = DbContext.Users
                    .Include(p => p.Specialties)
                    .Include(w => w.Wallet)
                    .Where(p => p.Wallet.Orders.Any(o => o.OrderJobs.Any(j => j.Job.ReservationStatus == ReservationStatus.ReservedByTec)))
                    .Include(o => o.Wallet.Orders)
                        .ThenInclude(j => j.OrderJobs.Where(j => j.Job.ReservationStatus == ReservationStatus.ReservedByTec))
                            .ThenInclude(j => j.Job)
                                        .ThenInclude(c => c.Customer);

         
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var paginationMetadata = new PaginationMetadata(totalCount, totalPages, CurrentPage: page, pageSize);
            return Tuple.Create(users,paginationMetadata);

        }

        public Task<Order> GetOrderDetails(int orderid)
        {
           var order=DbContext.Orders.FirstOrDefault(p=>p.Id == orderid);
           if (order == null) throw new Exception("Not Found order ....!");
           return Task.FromResult( order);
           //var rsult= order.Jobs.Select(p=>new Job
           //{
           //    Problems = p.Problems,
           //    Id = p.Id,
           //    Description = p.Description,
           //    Name_Device=p.Name_Device,
           //    Supplies = p.Supplies,
           //    Customer=p.Customer,
           //    StatusJob=p.StatusJob,
           //    SuppliesId=p.SuppliesId
           //}).ToList();
        }
    }
}
