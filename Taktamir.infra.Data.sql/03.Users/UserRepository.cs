using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.framework.Common;
using Taktamir.infra.Data.sql._01.Common;
using Taktamir.infra.Data.sql._06.Wallets;

namespace Taktamir.infra.Data.sql._03.Users
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
        public async override ValueTask<User> GetByIdAsync(CancellationToken cancellationToken, params object[] ids)
        {
            using (var transaction = await DbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var result = await Entities.Include(p=>p.Wallet).Include(p=>p.Room).FirstOrDefaultAsync(p=>p.Id==(int)ids[0], cancellationToken);
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
        public async Task<User> CreateAsync(User entity, CancellationToken cancellationToken)
        {
            using (var transaction = await DbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var wallet = new Wallet();
                    DbContext.Wallets.Add(wallet);;
                    DbContext.SaveChanges();
                    entity.Wallet = wallet;
                    await Entities.AddAsync(entity, cancellationToken).ConfigureAwait(false);
                    await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                    transaction.Commit();
                    return entity;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<User> Finduserbyphonenumber(string phonenumber, CancellationToken cancellationToken)
        {
           var result=await DbContext.Users.FirstOrDefaultAsync(p => p.PhoneNumber.Equals(phonenumber));
            if (result==null)return null;
            return result;

        }
        public Task<bool> TechnicianAccountVerification(int TechnicianId)
        {
            var user = DbContext.Users.FirstOrDefault(p => p.Id == TechnicianId);
            if (user == null) throw new Exception("Not Found Technician Account ....!");
            user.IsActive = true;
            DbContext.Update(user);
            DbContext.SaveChanges();
            return Task.FromResult(true);
        }

        public async Task<Tuple<List<User>, PaginationMetadata>> GetAllUsersAsync(int page = 1, int pageSize = 10)
        {
            var totalCount = await DbContext.Users.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var users = await DbContext.Users.Include(p=>p.Specialties)
                .Include(p => p.Wallet)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var paginationMetadata = new PaginationMetadata(totalCount, totalPages, CurrentPage: page, pageSize);
            
            return Tuple.Create(users, paginationMetadata);

           
        }

        public async Task<Tuple<List<User>, PaginationMetadata>> Unverified_users(int page = 1, int pageSize = 10)
        {
            var totalCount = await DbContext.Users.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var users = await DbContext.Users.Include(p=>p.Specialties)
                .Where(p => !p.IsConfirmedAccount)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var paginationMetadata = new PaginationMetadata(totalCount, totalPages, CurrentPage: page, pageSize);
            return Tuple.Create(users, paginationMetadata);
        }

        public async Task<Tuple<List<User>, PaginationMetadata>> Verified_user_account(int page = 1, int pageSize = 10)
        {
            var totalCount = await DbContext.Users.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var users = await DbContext.Users.Include(p => p.Specialties)
                .Where(p => p.IsConfirmedAccount)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var paginationMetadata = new PaginationMetadata(totalCount, totalPages, CurrentPage: page, pageSize);
            return Tuple.Create(users, paginationMetadata);
        }

        public async Task<Tuple<Wallet, PaginationMetadata>> JobsUser(int walletId, int page = 1, int pageSize = 10)
        {
            
            var totalCount = await DbContext.OrderJobs.Where(p => p.Order.Wallet.Id == walletId).CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var walletuser = await DbContext.Wallets
                .Include(p => p.Orders)
                    .ThenInclude(p => p.OrderJobs)
                        .ThenInclude(p => p.Job)
                            .ThenInclude(p => p.Customer)
                .FirstOrDefaultAsync(p => p.Id == walletId);
            var paginateddata= walletuser.Orders
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToList();

            walletuser.Orders.Clear();
            walletuser.Orders = paginateddata;


            var paginationMetadata = new PaginationMetadata(totalCount, totalPages, CurrentPage: page, pageSize);
            return Tuple.Create(walletuser, paginationMetadata);
        }
    }
}
