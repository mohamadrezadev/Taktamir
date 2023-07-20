using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.infra.Data.sql._01.Common;

namespace Taktamir.infra.Data.sql._06.Wallets
{
    public class WalletsRepository : Repository<Wallet>, IWalletRepository
    {
        public WalletsRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        //public  Task<bool> AddNewOrder(int walletid, Order order)
        //{
        //    var wallet =  DbContext.Wallets.SingleOrDefault(p => p.Id == walletid);
        //    if (wallet == null) throw new Exception("Wallet not found");

        //    if (order == null) throw new Exception("Order cannot be null");

        //    try
        //    {
        //        wallet.Orders.Add(order);
        //         DbContext.SaveChanges();
        //        return Task.FromResult(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //        return Task.FromResult(false);
        //    }
           
        //}
  
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
                .Include(p => p.Wallet).FirstOrDefault(p => p.Id == wallet.UserId);
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
            var wallet=await DbContext.Wallets.Include(p => p.Orders).FirstOrDefaultAsync(p => p.UserId == userid);
            return wallet.Orders.ToList();
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
