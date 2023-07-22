using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.infra.Data.sql._01.Common;

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
                    var result = await Entities.Include(p=>p.Wallet).FirstOrDefaultAsync(p=>p.Id==(int)ids[0], cancellationToken);
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
        
    }
}
